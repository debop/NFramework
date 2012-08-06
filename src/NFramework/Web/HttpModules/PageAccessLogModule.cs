using System;
using System.Web;
using System.Web.Hosting;

namespace NSoft.NFramework.Web.HttpModules {
    /// <summary>
    /// ASP.NET 요청에 대한 접근 정보를 로깅합니다.
    /// </summary>
    /// 
    /// <remarks>
    /// 로깅관련 포맷 및 레벨을 환경설정에서 설정할 수 있도록 하면 좋겠다.
    /// </remarks>
    /// <example>
    /// <code lang="none">
    /// // IIS 7.0 클래식 모드와 IIS 7.0 이전 버전에 적용할 때
    /// <configuration>
    ///   <system.web>
    ///		<httpModule>
    ///			<add name="PageAccessLogModule" type="NSoft.NFramework.Web.HttpModules.PageAccessLogModule, NSoft.NFramework.Web"/>
    ///		</httpModule>
    ///   </system.web>
    ///	</configuration>
    /// </code>
    /// <code>
    /// // IIS 7.0 통합모드에 적용할 때
    /// <configuration>
    ///   <system.webServer>
    ///		<module>
    ///			<add name="PageAccessLogModule" type="NSoft.NFramework.Web.HttpModules.PageAccessLogModule, NSoft.NFramework.Web"/>
    ///		</module>
    ///   </system.webServer>
    ///	</configuration>
    /// </code>
    /// </example>
    [Serializable]
    public class PageAccessLogModule : IHttpModule {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 모듈을 초기화하고 요청을 처리할 수 있도록 준비합니다.<br/>
        /// 요청처리를 위한 전처리/후처리는 <see cref="HttpApplication.BeginRequest"/>, <see cref="HttpApplication.EndRequest"/> 이벤트에서 하는 것이 아니라,
        /// <see cref="HttpApplication.PreRequestHandlerExecute"/>, <see cref="HttpApplication.PostRequestHandlerExecute"/>에서 처리하도록 합니다.<br/>
        /// 이는 ASP.NET Web Application의 실제 페이지 처리 직전, 직후의 Event가 PreRequestHandlerExecute, PostRequestHandlerExecute이기 때문이다.<br/>
        /// 참고 URL : ms-help://MS.MSDNQTR.v90.ko/fxref_system.web/html/aaf0c446-d27c-fe68-155e-0921c2357f02.htm
        /// </summary>
        /// <param name="context">
        /// ASP.NET 응용 프로그램 내의 모든 응용 프로그램 개체에 공통되는 메서드, 속성 및 이벤트에 액세스할 수 있도록 하는 <see cref="T:System.Web.HttpApplication" />입니다. 
        /// </param>
        /// <seealso cref="HttpApplication.BeginRequest"/>
        /// <seealso cref="HttpApplication.EndRequest"/>
        /// <seealso cref="HttpApplication.AcquireRequestState"/>
        public void Init(HttpApplication context) {
            if(log.IsInfoEnabled)
                log.Info("PageAccessLogModule 인스턴스를 초기화합니다.. Application=[{0}]", HostingEnvironment.ApplicationVirtualPath);

            context.BeginRequest += OnBeginRequestHandler;
            context.EndRequest += OnEndRequestHandler;
        }

        /// <summary>
        /// <see cref="T:System.Web.IHttpModule" />을 구현하는 모듈에서 사용하는 리소스(메모리 제외)를 삭제합니다.
        /// </summary>
        public void Dispose() {
            if(log.IsInfoEnabled)
                log.Info("PageAccessLogModule 인스턴스를 종료합니다. Application=[{0}]", HostingEnvironment.ApplicationVirtualPath);
        }

        /// <summary>
        /// Log message format that used Before Request Executing
        /// </summary>
        protected const string PRE_REQUEST_LOG_FORMAT = @"[{0}]로부터 [{1}] 의 실행 요청을 받았습니다. 요청방식=[{2}]";

        /// <summary>
        /// Log message format that used After Request Executing
        /// </summary>
        protected const string POST_REQUEST_LOG_FORMAT = @"[{0}]로부터 [{1}] 의 실행 요청을 처리했습니다. 요청방식=[{2}], Status=[{3}";

        /// <summary>
        /// ASP.NET에서 페이지 또는 XML Web services 같은 이벤트 처리기의 실행을 시작하기 바로 전에 발생하는 Event에 대한 Handler<br/>
        /// Page 처리 요청정보를 로깅한다.
        /// </summary>
        /// <remarks>
        /// 로그 레벨은 DEBUG 입니다.<br/>
        /// 이 함수를 override하여, 원하는 형식으로, 원하는 저장소에 기록할 수 있다.
        /// </remarks>
        protected virtual void OnBeginRequestHandler(object sender, EventArgs e) {
            if(IsDebugEnabled) {
                var request = HttpContext.Current.Request;

                log.Debug(PRE_REQUEST_LOG_FORMAT,
                          request.UserHostAddress,
                          request.CurrentExecutionFilePath,
                          request.RequestType);
            }
        }

        /// <summary>
        /// ASP.NET 이벤트 처리기(예: 페이지 또는 XML Web services)가 실행을 완료하는 경우 발생하는 Event의 Handler<br/>
        /// Page 처리 결과를 로깅한다.
        /// </summary>
        /// <remarks>
        /// 로그 레벨은 DEBUG 입니다.<br/>
        /// 이 함수를 override하여, 원하는 형식으로, 원하는 저장소에 기록할 수 있다.
        /// </remarks>
        protected virtual void OnEndRequestHandler(object sender, EventArgs e) {
            if(IsDebugEnabled) {
                var request = HttpContext.Current.Request;
                var response = HttpContext.Current.Response;

                log.Debug(POST_REQUEST_LOG_FORMAT,
                          request.UserHostAddress,
                          request.CurrentExecutionFilePath,
                          request.RequestType,
                          response.Status);
            }
        }
    }
}