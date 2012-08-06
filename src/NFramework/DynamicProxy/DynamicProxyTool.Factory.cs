using System;
using System.ComponentModel;
using Castle.DynamicProxy;
using NSoft.NFramework.Reflections;

namespace NSoft.NFramework.DynamicProxy {

    public static partial class DynamicProxyTool {

        // NOTE: 하나의 ProxyGenerator로 같은 수형에 대해 CreateClassProxy()와 CreateClassProxyWithTarget()을 동시에 사용하면 예외가 발생합니다.
        // NOTE: 그래서 Type으로 Proxy를 생성하는 ProxyGenerator, Target으로 Proxy를 생성하는 ProxyGenerator를 따로 나누었습니다.
        //
        // NOTE: NHibernate에서 Proxy를 만들 때에는 무조건 Type으로 Proxy를 만들어야 합니다!!!

        private static readonly ProxyGenerator proxyGenerator = new ProxyGenerator();
        private static readonly ProxyGenerator proxyGeneratorWithTarget = new ProxyGenerator();

        private static readonly Type[] notifyPropertyChangedTypes = new[] { typeof(INotifyPropertyChanged) };
        private static readonly Type[] editableObjectTypes = new[] { typeof(IEditableObject) };
        private static readonly Type[] editablePropertyChangedTypes = new[] { typeof(INotifyPropertyChanged), typeof(IEditableObject) };

        /// <summary>
        /// 지정된 객체 생성 메소드를 통해 원본 객체를 생성하고, Interceptor를 가지는 Proxy를 생성하여 반환합니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="additionalInterfaces"></param>
        /// <param name="interceptors"></param>
        /// <returns></returns>
        public static T Create<T>(Type[] additionalInterfaces, params IInterceptor[] interceptors) where T : class {
            return (T)Create(typeof(T), additionalInterfaces, interceptors);
        }

        /// <summary>
        /// 지정된 객체 생성 메소드를 통해 원본 객체를 생성하고, Interceptor를 가지는 Proxy를 생성하여 반환합니다.
        /// </summary>
        /// <param name="classToProxy"></param>
        /// <param name="additionalInterfaces"></param>
        /// <param name="interceptors"></param>
        /// <returns></returns>
        public static object Create(Type classToProxy, Type[] additionalInterfaces, params IInterceptor[] interceptors) {
            classToProxy.ShouldNotBeNull("classToProxy");

            if(IsDebugEnabled)
                log.Debug("대상 수형[{0}]의 Proxy를 생성합니다. additionalInterfaces=[{1}], interceptors=[{2}]",
                          classToProxy.FullName, additionalInterfaces.CollectionToString(), interceptors.CollectionToString());

            return proxyGenerator.CreateClassProxy(classToProxy,
                                                   additionalInterfaces,
                                                   interceptors);
        }

        /// <summary>
        /// 지정된 객체 생성 메소드를 통해 원본 객체를 생성하고, Interceptor를 가지는 Proxy를 생성하여 반환합니다.
        /// </summary>
        /// <param name="targetFactory"></param>
        /// <param name="additionalInterfaces"></param>
        /// <param name="interceptors"></param>
        /// <returns></returns>
        public static object Create(Func<object> targetFactory, Type[] additionalInterfaces, params IInterceptor[] interceptors) {
            targetFactory.ShouldNotBeNull("targetFactory");

            var target = targetFactory();
            target.ShouldNotBeNull("target");

            if(IsDebugEnabled)
                log.Debug("대상 객체[{0}]에 Proxy를 생성합니다. additionalInterfaces=[{1}], interceptors=[{2}]",
                          target.GetType().FullName, additionalInterfaces.CollectionToString(), interceptors.CollectionToString());

            return proxyGeneratorWithTarget.CreateClassProxyWithTarget(target.GetType(),
                                                                       additionalInterfaces,
                                                                       target,
                                                                       interceptors);
        }

