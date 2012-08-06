using System;
using System.Diagnostics;
using System.Web;
using System.Web.Hosting;

namespace NSoft.NFramework.Web.HttpModules {
    /// <summary>
    /// ASP.NET Page 처리 성능을 알아보기 위해 log4net을 이용하여 성능 측정값을 로그에 쓴다.
    /// </summary>
    /// <remarks>
    /// 이 모듈을 사용하려면 log4net 을 사용해야하고, Log Level은 DEBUG 여야 합니다.<br/>
    /// 
    /// log4net layout conversion Pattern에 %property{page_duration} 을 추가하면 duration값이 나온다.
    /// PagePerformanceModule에 대해서만 다른 log appender를 사용하면, 특정 DB / TABLE에 따로 저장할 수 있다.
    /// </remarks>
    /// <example>
    /// IIS 7.0 클래식 모드와 IIS 7.0 이전 버전에 적용할 때
    /// <code>
    /// &lt;configuration&gt;
    ///   &lt;system.web&gt;
    ///		&lt;httpModule&gt;
    ///			&lt;add name="PagePerformanceModule" type="NSoft.NFramework.Web.HttpModules.PagePerformanceModule, NSoft.NFramework.Web"/&gt;
    ///		&lt;/httpModule&gt;
    ///   &lt;/system.web&gt;
    ///	&lt;/configuration&gt;
    /// </code>
    /// IIS 7.0 통합모드에 적용할 때
    /// <code>
    /// &lt;configuration&gt;
    ///   &lt;system.webServer&gt;
    ///		&lt;module&gt;
    ///			&lt;add name="PagePerformanceModule" type="NSoft.NFramework.Web.HttpModules.PagePerformanceModule, NSoft.NFramework.Web"/&gt;
    ///		&lt;/module&gt;
    ///   &lt;/system.webServer&gt;
    ///	&lt;/configuration&gt;
    /// </code>
    /// log4net layout conversionPattern
    /// <code>
    /// // AdoAppender 등 DB를 사용해서 성능 정보를 DB화 할 수 있습니다.
    /// &lt;param name="ConversionPattern" value="%property{pageUrl} %property{startTime} %property{duration}" /&gt;
    /// </code>
    /// </example>
    [Serializable]
    public class PagePerformanceModule : IHttpModule {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private static readonly object PagePerformanceStartKey = new object();

        /// <summary>
        /// 모듈을 초기화하고 요청을 처리할 수 있도록 준비합니다.<br/>
        /// </summary>
        /// <remarks>
        /// 요청처리를 위한 전처리/후처리는 <see cref="HttpApplication.BeginRequest"/>, <see cref="HttpApplication.EndRequest"/> 이벤트에서 하는 것이 아니라,
        /// <see cref="HttpApplication.PreRequestHandlerExecute"/>, <see cref="HttpApplication.PostRequestHandlerExecute"/>에서 처리하도록 합니다.<br/>
        /// 이는 ASP.NET Web Application의 실제 페이지 처리 직전, 직후의 Event가 PreRequestHandlerExecute, PostRequestHandlerExecute이기 때문이다.<br/>
        /// 참고 URL : ms-help://MS.MSDNQTR.v90.ko/fxref_system.web/html/aaf0c446-d27c-fe68-155e-0921c2357f02.htm"
        /// </remarks>
        /// <param name="context">
        /// ASP.NET 응용 프로그램 내의 모든 응용 프로그램 개체에 공통되는 메서드, 속성 및 이벤트에 액세스할 수 있도록 하는 <see cref="T:System.Web.HttpApplication" />입니다. 
        /// </param>
        /// <seealso cref="HttpApplication.BeginRequest"/>
        /// <seealso cref="HttpApplication.EndRequest"/>
        /// <seealso cref="HttpApplication.AcquireRequestState"/>
        public void Init(HttpApplication context) {
            if(log.IsInfoEnabled)
                log.Info("새로운 PagePerformanceModule 인스턴스를 초기화합니다.. Application:" + HostingEnvironment.ApplicationVirtualPath);

            // BeginRequest/EndRequest 사이에는 너무 많은 이벤트가 있다. 
            // 실제 Page 처리 바로 전/후 이벤트는 PreRequestHandlerExecute, PostRequestHandlerExecute이므로 성능 측정이 더 정확하다.
            //
            context.PreRequestHandlerExecute += OnPreRequestHandlerExecute;
            context.PostRequestHandlerExecute += OnPostRequestHandlerExecute;
        }

