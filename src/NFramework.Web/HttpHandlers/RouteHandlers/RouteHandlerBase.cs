using System;
using System.Security;
using System.Web;
using System.Web.Compilation;
using System.Web.Routing;
using System.Web.Security;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Web.HttpHandlers.RouteHandlers
{
    /// <summary>
    /// <see cref="IRouteHandler"/>를 구현한 기본 추상 클래스입니다.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class RouteHandlerBase<T> : IRouteHandler where T : IHttpHandler
    {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 처리할 Handler 경로
        /// </summary>
        public abstract string GetVirtualPath();

        /// <summary>
        /// 접근여부 체크여부
        /// </summary>
        public virtual bool CheckPhysicalUrlAccess { get; set; }

        /// <summary>
        /// 해당 RequestContext에 대한 처리기 반환
        /// </summary>
        /// <param name="requestContext"></param>
        /// <returns></returns>
        public virtual IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            if(IsDebugEnabled)
                log.Debug("요청에 따른 HttpHandler를 선택합니다... requestContext=[{0}]", requestContext);

            var virtualPath = GetVirtualPath();

            if(virtualPath.IsWhiteSpace())
                throw new InvalidOperationException("처리할 핸들러 가상경로가 없습니다.");

            foreach(var urlParm in requestContext.RouteData.Values)
                requestContext.HttpContext.Items[urlParm.Key] = urlParm.Value;

            if(CheckPhysicalUrlAccess &&
               !UrlAuthorizationModule.CheckUrlAccessForPrincipal(virtualPath,
                                                                  requestContext.HttpContext.User,
                                                                  requestContext.HttpContext.Request.HttpMethod))
                throw new SecurityException();

            var page = BuildManager.CreateInstanceFromVirtualPath(virtualPath, typeof(T)) as IHttpHandler;

            if(IsDebugEnabled)
                log.Debug("HttpHandler를 구했습니다. virtualPath=[{0}], page=[{1}]", virtualPath, page);

            return page;
        }
    }
}