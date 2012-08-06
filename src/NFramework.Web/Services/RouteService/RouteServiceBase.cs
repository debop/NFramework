using System;
using System.Web.Routing;

namespace NSoft.NFramework.Web.Services.RouteService
{
    /// <summary>
    /// RouteServiceBase
    /// </summary>
    public abstract class RouteServiceBase : IRouteService
    {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion       

        /// <summary>
        /// Routing Route 정보
        /// </summary>
        public RouteCollection Routes
        {
            get { return RouteTable.Routes; }
        }

        /// <summary>
        /// <paramref name="name"/> 에 대한 Route VirtualPathData를 반환합니다.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public VirtualPathData GetvirtualPathData(string name)
        {
            if(IsDebugEnabled)
                log.Debug("지정한 name에 대한 Route VirtualPathData를 반환합니다... name=[{0}]", name);

            return RouteTable.Routes.GetVirtualPath(null, name, new RouteValueDictionary());
        }

        /// <summary>
        /// name 에 대한 Route VirtualPath를 반환합니다.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetVirtualPath(string name)
        {
            var virtualPathData = GetvirtualPathData(name);
            return virtualPathData != null ? virtualPathData.VirtualPath : string.Empty;
        }
    }
}