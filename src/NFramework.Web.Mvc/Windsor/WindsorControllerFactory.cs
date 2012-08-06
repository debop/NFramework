using System;
using NSoft.NFramework.InversionOfControl;
using NSoft.NFramework.Reflections;

namespace NSoft.NFramework.Web.Mvc.Windsor {
    public class WindsorControllerFactory : System.Web.Mvc.DefaultControllerFactory {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        protected override System.Web.Mvc.IController GetControllerInstance(System.Web.Routing.RequestContext requestContext, Type controllerType) {

            if(controllerType == null || !controllerType.HasInterface(typeof(System.Web.Mvc.IController)))
                return base.GetControllerInstance(requestContext, controllerType);

            if(IsDebugEnabled)
                log.Debug("경로 [{0}]에 해당하는 Controller [{1}] 를 생성합니다...", requestContext.HttpContext.Request.Path, controllerType);

            return (System.Web.Mvc.IController)IoC.Resolve(controllerType);
        }
    }
}