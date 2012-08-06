using System;
using NSoft.NFramework.Web;
using NSoft.NFramework.Web.Tools;

namespace RCL.Web.Pages
{
    /// <summary>
    /// Web Application의 기본 Page class 입니다. 
    /// 모든 웹 Application Page는 이 클래스를 상속해야 합니다.
    /// </summary>
    public abstract class DefaultPageBase : NonAuthenticationPageBase
    {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 접근권한 체크
        /// </summary>
        /// <returns></returns>
        protected virtual bool CanAccess
        {
            get { return true; }
        }

        /// <summary>
        /// 로그인 프로세스 Skip 여부
        /// </summary>
        /// <remarks>
        /// TODO: 페이지UI의 기능과 인증기능은 분리되어야 한다. (추후에 분리시킬 예정이며, 이 Property는 분리기능을 임시로 가능케 설정함)
        /// (e.q. <see cref="NHibernateEntityGridPageBase{T}"/>을 사용하는 페이지에서 인증을 원하는 않을 경우도 존재한다.)
        /// </remarks>
        /// <returns></returns>
        protected virtual bool IsSkipLoginProcess
        {
            get { return false; }
        }

        /// <summary>
        /// 페이지 초기화를 수행하는 메소드입니다.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            if(log.IsDebugEnabled)
                log.Debug(@"OnInit Start... url=" + Request.RawUrl);

            if(IsSkipLoginProcess == false)
            {
                var isLoginFailed = CheckUserLogin();
                //var isLoginFailed = (AppSettings.NeedLogin ? CheckUserLogin() : false);

                if(isLoginFailed)
                {
                    if(IsDebugEnabled)
                        log.Debug(@"로그인에 실패했으므로, Page Loading을 중단합니다.");

                    Response.End();
                    return;
                }
            }

            base.OnInit(e);

            if(log.IsDebugEnabled)
                log.Debug(@"OnInit End! url=" + Request.RawUrl);
        }

        private bool CheckUserLogin()
        {
            bool isResponseEnd;
            try
            {
                isResponseEnd = LoginProcess() == false;

                //인증정보가 있다면 접권권한 체크
                if((WebAppContext.Current.Identity != null) && (!string.IsNullOrEmpty(WebAppContext.Current.Identity.LoginId)))
                {
                    if(log.IsDebugEnabled)
                        log.Debug("접속자 정보. LoginId=" + WebAppContext.Current.Identity.LoginId);

                    if(IsPostBack == false && CanAccess == false)
                    {
                        WebAppTool.ShowMessageOfAccessDenied();
                        isResponseEnd = true;
                    }
                }
            }
            catch(Exception ex)
            {
                if(log.IsErrorEnabled)
                    log.ErrorException(@"페이지 로드중 오류가 발생하였습니다.", ex);

                WebAppTool.ShowMessageOfLoginFail();
                isResponseEnd = true;
            }

            return isResponseEnd;
        }
    }
}