        /// <summary>
        /// <see cref="INotifyPropertyChanged"/> 인터페이스를 구현한 것처럼 감싸주는 Proxy를 생성합니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T CreateNotifyPropertyChanged<T>() where T : class {
            return (T)CreateNotifyPropertyChanged(typeof(T));
        }

        /// <summary>
        /// 지정된 수형에서 <see cref="INotifyPropertyChanged"/> 를 구현한 Proxy로 생성합니다.
        /// </summary>
        /// <param name="classToProxy">Proxy 대상 수형</param>
        /// <returns></returns>
        public static object CreateNotifyPropertyChanged(Type classToProxy) {
            return Create(classToProxy,
                          notifyPropertyChangedTypes,
                          new NotifyPropertyChangedInterceptor());
        }

        /// <summary>
        /// 지정된 수형에서 <see cref="INotifyPropertyChanged"/> 를 구현한 Proxy로 생성합니다.
        /// </summary>
        /// <param name="targetFactory">Proxy 대상 인스턴스 생성 Factory</param>
        /// <returns></returns>
        public static object CreateNotifyPropertyChanged(Func<object> targetFactory) {
            return Create(targetFactory,
                          notifyPropertyChangedTypes,
                          new NotifyPropertyChangedInterceptor());
        }

        /// <summary>
        /// 지정된 수형에서 <see cref="IEditableObject"/> 를 구현한 Proxy로 생성합니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T CreateEditableObject<T>() where T : class {
            return (T)CreateEditableObject(typeof(T));
        }

        /// <summary>
        /// 지정된 수형에서 <see cref="IEditableObject"/> 를 구현한 Proxy로 생성합니다.
        /// </summary>
        /// <param name="classToProxy">Proxy 대상 수형</param>
        /// <returns></returns>
        public static object CreateEditableObject(Type classToProxy) {
            return Create(classToProxy,
                          editableObjectTypes,
                          new EditableObjectInterceptor());
        }

        /// <summary>
        /// 지정된 수형에서 <see cref="IEditableObject"/> 를 구현한 Proxy로 생성합니다.
        /// </summary>
        /// <param name="targetFactory">Proxy 대상 인스턴스 생성 Factory</param>
        /// <returns></returns>
        public static object CreateEditableObject(Func<object> targetFactory) {
            return Create(targetFactory,
                          editableObjectTypes,
                          new EditableObjectInterceptor());
        }

        /// <summary>
        /// 지정된 수형에서 <see cref="IEditableObject"/>, <see cref="INotifyPropertyChanged"/> 를 구현한 Proxy로 생성합니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T CreateEditablePropertyChanged<T>() where T : class {
            return (T)CreateEditablePropertyChanged(typeof(T));
        }

        /// <summary>
        /// 지정된 수형에서 <see cref="IEditableObject"/>, <see cref="INotifyPropertyChanged"/> 를 구현한 Proxy로 생성합니다.
        /// </summary>
        /// <param name="classToProxy">Proxy 대상 수형</param>
        /// <returns></returns>
        public static object CreateEditablePropertyChanged(Type classToProxy) {
            return proxyGenerator.CreateClassProxy(classToProxy,
                                                   editablePropertyChangedTypes,
                                                   new NotifyPropertyChangedInterceptor(),
                                                   new EditableObjectInterceptor());
        }

        /// <summary>
        /// 지정된 수형에서 <see cref="IEditableObject"/>, <see cref="INotifyPropertyChanged"/> 를 구현한 Proxy로 생성합니다.
        /// </summary>
        /// <param name="targetFactory">Proxy 대상 인스턴스 생성 Factory</param>
        /// <returns></returns>
        public static object CreateEditablePropertyChanged(Func<object> targetFactory) {
            return Create(targetFactory,
                          editablePropertyChangedTypes,
                          new NotifyPropertyChangedInterceptor(),
                          new EditableObjectInterceptor());
        }
    }
}