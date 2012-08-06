using NSoft.NFramework.InversionOfControl;
using NSoft.NFramework.Web.Routings;
using NSoft.NFramework.Web.Services.AuthenticationService;
using NSoft.NFramework.Web.Services.FileService;
using NSoft.NFramework.Web.Services.MenuService;
using NSoft.NFramework.Web.Services.RoleService;

namespace NSoft.NFramework.Web
{
    /// <summary>
    /// Current Thread Context에서 유지할 정보를 제공합니다.
    /// </summary>
    public static partial class WebAppContext
    {
        /// <summary>
        /// Business Logic Layer 의 서비스들입니다.
        /// </summary>
        public static class Services
        {
            /// <summary>
            /// 인증 서비스
            /// </summary>
            public static IAuthenticationService Authentication
            {
                get
                {
                    var svc = IoC.Resolve<IAuthenticationService>();

                    if(IsDebugEnabled)
                        log.Debug(@"Ioc에서 생성된 인증서비스를 반환합니다. IAuthenticationService=[{0}]", svc);

                    return svc;
                }
            }

            /// <summary>
            /// HttpFile관련 서비스
            /// </summary>
            public static IFileService FileService
            {
                get
                {
                    var svc = IoC.Resolve<IFileService>();

                    if(IsDebugEnabled)
                        log.Debug(@"Ioc에서 생성된 IFileService를 반환합니다. IWebFileService=[{0}]", svc);

                    return svc;
                }
            }

            /// <summary>
            /// Routing Service
            /// </summary>
            public static IRouteService RouteService
            {
                get
                {
                    var svc = IoC.Resolve<IRouteService>();

                    if(IsDebugEnabled)
                        log.Debug(@"Ioc에서 생성된 IRouteService를 반환합니다. IRouteService=[{0}]", svc);

                    return svc;
                }
            }

            /// <summary>
            /// 메뉴 서비스
            /// </summary>
            public static IMenuService MenuService
            {
                get
                {
                    var svc = IoC.Resolve<IMenuService>();

                    if(IsDebugEnabled)
                        log.Debug(@"Ioc에서 생성된 IMenuService를 반환합니다. IMenuService=[{0}]", svc);

                    return svc;
                }
            }

            /// <summary>
            /// Role 서비스
            /// </summary>
            public static IRoleService RoleService
            {
                get
                {
                    var svc = IoC.Resolve<IRoleService>();

                    if(IsDebugEnabled)
                        log.Debug(@"Ioc에서 생성된 IRoleService를 반환합니다. IRoleService=[{0}]", svc);

                    return svc;
                }
            }
        }
    }
}