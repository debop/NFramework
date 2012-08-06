using System;
using System.ComponentModel;
using Castle.DynamicProxy;
using NSoft.NFramework.DynamicProxy;

namespace NSoft.NFramework.InversionOfControl {
    public static partial class IoC {
        // private static readonly ProxyGenerator _proxyGenerator = new ProxyGenerator();

        /// <summary>
        /// IoC를 통해 컴포넌트를 Resolve하고, <see cref="INotifyPropertyChanged"/>를 가지는 Proxy로 생성하여 반환합니다.
        /// NOTE: 원본 클래스의 속성이나 메소드가 virtual 이어야만 proxy의 interceptor가 수행합니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T CreateNotifyPropertyChangedProxy<T>() {
            return (T)DynamicProxyTool.CreateNotifyPropertyChanged(() => IoC.Resolve<T>());
        }

        /// <summary>
        /// IoC를 통해 컴포넌트를 Resolve하고, <see cref="INotifyPropertyChanged"/>를 가지는 Proxy로 생성하여 반환합니다.
        /// NOTE: 원본 클래스의 속성이나 메소드가 virtual 이어야만 proxy의 interceptor가 수행합니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="componentId"></param>
        /// <returns></returns>
        public static T CreateNotifyPropertyChangedProxy<T>(string componentId) {
            componentId.ShouldNotBeEmpty("componentId");
            return (T)DynamicProxyTool.CreateNotifyPropertyChanged(() => IoC.Resolve<T>(componentId));
        }

        /// <summary>
        /// IoC를 통해 컴포넌트를 Resolve하고, <see cref="INotifyPropertyChanged"/>를 가지는 Proxy로 생성하여 반환합니다.
        /// NOTE: 원본 클래스의 속성이나 메소드가 virtual 이어야만 proxy의 interceptor가 수행합니다.
        /// </summary>
        /// <param name="typeToProxy"></param>
        /// <returns></returns>
        public static object CreateNotifyPropertyChangedProxy(Type typeToProxy) {
            return DynamicProxyTool.CreateNotifyPropertyChanged(() => IoC.Resolve(typeToProxy));
        }

        /// <summary>
        /// IoC를 통해 컴포넌트를 Resolve하고, <see cref="INotifyPropertyChanged"/>를 가지는 Proxy로 생성하여 반환합니다.
        /// NOTE: 원본 클래스의 속성이나 메소드가 virtual 이어야만 proxy의 interceptor가 수행합니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="componentId"></param>
        /// <param name="additionalInterfacesInterfaceToProxy"></param>
        /// <param name="interceptors"></param>
        /// <returns></returns>
        public static T CreateProxy<T>(string componentId, Type[] additionalInterfacesInterfaceToProxy,
                                       params IInterceptor[] interceptors) where T : class {
            if(IsDebugEnabled)
                log.Debug("ComponentId[{0}]의 형식[{1}]에  INotifyPropertyChanged가 구현된 Proxy를 생성합니다...", componentId,
                          typeof(T).AssemblyQualifiedName);

            return (T)DynamicProxyTool.Create(() => IoC.Resolve<T>(componentId), additionalInterfacesInterfaceToProxy, interceptors);
        }

        /// <summary>
        /// IoC를 통해 컴포넌트를 Resolve하고, <see cref="INotifyPropertyChanged"/>를 가지는 Proxy로 생성하여 반환합니다.
        /// NOTE: 원본 클래스의 속성이나 메소드가 virtual 이어야만 proxy의 interceptor가 수행합니다.
        /// </summary>
        /// <param name="componentType"></param>
        /// <param name="additionalInterfacesInterfaceToProxy"></param>
        /// <param name="interceptors"></param>
        /// <returns></returns>
        public static object CreateProxy(Type componentType, Type[] additionalInterfacesInterfaceToProxy,
                                         params IInterceptor[] interceptors) {
            componentType.ShouldNotBeNull("componentType");

            if(IsDebugEnabled)
                log.Debug("Component 형식[{0}]에  INotifyPropertyChanged가 구현된 Proxy를 생성합니다...", componentType.AssemblyQualifiedName);


            return DynamicProxyTool.Create(() => IoC.Resolve(componentType), additionalInterfacesInterfaceToProxy, interceptors);
        }
    }
}