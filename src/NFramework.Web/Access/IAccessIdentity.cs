using System;
using System.Security.Principal;

namespace NSoft.NFramework.Web.Access
{
    /// <summary>
    /// 사용자 계정 정보를 나타내는 인터페이스입니다.
    /// </summary>
    public interface IAccessIdentity : IIdentity, IEquatable<IAccessIdentity>
    {
        /// <summary>
        /// 인증 사용자의 회사 Code
        /// </summary>
        string CompanyCode { get; set; }

        /// <summary>
        /// 인증 사용자의 부서 Code
        /// </summary>
        string OrganizationCode { get; set; }

        /// <summary>
        /// 인증 사용자의 사용자 Code
        /// </summary>
        string UserCode { get; set; }

        /// <summary>
        /// 인증 사용자 이름 (Localized)
        /// </summary>
        string UserName { get; set; }

        /// <summary>
        /// 인증 사용자의 로그인 Id
        /// </summary>
        string LoginId { get; set; }

        /// <summary>
        /// 인증 사용자 LocaleKey
        /// </summary>
        string LocaleKey { get; set; }

        /// <summary>
        /// 사용자 테마
        /// </summary>
        string Theme { get; set; }
    }
}