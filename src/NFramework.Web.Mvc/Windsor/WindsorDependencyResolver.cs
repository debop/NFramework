using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Core;
using NSoft.NFramework.InversionOfControl;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Web.Mvc.Windsor {
    public class WindsorDependencyResolver : System.Web.Mvc.IDependencyResolver {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        ///     Resolves singly registered services that support arbitrary object creation.
        /// </summary>
        /// <returns> The requested service or object. </returns>
        /// <param name="serviceType"> The type of the requested service or object. </param>
        public object GetService(Type serviceType) {
            if(IsDebugEnabled)
                log.Debug("Service를 생성합니다... serviceType=[{0}]", serviceType.FullName);

            if(serviceType.IsInterface || serviceType.IsAbstract)
                return null;

            // Contructor 가 없다면 null 을 반환한다.
            var ciArray = serviceType.GetConstructors();
            if(ciArray.Length == 0)
                return null;

            // 동적으로 생성되는 View 용 Class에 대해서는 Service를 제공할 수 없다.
            if(serviceType.IsSubclassOf(typeof(System.Web.Mvc.WebViewPage)) ||
               serviceType.IsSubclassOf(typeof(System.Web.Mvc.WebViewPage<>)))
                return null;

            return IoC.TryResolve(() => ActivatorTool.CreateInstance(serviceType), true, LifestyleType.Transient);
        }


        /// <summary>
        ///     Resolves multiply registered services.
        /// </summary>
        /// <returns> The requested services. </returns>
        /// <param name="serviceType"> The type of the requested services. </param>
        public IEnumerable<object> GetServices(Type serviceType) { return IoC.ResolveAll(serviceType).Cast<object>(); }
    }
}