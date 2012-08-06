using System;
using System.Security.Principal;
using System.Threading;
using System.Web;
using NSoft.NFramework.Web.Access;
using NSoft.NFramework.Web.Tools;

namespace NSoft.NFramework.Web.Services.AuthenticationService
{
    /// <summary>
    /// 인증 서비스를 제공하는 Base Class입니다.
    /// </summary>
    public abstract class AuthenticationServiceBase : IAuthenticationService
    {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 로그인정보의 데이터 저장소의 KeyName
        /// </summary>
        protected static readonly string ApplicationKey = AppSettings.ApplicationName;

        /// <summary>
        /// 로그인정보를 표현하는 객체
        /// </summary>
        public virtual IAccessIdentity Identity
        {
            get { return (WebAppTool.LoadValue(ApplicationKey, default(AccessIdentity))); }
        }

        /// <summary>
        /// 접속자의 Id
        /// </summary>
        public virtual string LoginUserName
        {
            get { return AppSettings.Impersonate ? AppSettings.ImpersonateUserName : WebAppContext.Current.Identity.LoginId; }
        }

        /// <summary>
        /// 로그인 Url
        /// </summary>
        public virtual string LoginUrl
        {
            get { return WebAppTool.ResolveUrl(AppSettings.LoginUrl); }
        }

        /// <summary>
        /// 기본 Url
        /// </summary>
        public virtual string DefaultUrl
        {
            get { return WebAppTool.ResolveUrl(AppSettings.DefaultUrl); }
        }

        /// <summary>
        /// 사용자 검증을 수행합니다.
        /// </summary>
        /// <param name="userId">로그인Id</param>
        /// <param name="password">비밀번호</param>
        /// <returns>검증여부</returns>
        public abstract bool VerifyUser(string userId, string password);

        /// <summary>
        /// 로그인.
        /// </summary>
        /// <param name="userName">로그인Id</param>
        /// <param name="password">비밀번호</param>
        /// <param name="redirectDefault">로그인 후 기본페이지 이동여부</param>
        /// <remarks>사용자인증 수행 후, 인증정보를 저장합니다.</remarks>
        public void Login(string userName = null, string password = null, bool redirectDefault = true)
        {
            ProcessingLogin(userName, password);
            WriteLoginInfo();

            if(redirectDefault)
                RedirectToDefaultPage(true);
        }

        /// <summary>
        /// 접근정보로 로그인 처리한다.
        /// </summary>
        public virtual void ProcessingLogin()
        {
            ProcessingLogin(LoginUserName);
        }

        /// <summary>
        /// 로그인 처리한다.
        /// </summary>
        /// <param name="userName">로그인Id</param>
        public virtual void ProcessingLogin(string userName)
        {
            if(log.IsDebugEnabled)
                log.Debug("==>>S 로그인 처리합니다. enterpriseName:{0}, userName:{1}",
                          AppSettings.EnterpriseName, userName);

            var identity = AppSettings.Impersonate ? CreateIdentityAsImpersonate(userName) : CreateIdentity(userName);

            if(identity == null)
                throw new InvalidOperationException(WebAppTool.GetGlobalResourceString(AppSettings.ResourceMessages,
                                                                                      "NotExistLoginInfo", "로그인 정보가 없습니다."));

            SetIdentity(identity);
            SetAuthData(identity.LoginId);

            if(log.IsDebugEnabled)
                log.Debug("==>>E 로그인 처리하였습니다. enterpriseName:{0}, userName:{1}",
                          AppSettings.EnterpriseName, userName);
        }

        /// <summary>
        /// 로그인 처리한다.
        /// </summary>
        /// <param name="userName">로그인Id</param>
        /// <param name="password">비밀번호</param>
        public virtual void ProcessingLogin(string userName, string password)
        {
            if(log.IsDebugEnabled)
                log.Debug("==>>S 로그인 처리합니다. enterpriseName:{0}, userName:{1}, password:{2}",
                          AppSettings.EnterpriseName, userName, password);

            if(AppSettings.Impersonate == false)
            {
                bool isVerified = VerifyUser(userName, password);

                if(isVerified == false)
                    throw new InvalidOperationException(WebAppTool.GetGlobalResourceString(
                        AppSettings.ResourceMessages,
                        "LoginDifferentInfo", "로그인 정보가 일치하지 않습니다."));
            }

            var identity = AppSettings.Impersonate ? CreateIdentityAsImpersonate(userName) : CreateIdentity(userName);

            SetIdentity(identity);
            SetAuthData(identity.LoginId);

            if(log.IsDebugEnabled)
                log.Debug("==>>E 로그인 처리하였습니다. enterpriseName:{0}, userName:{1}, password:{2}",
                          AppSettings.EnterpriseName, userName, password);
        }

        /// <summary>
        /// 로그아웃한다.
        /// </summary>
        /// <param name="redirectLogin">로그인페이지로 이동</param>
        /// <param name="endReponse">프로세스 종료</param>
        public virtual void Logout(bool redirectLogin = true, bool endReponse = true)
        {
            Logout();

            if(redirectLogin)
                RedirectToLoginPage(endReponse);
        }

