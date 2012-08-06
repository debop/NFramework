using System;
using Castle.DynamicProxy;

namespace NSoft.NFramework.DynamicProxy {

    /// <summary>
    /// Castle.DynamicProxy 에 대한 확장 메소드를 제공합니다.
    /// </summary>
    public static partial class DynamicProxyTool {

        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 지정된 객체가 <see cref="ProxyGenerator"/>로 생성된 Proxy인지 파악합니다.
        /// </summary>
        /// <param name="proxy"></param>
        /// <returns></returns>
        public static bool IsDynamicProxy(this object proxy) {
            return (proxy != null) && IsDynamicProxyType(proxy.GetType());
        }

        /// <summary>
        /// 지정된 형식이 DynamicProxy 로 생성된 Proxy의 수형인지 파악합니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool IsDynamicProxyType<T>() {
            return typeof(T).IsDynamicProxyType();
        }

        /// <summary>
        /// 지정된 형식이 DynamicProxy 로 생성된 Proxy의 수형인지 파악합니다.
        /// </summary>
        /// <param name="proxiedType"></param>
        /// <returns></returns>
        public static bool IsDynamicProxyType(this Type proxiedType) {
            if(proxiedType == null)
                return false;

            var isProxyType = (proxiedType.BaseType != null && proxiedType.Assembly.IsDynamic);

            if(IsDebugEnabled)
                log.Debug("수형[{0}] 가 Proxy 수형인지 파악했습니다. IsDynamicProxyType=[{1}]", proxiedType, isProxyType);

            return isProxyType;
        }

        /// <summary>
        /// 지정된 수형이 Proxy 수형이라면, 기본 수형을 반환합니다.
        /// </summary>
        /// <param name="proxiedType"></param>
        /// <returns></returns>
        public static Type GetUnproxiedType(this Type proxiedType) {
            proxiedType.ShouldNotBeNull("proxiedType");

            if(IsDebugEnabled)
                log.Debug("Proxy 수형[{0}]의 원본 수형을 찾습니다...", proxiedType.Name);

            return proxiedType.IsDynamicProxyType() ? proxiedType.BaseType : proxiedType;
        }
    }
}