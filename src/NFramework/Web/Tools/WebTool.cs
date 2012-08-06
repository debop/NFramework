using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;
using System.Web.Configuration;
using System.Web.Hosting;
using System.Web.UI;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Web.Tools {
    /// <summary>
    /// 
    /// </summary>
    public static partial class WebTool {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// Script Path 구분자 ('/')
        /// </summary>
        public const char ScriptSeparatorChar = '/';

        /// <summary>
        /// 이미 압축이 되어 있는 파일을 압축할 필요 없다.
        /// </summary>
        public static string[] NoCompressionFileExtensions = new[]
                                                             {
                                                                 ".swf",
                                                                 ".zip",
                                                                 ".cab",
                                                                 ".7z",
                                                                 ".alz",
                                                                 ".egg",
                                                                 ".tar",
                                                                 ".jpg",
                                                                 ".gif",
                                                                 ".png",
                                                                 ".ico"
                                                             };

        /// <summary>
        /// 웹 Application Context가 존재하는가?
        /// </summary>
        public static bool IsWebContext {
            get { return (HttpContext.Current != null); }
        }

        /// <summary>
        /// Web Application 서버 이름 (ex: www.realweb21.com, localhost)
        /// </summary>
        public static string HostName {
            get {
                if(IsWebContext)
                    return HttpContext.Current.Request.Url.Host;

                return String.Empty;
            }
        }

        /// <summary>
        /// Web Application 의 Port Number (기본은 80)
        /// </summary>
        public static int Port {
            get {
                if(IsWebContext)
                    return HttpContext.Current.Request.Url.Port;

                return 80;
            }
        }

        /// <summary>
        /// 서버 이름 (ex: http://www.realweb21.com:8080)
        /// </summary>
        public static string ServerName {
            get {
                if(IsWebContext) {
                    string serverName = HttpContext.Current.Request.Url.Scheme + "://" + HostName;

                    if(Port != 80)
                        serverName = serverName + ":" + Port;

                    return serverName;
                }

                return String.Empty;
            }
        }

        /// <summary>
        /// ASP.NET 응용 프로그램의 가상 응용 프로그램 루트 경로를 서버에서 가져옵니다. (예: /NFramework )
        /// </summary>
        public static string AppPath {
            get {
                return HostingEnvironment.ApplicationVirtualPath;
                //return IsWebContext ? HostingEnvironment.ApplicationVirtualPath : string.Empty;
                // return HttpContext.Current.Request.ApplicationPath;
            }
        }

        /// <summary>
        /// ASP.NET 응용 프로그램의 Root Path를 가져옵니다. (예: http://localhost:3500/NFramework )
        /// </summary>
        public static string ApplicationRootPath {
            get { return HttpContext.Current.GetApplicationRootPath(); }
        }

        /// <summary>
        /// 로그인 사용자 이름을 반환한다. User Identity가 없으면 Username은 빈문자열이다.
        /// </summary>
        public static string UserName {
            get {
                var identity = GetUserIdentity();
                return (identity != null) ? identity.Name : String.Empty;
            }
        }

        /// <summary>
        /// 로그인 사용자의 <see cref="IIdentity"/> 정보를 반환한다.
        /// </summary>
        /// <returns></returns>
        public static IIdentity GetUserIdentity() {
            if(IsWebContext && HttpContext.Current.User != null)
                return HttpContext.Current.User.Identity;

            return null;
        }

        /// <summary>
        /// HTML 문자열에서 HTML Tag를 제거하고, 순수 TEXT만 반환합니다.
        /// </summary>
        /// <param name="htmlString"></param>
        /// <returns></returns>
        public static string RemoveHtml(string htmlString) {
            if(htmlString.IsWhiteSpace())
                return htmlString;

            if(IsWebContext)
                return HttpContext.Current.Server.HtmlDecode(Regex.Replace(htmlString, "<(.|\n)*?>", String.Empty));

            try {
                using(var app = new HttpApplication())
                    return app.Server.HtmlDecode(Regex.Replace(htmlString, "<(.|\n)*?>", String.Empty));
            }
            catch {
                return htmlString;
            }
        }

        /// <summary>
        /// <see cref="HttpRequest.QueryString"/>의 모든 정보를 문자열로 반환한다.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string GetQueryString(this HttpRequest request) {
            var strs = new List<string>();

            foreach(string key in request.QueryString.Keys)
                strs.Add(key + "=" + request.QueryString[key].AsText());

            return String.Join("&", strs.ToArray());
        }

        /// <summary>
        /// web.config의 appSetting의 값을 읽어온다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T GetAppSettings<T>(string key, T defaultValue) {
            return ConfigTool.GetAppSettings(key, defaultValue);
        }

        /// <summary>
        /// 현재 호출자가 Web Context하에서 호출되면 Web Application 환경설정 중에 appSettings 의 정보를 가져오고,
        /// Application Context 하에서 호출되면 Application 환경 설정 중의 appSettings에서 정보를 가져온다.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <seealso cref="WebConfigurationManager"/>
        /// <seealso cref="ConfigurationManager"/>
        public static object GetAppSettings(string key) {
            key.ShouldNotBeWhiteSpace("key");

            return ConfigTool.GetAppSettings(key, String.Empty);
        }

        /// <summary>
        /// 환경설정에서 지정된 이름의 <see cref="ConnectionStringSettings"/> 인스턴스를 가져온다.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ConnectionStringSettings GetConnectionStringSettings(string name) {
            return ConfigTool.GetConnectionSettings(name);
        }

        /// <summary>
        /// 지정된 이름의 ConnectionString 정보를 가져온다.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetConnectionString(string name) {
            return ConfigTool.GetConnectionString(name);
        }

        /// <summary>
        /// 파일 경로를 상대적인 값으로 표현 했을 경우
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <returns></returns>
        public static string GetPhysicalPath(this string virtualPath) {
            if(virtualPath.IsWhiteSpace())
                return String.Empty;

            string path = virtualPath;
            string rootPath = AppDomain.CurrentDomain.BaseDirectory;

            if(virtualPath.IsWhiteSpace()) {
                path = rootPath;
            }
            else if(virtualPath.StartsWith("~")) {
                path = Path.Combine(rootPath, virtualPath.Substring(1).TrimStart(ScriptSeparatorChar, Path.DirectorySeparatorChar));
            }
            else if(virtualPath.StartsWith(".")) {
                path = Path.GetFullPath(virtualPath);
            }
            else if(virtualPath.StartsWith("/")) {
                path = HttpContext.Current.Server.MapPath(virtualPath);
            }

            var physicalPath = path.Replace(ScriptSeparatorChar, Path.DirectorySeparatorChar);

            if(IsDebugEnabled)
                log.Debug("Virtual Path로부터 Physical Path를 구합니다. virtualPath=[{0}], physicalPath=[{1}]", virtualPath, physicalPath);

            return physicalPath;
        }

        /// <summary>
        /// Script 전체 경로를 반환한다.
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <returns></returns>
        /// <seealso cref="Control.ResolveUrl"/>
        public static string GetScriptPath(this string virtualPath) {
            if(IsDebugEnabled)
                log.Debug("Script 전체경로를 만듭니다. virtualPath=[{0}]", virtualPath);

            if(IsWebContext == false)
                return virtualPath;

            #region << VirtualPathUtility 가 너무 엄격함 >>

            // HINT: VirtualPathUtility class를 요즘 알았네...
            //var result = virtualPath;

            //if(VirtualPathUtility.IsAppRelative(virtualPath))
            //    result = VirtualPathUtility.Combine(AppPath, result.Substring(1));

            #endregion

            var path = ServerName + AppPath;

            if(IsDebugEnabled)
                log.Debug("가상경로로부터 전체경로를 찾습니다. ServerName=[{0}], AppPath=[{1}], Path=[{2}]", ServerName, AppPath, path);

            path = virtualPath.StartsWith("~")
                       ? Path.Combine(path, virtualPath
                                                .Substring(1)
                                                .TrimStart(ScriptSeparatorChar, Path.DirectorySeparatorChar))
                       : virtualPath;

            var result = path.Replace(Path.DirectorySeparatorChar, ScriptSeparatorChar);

            if(IsDebugEnabled)
                log.Debug("virtualPath=[{0}], Full Script path=[{1}]", virtualPath, result);

            return result;
        }

        /// <summary>
        /// 현재 context 의 client가 GZip 압축을 지원하는지?
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static bool CanBrowserAcceptGzip(this HttpContext context) {
            context.ShouldNotBeNull("context");

            string acceptEncoding = context.Request.Headers["Accept-Encoding"];
            return acceptEncoding.IsNotWhiteSpace() && acceptEncoding.Contains("gzip");
        }

        /// <summary>
        /// 현재 context 의 client가 Deflate 압축을 지원하는지?
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static bool CanBrowserAcceptDeflate(this HttpContext context) {
            context.ShouldNotBeNull("context");

            string acceptEncoding = context.Request.Headers["Accept-Encoding"];
            return acceptEncoding.IsNotWhiteSpace() && acceptEncoding.Contains("deflate");
        }

        /// <summary>
        /// 정보를 Cache에 저장합니다.
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="slidingExpiration"></param>
        /// <returns></returns>
        public static bool SaveCacheItem(this Cache cache, string key, object value, TimeSpan slidingExpiration) {
            key.ShouldNotBeWhiteSpace("key");

            if(IsDebugEnabled)
                log.Debug("웹 캐시에 정보를 저장합니다. key=[{0}], slidingExpiration=[{1}]", key, slidingExpiration);

            cache.Insert(key, value, null, Cache.NoAbsoluteExpiration, slidingExpiration);
            return true;
        }

        /// <summary>
        /// 캐시에서 지정된 키의 값을 반환한다.
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="key"></param>
        /// <param name="defaultValueFactory"></param>
        /// <returns></returns>
        public static object LoadCacheItem(this Cache cache, string key, Func<object> defaultValueFactory) {
            key.ShouldNotBeWhiteSpace("key");

            if(IsDebugEnabled)
                log.Debug("웹 캐시에서 캐시된 값을 로드합니다. key=[{0}]", key);

            var result = cache.Get(key);

            if(result == null && defaultValueFactory != null)
                result = defaultValueFactory();

            return result;
        }

        /// <summary>
        /// 캐시에서 지정된 키의 값을 제거하여, 반환한다.
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static object RemoveCacheItem(this Cache cache, string key) {
            key.ShouldNotBeWhiteSpace("key");

            return cache.Remove(key);
        }

        /// <summary>
        /// <paramref name="context"/>의 Uri Sheme을 추출합니다. 기본은 http 입니다.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetUrlScheme(this HttpContext context) {
            return (context != null) ? context.Request.Url.Scheme : "http";
        }

        /// <summary>
        /// <paramref name="context"/>의 Http Port 값을 추출합니다.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static int GetPort(this HttpContext context) {
            return context != null ? context.Request.Url.Port : 80;
        }

        /// <summary>
        ///  Web Application 서버 이름 (ex: www.realweb21.com, localhost)
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetHostName(this HttpContext context) {
            return (context != null) ? context.Request.Url.Host : "localhost";
        }

        /// <summary>
        /// 서버 이름 (예: http://www.realweb21.com:8080)
        /// </summary>
        public static string GetServerName(this HttpContext context) {
            if(context != null) {
                string serverName = string.Concat(context.GetUrlScheme(), "://", context.GetHostName());

                var port = context.GetPort();
                if(port != 80)
                    serverName = string.Concat(serverName, ":", port);

                return serverName;
            }

            return string.Empty;
        }

        /// <summary>
        /// ASP.NET 응용 프로그램의 가상 응용 프로그램 루트 경로를 서버에서 가져옵니다. (예: /NFramework )
        /// </summary>
        public static string GetApplicationVirtualPath(this HttpContext context) {
            return HostingEnvironment.ApplicationVirtualPath;
        }

        /// <summary>
        /// ASP.NET 응용 프로그램의 Root Path를 가져옵니다. (예: http://localhost:3500/NFramework )
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetApplicationRootPath(this HttpContext context) {
            return (context != null)
                       ? context.GetServerName() + context.GetApplicationVirtualPath()
                       : context.GetApplicationVirtualPath();
        }

        /// <summary>
        /// 응답 정보를 <paramref name="context"/>에 씁니다. <paramref name="responseSettings"/>을 통해 <see cref="HttpResponse"/>의 부가 설정을 수행합니다.
        /// </summary>
        public static void WriteResponse(this HttpContext context, byte[] responseBytes, Action<HttpResponse> responseSettings = null) {
            if(IsDebugEnabled)
                log.Debug("응답정보를 씁니다...");

            var response = context.Response;

            if(response.IsClientConnected == false)
                return;

            response.Initialize();

            if(responseSettings != null)
                responseSettings(response);

            response.OutputStream.Write(responseBytes, 0, responseBytes.Length);
        }

        /// <summary>
        /// 응답 정보를 <paramref name="context"/>에 씁니다. <paramref name="responseSettings"/>을 통해 <see cref="HttpResponse"/>의 부가 설정을 수행합니다.
        /// </summary>
        public static void WriteResponse(this HttpContext context, string responseText, Action<HttpResponse> responseSettings = null) {
            if(IsDebugEnabled)
                log.Debug("응답정보를 씁니다...");

            var response = context.Response;

            if(response.IsClientConnected == false)
                return;

            response.Initialize();

            if(responseSettings != null)
                responseSettings(response);

            response.Write(responseText);
        }

        /// <summary>
        /// response를 초기화합니다.
        /// </summary>
        /// <param name="response"></param>
        private static void Initialize(this HttpResponse response) {
            response.Clear();
            response.Buffer = true;
            response.Expires = -1;
        }
    }
}