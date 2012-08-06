using System;
using System.Web;
using NSoft.NFramework;
using NSoft.NFramework.Web;
using NSoft.NFramework.Web.Tools;
using RealWeb.Portal.Controls;

namespace RCL.Web.Pages
{
    /// <summary>
    /// 보안 검사 (사용자 인증)을 수행하지 않고, 모든 요청에 응답하는 웹 페이지입니다.
    /// </summary>
    public abstract class NonAuthenticationPageBase : PageBase
    {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 페이지 접근 이력을 작성합니다.
        /// </summary>
        protected void WriteAccessHistory()
        {
            string pageUrl = string.Empty;
            string loginId = string.Empty;
            try
            {
                //loginId = Identity == null ? WebAppContext.Anonymous : Identity.LoginId;

                if(Local.IsInWebContext)
                {
                    pageUrl = HttpContext.Current.Request.Url.PathAndQuery;

                    if(IsDebugEnabled)
                        log.Debug("접근이력 정보를 작성합니다. Identity:{0} pageUrl:{1}", WebAppContext.Current.Identity, pageUrl);
                }
            }
            catch(Exception ex)
            {
                if(log.IsWarnEnabled)
                    log.Warn(string.Format("접근이력 정보 작성중 오류가 발생하였습니다. Identity:{0} pageUrl:{1}", WebAppContext.Current.Identity, pageUrl), ex);
            }
        }

        /// <summary>
        /// 로그인 처리
        /// </summary>
        /// <returns>로그인 성공여부</returns>
        protected virtual bool LoginProcess()
        {
            if(IsDebugEnabled)
                log.Debug("로그인 사용자 정보:{0}", WebAppContext.Current.Identity);

            if(WebAppContext.Current.Identity != null || AppSettings.Impersonate)
                WebAppContext.Services.Authentication.Login();
            else
            {
                WebAppContext.Services.Authentication.RedirectToLoginPage(false);

                string currentPath = Request.Path;

                //로그인 Url이 없거나 요청페이지와 로그인 페이지가 동일하다면
                if((string.IsNullOrEmpty(WebAppContext.Services.Authentication.LoginUrl)) || (WebAppContext.Services.Authentication.LoginUrl.IndexOf(currentPath) == 0))
                {
                    var msgt = WebAppTool.GetGlobalResourceString(AppSettings.ResourceGlossary, "LoginFailed", "로그인 실패");
                    var msg = WebAppTool.GetGlobalResourceString(AppSettings.ResourceMessages, "NotExistLoginInfo", "로그인 정보가 없습니다");

                    WebAppTool.MessageBox(MessageBoxDisplayKind.Page,
                                         msgt,
                                         msg,
                                         MessageType.Warning,
                                         MessageButtons.Ok | MessageButtons.Login,
                                         endResponse: false);
                }
                else
                    WebAppContext.Services.Authentication.RedirectToLoginPage(false);
            }

            var isLoggined = WebAppContext.Current.Identity != null;

            if(IsDebugEnabled)
                log.Debug("로그인 사용자 정보:{0}, 로그인 성공여부=[{1}]", WebAppContext.Current.Identity, isLoggined);

            return isLoggined;
        }

        /// <summary>
        /// 페이지를 초기화
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if(!IsPostBack)
                WriteAccessHistory();
        }
    }
}