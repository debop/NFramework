using System.Collections.Generic;
using NSoft.NFramework.Web.Access;

namespace NSoft.NFramework.Web.Services.MenuService
{
    /// <summary>
    /// Menu에 대한 서비스를 제공합니다.
    /// </summary>
    public interface IMenuService
    {
        /// <summary>
        /// 메뉴중 사용자가 권한이 있는 루트메뉴목록
        /// </summary>
        /// <param name="productId">제품Id</param>
        /// <param name="identity">요청자 정보</param>
        /// <returns>메뉴목록</returns>
        IEnumerable<MenuItem> GetRootMenu(string productId, IAccessIdentity identity);

        /// <summary>
        /// 메뉴중 사용자가 권한이 있는 메뉴목록
        /// </summary>
        /// <param name="productId">제품Id</param>
        /// <param name="identity">요청자 정보</param>
        /// <returns>메뉴목록</returns>
        IEnumerable<MenuItem> FindAllMenuByProduct(string productId, IAccessIdentity identity);

        /// <summary>
        /// 메뉴정보를 반환한다.
        /// </summary>
        /// <param name="productId">제품Id</param>
        /// <param name="identity">요청자 정보</param>
        /// <param name="menuId">메뉴Id</param>
        /// <returns>메뉴</returns>
        MenuItem FindOneMenu(string productId, IAccessIdentity identity, string menuId);
    }
}