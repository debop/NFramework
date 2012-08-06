using System;

namespace RCL.Web.Pages
{
    /// <summary>
    /// Web Application의 기본 MasterPage 입니다.
    /// </summary>
    public abstract class DefaultMasterPageBase : System.Web.UI.MasterPage
    {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion
    }
}