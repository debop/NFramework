using System.Web.UI;

namespace NSoft.NFramework.Web.HttpHandlers.RouteHandlers
{
    /// <summary>
    /// MainPage 화면의 Route Handler
    /// </summary>
    public class MainPageRouteHandler : RouteHandlerBase<Page>
    {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public const string MainPageFormatString = @"~/Index/{0}/Main.aspx";

        /// <summary>
        /// 처리할 Handler 경로
        /// </summary>
        public override string GetVirtualPath()
        {
            return string.Format(MainPageFormatString, WebAppContext.Current.ThemeName);
        }
    }
}