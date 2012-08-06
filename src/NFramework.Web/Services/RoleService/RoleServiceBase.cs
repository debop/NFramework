using NSoft.NFramework.Web.Access;

namespace NSoft.NFramework.Web.Services.RoleService
{
    /// <summary>
    /// RoleServiceBase
    /// </summary>
    public abstract class RoleServiceBase : IRoleService
    {
        #region Implementation of IRoleService

        /// <summary>
        /// 사용자의 모든 Role정보를 반환합니다.
        /// </summary>
        /// <param name="identity">요청자 정보</param>
        /// <returns>Role 목록</returns>
        public abstract string[] GetRoles(IAccessIdentity identity);

        /// <summary>
        /// 사용자의 해당 Role 소속여부를 판단합니다.
        /// </summary>
        /// <param name="roleName">Role Name</param>
        /// <param name="identity">요청자 정보</param>
        /// <returns>Role 소속 여부</returns>
        public abstract bool IsInRole(string roleName, IAccessIdentity identity);

        #endregion
    }
}