using System.Web.UI;

namespace NSoft.NFramework.Web.HttpHandlers.RouteHandlers
{
    /// <summary>
    /// Index 화면의 Route Handler
    /// </summary>
    public class IndexRouteHandler : RouteHandlerBase<Page>
    {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 경로
        /// </summary>
        public const string IndexPageFormatString = @"~/Index/{0}/Index.aspx";

        /// <summary>
        /// 처리할 Handler 경로
        /// </summary>
        public override string GetVirtualPath()
        {
            return string.Format(IndexPageFormatString, WebAppContext.Current.ThemeName);
        }
    }
}