using System;
using System.Reflection;
using NSoft.NFramework.Reflections;

namespace NSoft.NFramework.Tools {
    /// <summary>
    /// 특정 수형을 생성해주는 Activator
    /// </summary>
    public static class ActivatorTool {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        ///// <summary>
        ///// 지정된 타입명(클래스 이름)을 가진 클래스에 대해 동적으로 기본 생성자를 이용하여 인스턴스를 생성한다.
        ///// </summary>
        ///// <param name="typeName">타입 명</param>
        ///// <returns>동적으로 생성된 객체</returns>
        //public static object CreateInstance(this string typeName)
        //{
        //	return CreateInstance(typeName, new object[0]);
        //}

        /// <summary>
        /// 지정된 타입명(클래스 이름)을 가진 클래스에 대해 동적으로 인스턴스를 생성한다.
        /// </summary>
        /// <param name="typeName">타입명</param>
        /// <param name="args">생성자 호출시의 인자들</param>
        /// <returns>동적으로 생성된 객체</returns>
        public static object CreateInstance(this string typeName, object[] args) {
            var type = Type.GetType(typeName, true, false);
            return CreateInstance(type, args);
        }

        /// <summary>
        /// 지정된 타입명(클래스 이름)을 가진 클래스에 대해 동적으로 기본 생성자를 이용하여 인스턴스를 생성한다.
        /// </summary>
        /// <param name="typeName">생성할 객체의 타입명</param>
        /// <param name="nonPublic">비공용 생성자를 사용할 수 있는지 여부, Singleton 패턴의 경우 기본 생성자가 private이므로 이런 클래스의 생성시에는 nonPublic을 true 로 해주어야 한다.</param>
        /// <returns>동적으로 생성된 객체</returns>
        public static object CreateInstance(this string typeName, bool nonPublic = true) {
            var type = Type.GetType(typeName, true, false);
            return CreateInstance(type, nonPublic);
        }

        //		/// <summary>
        //		/// 지정한 타입의 인스턴스를 동적으로 생성한다.
        //		/// </summary>
        //		/// <param name="type">타입</param>
        //		/// <returns>동적으로 생성된 인스턴스</returns>
        //		public static object CreateInstance(this Type type)
        //		{
        //#if !SILVERLIGHT
        //			return FasterflectTool.GetConstructorInvoker(type)();
        //#else
        //			return Activator.CreateInstance(type);
        //#endif
        //		}

#if !SILVERLIGHT
        /// <summary>
        /// 지정한 타입의 인스턴스를 동적으로 생성한다.
        /// </summary>
        /// <param name="type">타입</param>
        /// <returns>동적으로 생성된 인스턴스</returns>
        /// <param name="bindingFlags">Binding Flags</param>
        public static object CreateInstance(this Type type, BindingFlags bindingFlags) {
            if(IsDebugEnabled)
                log.Debug("수형[{0}]을 동적으로 생성합니다. bindingFlags=[{1}]", type.FullName, bindingFlags);

            return FasterflectTool.GetConstructorInvoker(type, bindingFlags)();
        }
#endif

        /// <summary>
        /// 지정한 타입의 인스턴스를 동적으로 생성한다.
        /// </summary>
        /// <param name="type">타입</param>
        /// <param name="args">생성자의 인자값</param>
        /// <returns>동적으로 생성된 인스턴스</returns>
        public static object CreateInstance(this Type type, object[] args) {
#if !SILVERLIGHT
            return Fasterflect.ConstructorExtensions.CreateInstance(type, Fasterflect.Flags.InstanceAnyVisibility, args);
#else
			return Activator.CreateInstance(type, args);
#endif
        }

        /// <summary>
        /// 지정한 타입의 인스턴스를 동적으로 생성한다.
        /// </summary>
        /// <param name="type">타입</param>
        /// <param name="nonPublic">비공용 생성자를 사용할 수 있는지 여부, Singleton 패턴의 경우 기본 생성자가 private이므로 이런 클래스의 생성시에는 nonPublic을 true 로 해주어야 한다.</param>
        /// <returns>동적으로 생성된 인스턴스</returns>
        public static object CreateInstance(this Type type, bool nonPublic = true) {
            var flags = BindingFlags.Instance | BindingFlags.Public;

            if(nonPublic)
                flags |= BindingFlags.NonPublic;

#if !SILVERLIGHT
            return CreateInstance(type, flags);
#else
			return Activator.CreateInstance(type);
#endif
        }

        /// <summary>
        /// Generic을 이용하여 지정된 타입을 기본생성자를 이용하여 동적으로 인스턴스를 생성시킨다.
        /// </summary>
        /// <typeparam name="T">생성할 인스턴스의 타입</typeparam>
        public static T CreateInstance<T>() {
#if !SILVERLIGHT
            return CreateInstance<T>(Fasterflect.Flags.InstanceAnyVisibility);
#else
			return (T) CreateInstance(typeof (T));
#endif
        }

        /// <summary>
        /// 지정된 인자를 가지는 생성자를 호출하여 인스턴스를 생성합니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args"></param>
        /// <returns></returns>
        public static T CreateInstance<T>(object[] args) {
            return (T)CreateInstance(typeof(T), args);
        }

        /// <summary>
        /// 지정된 인자를 가지는 생성자를 호출하여 인스턴스를 생성합니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bindingFlags"></param>
        /// <returns></returns>
        public static T CreateInstance<T>(BindingFlags bindingFlags) {
#if !SILVERLIGHT
            return (T)Fasterflect.ConstructorExtensions.CreateInstance(typeof(T), bindingFlags);
#else
			return (T) Activator.CreateInstance(typeof(T), bindingFlags, null, null);
#endif
        }

        /// <summary>
        /// 지정된 인자를 가지는 생성자를 호출하여 인스턴스를 생성합니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bindingFlags"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static T CreateInstance<T>(BindingFlags bindingFlags, object[] args) {
#if !SILVERLIGHT
            return (T)Fasterflect.ConstructorExtensions.CreateInstance(typeof(T), bindingFlags, args);
#else
			return (T) Activator.CreateInstance(typeof(T), bindingFlags, null, args, null);
#endif
        }
    }
}