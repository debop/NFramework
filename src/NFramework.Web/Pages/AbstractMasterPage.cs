namespace NSoft.NFramework.Web.Pages
{
    /// <summary>
    /// 웹 어플리케이션의 기본 마스터 페이지입니다.
    /// </summary>
    public abstract class AbstractMasterPage : System.Web.UI.MasterPage
    {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion
    }
}