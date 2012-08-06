using System.Web.Mvc;

namespace NSoft.NFramework.Web.Mvc
{
    public class SimplifiedRedirectResult : ActionResult
    {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public SimplifiedRedirectResult(string url, bool permanent = false)
        {
            if(IsDebugEnabled)
                log.Debug("SimplifiedRedirectResult를 생성합니다. url=[{0}], permanent=[{1}]", url, permanent);

            Url = url;
            Permanent = permanent;
        }

        public string Url { get; private set; }

        public bool Permanent { get; private set; }

        /// <summary>
        /// Enables processing of the result of an action method by a custom type that inherits from the <see cref="T:System.Web.Mvc.ActionResult"/> class.
        /// </summary>
        /// <param name="context">The context in which the result is executed. The context information includes the controller, HTTP content, request context, and route data.</param>
        public override void ExecuteResult(ControllerContext context)
        {
            if(IsDebugEnabled)
                log.Debug("실제 Redirect를 수행합니다...");

            var destinationUrl = UrlHelper.GenerateContentUrl(Url, context.HttpContext);
            context.Controller.TempData.Keep();

            if(Permanent)
            {
                context.HttpContext.Response.RedirectPermanent(destinationUrl, endResponse: false);
            }
            else
            {
                context.HttpContext.Response.Redirect(destinationUrl, endResponse: false);
            }
        }
    }
}