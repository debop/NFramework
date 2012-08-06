using System.Web;
using System.Web.Routing;

namespace NSoft.NFramework.Web.HttpHandlers.RouteHandlers
{
    /// <summary>
    /// 기본 <see cref="IRouteHandler"/> 구현 클래스입니다.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RouteHandler<T> : RouteHandlerBase<T> where T : IHttpHandler
    {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="virtualPath">가상디렉터리</param>
        /// <param name="checkPhysicalUrlAccess">접근권한 체크여부</param>
        public RouteHandler(string virtualPath, bool checkPhysicalUrlAccess = true)
        {
            virtualPath.ShouldNotBeWhiteSpace("virtualPath");
            VirtualPath = virtualPath;
            CheckPhysicalUrlAccess = checkPhysicalUrlAccess;
        }

        /// <summary>
        /// 가상디렉터리
        /// </summary>
        public string VirtualPath { get; set; }

        /// <summary>
        /// 처리할 Handler 경로
        /// </summary>
        public override string GetVirtualPath()
        {
            return VirtualPath;
        }
    }
}