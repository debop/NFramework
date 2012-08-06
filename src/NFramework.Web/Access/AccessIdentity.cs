using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Web.Access
{
    /// <summary>
    /// 사용자 계정에 대한 인증정보를 표현한 클래스입니다.
    /// </summary>
    public class AccessIdentity : ValueObjectBase, IAccessIdentity
    {
        /// <summary>
        /// 현재 사용자의 이름을 가져옵니다.
        /// </summary>
        /// <returns>
        /// 코드를 실행하고 있는 사용자의 이름입니다.
        /// </returns>
        public string Name
        {
            get { return LoginId; }
        }

        /// <summary>
        /// 사용한 인증 형식을 가져옵니다.
        /// </summary>
        /// <returns>
        /// 사용자를 식별하는 데 사용되는 인증 형식입니다.
        /// </returns>
        public string AuthenticationType
        {
            get { return "Custom"; }
        }

        /// <summary>
        /// 사용자가 인증되었는지 여부를 나타내는 값을 가져옵니다.
        /// </summary>
        /// <returns>
        /// 사용자가 인증되었으면 true이고, 그렇지 않으면 false입니다.
        /// </returns>
        public bool IsAuthenticated
        {
            get { return StringTool.IsNotWhiteSpace(LoginId); }
        }

        /// <summary>
        /// 인증 사용자의 회사 Code
        /// </summary>
        public string CompanyCode { get; set; }

        /// <summary>
        /// 인증 사용자의 부서 Code
        /// </summary>
        public string OrganizationCode { get; set; }

        /// <summary>
        /// 인증 사용자의 사용자 Code
        /// </summary>
        public string UserCode { get; set; }

        /// <summary>
        /// 인증 사용자 이름 (Localized)
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 인증 사용자의 로그인 Id
        /// </summary>
        public string LoginId { get; set; }

        /// <summary>
        /// 인증 사용자 LocaleKey
        /// </summary>
        public string LocaleKey { get; set; }

        /// <summary>
        /// 사용자 테마
        /// </summary>
        public string Theme { get; set; }

        public bool Equals(IAccessIdentity other)
        {
            return (other != null) && GetHashCode().Equals(other.GetHashCode());
        }

        public override int GetHashCode()
        {
            return HashTool.Compute(LoginId, UserCode, OrganizationCode, CompanyCode);
        }

        public override string ToString()
        {
            return string.Format("AccessIdentity# LoginId=[{0}], UserCode=[{1}], OrganizationCode=[{2}], CompanyCode=[{3}]",
                                 LoginId, UserCode, OrganizationCode, CompanyCode);
        }
    }
}