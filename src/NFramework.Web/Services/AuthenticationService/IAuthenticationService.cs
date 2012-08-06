using System.Security.Principal;
using NSoft.NFramework.Web.Access;

namespace NSoft.NFramework.Web.Services.AuthenticationService
{
    /// <summary>
    /// 인증서비스
    /// </summary>
    public interface IAuthenticationService
    {
        /// <summary>
        /// 접속자 정보
        /// </summary>
        IAccessIdentity Identity { get; }

        /// <summary>
        /// 접속자의 Id
        /// </summary>
        string LoginUserName { get; }

        /// <summary>
        /// 로그인 Url
        /// </summary>
        string LoginUrl { get; }

        /// <summary>
        /// 기본 Url
        /// </summary>
        string DefaultUrl { get; }

        /// <summary>
        /// 사용자 검증을 수행합니다.
        /// </summary>
        /// <param name="userId">로그인Id</param>
        /// <param name="password">비밀번호</param>
        /// <returns>검증여부</returns>
        bool VerifyUser(string userId, string password);

        /// <summary>
        /// 로그인 처리
        /// </summary>
        /// <param name="userName">로그인Id</param>
        /// <param name="password">비밀번호</param>
        /// <param name="redirectDefault">기본페이지로 이동</param>
        void Login(string userName = null, string password = null, bool redirectDefault = true);

        /// <summary>
        /// Identity 정보를 생성하여 반환합니다.
        /// </summary>
        /// <param name="loginId">로그인Id</param>
        /// <returns>IAccessIdentity</returns>
        IAccessIdentity CreateIdentity(string loginId);

        /// <summary>
        /// Principal 정보를 생성하여 반환합니다.
        /// </summary>
        /// <param name="identity">IIdentity</param>
        /// <returns>IPrincipal</returns>
        IPrincipal CreatePrincipal(IIdentity identity);

        /// <summary>
        /// 로그아웃한다.
        /// </summary>
        /// <param name="redirectLogin">로그인페이지로 이동</param>
        /// <param name="endReponse">프로세스 종료</param>
        void Logout(bool redirectLogin = true, bool endReponse = true);

        /// <summary>
        /// 로그인페이지로 이동
        /// </summary>
        /// <param name="endReponse">프로세스 종료</param>
        void RedirectToLoginPage(bool endReponse);
    }
}