        /// <summary>
        /// 로그인 페이지로 이동
        /// </summary>
        /// <param name="endReponse">프로세스 종료</param>
        public virtual void RedirectToLoginPage(bool endReponse)
        {
            if(log.IsDebugEnabled)
                log.Debug("==>>S 로그인 페이지로 이동작업을 시작합니다.");

            var returnPathAndQuery = HttpContext.Current.Request.RawUrl == WebAppTool.ResolveUrl(AppSettings.LogoutUrl)
                                         ? AppSettings.DefaultUrl
                                         : HttpContext.Current.Request.Url.PathAndQuery;

            string url = WebAppTool.UrlParamConcat(LoginUrl, string.Format("ReturnUrl=[{0}]", returnPathAndQuery.UrlEncode()));

            if(log.IsDebugEnabled)
                log.Debug("{0}로 이동합니다.", url);

            HttpContext.Current.Response.Redirect(url, endReponse);

            if(log.IsDebugEnabled)
                log.Debug("==>>E 로그인 페이지로 이동작업을 완료합니다.");
        }

        /// <summary>
        /// 기본 페이지로 이동
        /// </summary>
        /// <param name="endReponse">프로세스 종료</param>
        public virtual void RedirectToDefaultPage(bool endReponse)
        {
            if(log.IsDebugEnabled)
                log.Debug("기본 페이지로 이동작업을 시작합니다...");

            string currentPathAndQuery = HttpContext.Current.Request.Url.PathAndQuery;
            string url = WebAppTool.UrlParamConcat(DefaultUrl, currentPathAndQuery.UrlEncode());

            if(log.IsDebugEnabled)
                log.Debug("페이지 이동... url=[{0}]", url);

            HttpContext.Current.Response.Redirect(url, endReponse);

            if(log.IsDebugEnabled)
                log.Debug("기본 페이지로 이동작업을 완료합니다.");
        }

        /// <summary>
        /// Identity 정보를 가장으로 생성하여 반환합니다.
        /// </summary>
        /// <param name="loginId"></param>
        /// <returns></returns>
        private static IAccessIdentity CreateIdentityAsImpersonate(string loginId)
        {
            return new AccessIdentity
                   {
                       CompanyCode = AppSettings.CompanyCode,
                       OrganizationCode = string.Empty,
                       UserName = AppSettings.ImpersonateUserName,
                       UserCode = loginId,
                       LoginId = loginId,
                   };
        }

        /// <summary>
        /// Identity 정보를 생성하여 반환합니다.
        /// </summary>
        /// <param name="loginId">로그인Id</param>
        /// <returns>IAccessIdentity</returns>
        public abstract IAccessIdentity CreateIdentity(string loginId);

        /// <summary>
        /// Principal 정보를 생성하여 반환합니다.
        /// </summary>
        /// <param name="identity">IIdentity</param>
        /// <returns>IPrincipal</returns>
        public virtual IPrincipal CreatePrincipal(IIdentity identity)
        {
            return new AccessPrincipal(identity);
        }

        /// <summary>
        /// 로그인 사용자의 정보를 저장한다.
        /// </summary>
        /// <param name="loginId"></param>
        protected abstract void SetAuthData(string loginId);

        /// <summary>
        /// 저장한 로그인 사용자 정보를 Clear합니다.
        /// </summary>
        protected abstract void ClearAuthData();

        /// <summary>
        /// 사용자 인증정보인 Identity에 사용자 객체 정보를 채운다.
        /// </summary>
        protected virtual void SetIdentity(IAccessIdentity rwIdentity)
        {
            WebAppContext.Current.SetCurrent(rwIdentity);

            WebAppTool.SetValue(ApplicationKey, rwIdentity, DateTime.MinValue, string.Empty);
        }

        /// <summary>
        /// 사용자 인증정보인 Identity에 사용자 객체 정보를 지운다.
        /// </summary>
        protected virtual void ClearIdentity()
        {
            if(WebTool.IsWebContext)
            {
                HttpContext.Current.User = null;
                WebAppTool.DeleteValue(ApplicationKey);
            }
            else
            {
                Thread.CurrentPrincipal = null;
                Local.Data[ApplicationKey] = null;
            }
        }

        /// <summary>
        /// 로그인정보를 작성한다.
        /// </summary>
        protected virtual void WriteLoginInfo()
        {
            try
            {
                if(log.IsDebugEnabled)
                    log.Debug("로그인이력 정보를 작성을 시작합니다... Identity:[{0}]", Identity);

                // Write login log


                if(log.IsDebugEnabled)
                    log.Debug("로그인이력 정보를 작성을 완료합니다. Identity:[{0}]", Identity);
            }
            catch(Exception ex)
            {
                if(log.IsWarnEnabled)
                    log.WarnException(string.Format("로그인이력 정보 작성중 오류가 발생하였습니다. Identity:[{0}]", Identity), ex);
            }
        }
    }
}