        /// <summary>
        /// <see cref="T:System.Web.IHttpModule" />을 구현하는 모듈에서 사용하는 리소스(메모리 제외)를 삭제합니다.
        /// </summary>
        public void Dispose() {
            if(log.IsInfoEnabled)
                log.Info("Dispose current instance of PagePerformanceModule. Application:" + HostingEnvironment.ApplicationVirtualPath);
        }

        /// <summary>
        /// ASP.NET에서 페이지 또는 XML Web services 같은 이벤트 처리기의 실행을 시작하기 바로 전에 발생하는 Event에 대한 Handler<br/>
        /// Page 처리 시작 시각을 기록합니다.
        /// </summary>
        public virtual void OnPreRequestHandlerExecute(object sender, EventArgs e) {
            Start();
        }

        /// <summary>
        /// ASP.NET 이벤트 처리기(예: 페이지 또는 XML Web services)가 실행을 완료하는 경우 발생하는 Event의 Handler<br/>
        /// 성능측정을 하고, 결과를 로그에 기록합니다. 로그 레벨은 DEBUG 입니다.
        /// </summary>
        /// <remarks>
        /// 로그 레벨은 DEBUG 입니다.
        /// 재정의를 통해 로그 형식등을 변경할 수 있습니다.
        /// </remarks>
        public virtual void OnPostRequestHandlerExecute(object sender, EventArgs e) {
            Stop();
        }

        /// <summary>
        /// Page 처리 시작 시각을 기록합니다.
        /// </summary>
        private static void Start() {
            // HttpContext.Current.Items[PagePerformanceStartKey] = DateTime.Now;

            var sw = new Stopwatch();
            sw.Start();
            Local.Data[PagePerformanceStartKey] = sw;
        }

        /// <summary>
        /// 성능측정을 하고, 결과를 로그에 기록합니다. 로그 레벨은 DEBUG 입니다.
        /// </summary>
        private static void Stop() {
            // chck performance
            var request = HttpContext.Current.Request;
            var sw = Local.Data[PagePerformanceStartKey] as Stopwatch;

            if(sw == null)
                return;

            sw.Stop();
            var duration = TimeSpan.FromMilliseconds(sw.ElapsedMilliseconds);
            var startRequest = DateTime.Now.Subtract(duration);
            SetLoggingProperties(duration, startRequest, request.RawUrl);

            //DateTime startRequest = (DateTime)HttpContext.Current.Items[PagePerformanceStartKey];
            //TimeSpan duration = DateTime.Now.Subtract(startRequest);
            // SetLoggingProperties(duration, startRequest, request.RawUrl);

            if(IsDebugEnabled)
                log.Debug("Processing {0} started at {1} took {2} msecs.", request.RawUrl, startRequest, duration.Milliseconds);

            // 성능 측정값을 리셋합니다. 
            ResetLoggingProperties();
        }

        /// <summary>
        /// log4net layout conversion Pattern에 %property{page_duration} 을 추가하면 duration값이 나온다.
        /// PagePerformanceModule에 대해서만 다른 log appender를 사용하면, 특정 DB / TABLE에 따로 저장할 수 있다.
        /// </summary>
        /// <param name="duration">실행 기간</param>
        /// <param name="startRequest">시작 시간</param>
        /// <param name="pageUrl">실행한 Page</param>
        private static void SetLoggingProperties(TimeSpan duration, DateTime startRequest, string pageUrl) {
#if LOG4NET
			log4net.ThreadContext.Properties["duration"] = duration.ToString();
			log4net.ThreadContext.Properties["startTime"] = startRequest;
			log4net.ThreadContext.Properties["pageUrl"] = pageUrl;
#endif
        }

        /// <summary>
        /// Reset current performance data for estimating next request.
        /// </summary>
        private static void ResetLoggingProperties() {
#if LOG4NET
			log4net.ThreadContext.Properties["duration"] = null;
			log4net.ThreadContext.Properties["startTime"] = null;
			log4net.ThreadContext.Properties["pageUrl"] = null;
#endif
        }
    }
}