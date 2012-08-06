using NSoft.NFramework.Web.Access;

namespace NSoft.NFramework.Web.Services.RoleService
{
    /// <summary>
    /// Role에 대한 서비스를 제공합니다.
    /// </summary>
    public interface IRoleService
    {
        /// <summary>
        /// 사용자의 모든 Role정보를 반환합니다.
        /// </summary>
        /// <param name="identity">요청자 정보</param>
        /// <returns>Role 목록</returns>
        string[] GetRoles(IAccessIdentity identity);

        /// <summary>
        /// 사용자의 해당 Role 소속여부를 판단합니다.
        /// </summary>
        /// <param name="roleName">Role Name</param>
        /// <param name="identity">요청자 정보</param>
        /// <returns>Role 소속 여부</returns>
        bool IsInRole(string roleName, IAccessIdentity identity);
    }
}