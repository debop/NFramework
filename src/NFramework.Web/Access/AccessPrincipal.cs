using System.Security.Principal;

namespace NSoft.NFramework.Web.Access
{
    /// <summary>
    /// 사용자 인증 정보를 나타내는 Principal 클래스입니다.
    /// </summary>
    /// <remarks>
    /// NOTE: Raise exception in ASP.NET Development Server 
    /// <see cref="https://connect.microsoft.com/VisualStudio/feedback/details/302478/connection-get-localip-method-raise-exception-in-asp-net-development-server"/>
    /// </remarks>
    public class AccessPrincipal : ValueObjectBase, IPrincipal
    {
        public AccessPrincipal(IIdentity identity)
        {
            identity.ShouldNotBeNull("identity");
            Identity = identity;
        }

        /// <summary>
        /// 현재 Principal이 지정된 역할에 속하는지 여부를 확인합니다.
        /// </summary>
        /// <returns>
        /// 현재 Principal이 지정된 역할의 멤버이면 true이고, 그렇지 않으면 false입니다.
        /// </returns>
        /// <param name="role">멤버 여부를 확인하기 위한 역할의 이름입니다. </param>
        public bool IsInRole(string role)
        {
            role.ShouldNotBeWhiteSpace("role");
            return WebAppContext.Services.RoleService.IsInRole(role, (IAccessIdentity) Identity);
        }

        /// <summary>
        /// 현재 보안 주체(principal)의 ID를 가져옵니다.
        /// </summary>
        /// <returns>
        /// 현재 Principal과 관련된 <see cref="T:System.Security.Principal.IIdentity"/> 개체입니다.
        /// </returns>
        public IIdentity Identity { get; set; }

        public override int GetHashCode()
        {
            return HashTool.Compute(Identity);
        }

        public override string ToString()
        {
            return string.Format("AccessPrincipal# Identity=[{0}]", Identity);
        }
    }
}