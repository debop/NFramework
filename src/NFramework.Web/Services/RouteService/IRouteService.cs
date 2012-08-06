using System.Web.Routing;

namespace NSoft.NFramework.Web.Services.RouteService
{
    /// <summary>
    /// RouteService
    /// </summary>
    public interface IRouteService
    {
        /// <summary>
        /// Routing Route 정보
        /// </summary>
        RouteCollection Routes { get; }

        /// <summary>
        /// name 에 대한 Route VirtualPathData를 반환합니다.
        /// </summary>
        /// <param name="name">이름</param>
        /// <returns>VirtualPathData</returns>
        VirtualPathData GetvirtualPathData(string name);

        /// <summary>
        /// name 에 대한 Route VirtualPath를 반환합니다.
        /// </summary>
        /// <param name="name">이름</param>
        /// <returns>가상경로</returns>
        string GetVirtualPath(string name);
    }
}