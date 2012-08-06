using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;

namespace NSoft.NFramework.Web.HttpApplications {
    /// <summary>
    /// Lazy Pattern을 이용하여, 웹 Application 시작 시에 미리 준비해야 할 작업이 있다면, 그 작업을 비동기적으로 실행시켜줍니다.
    /// 이렇게 하면, 실제 데이터를 필요로 할 때, 미리 준비된 데이터를 사용할 수 있으므로, 성능상에 유리하게 됩니다.
    /// </summary>
    /// <remarks>
    /// Application_Start, Session_Start, Session_End, Application_End 시점에 비동기적으로 실행할 메소드를 재정의하면, 
    /// 각 싯점마다, 비동기적으로 실행해줍니다.
    /// </remarks>
    public abstract class AsyncHttpApplicationBase : HttpApplication {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private static readonly string _applicationVirtualPath = HostingEnvironment.ApplicationVirtualPath ?? string.Empty;
        private static readonly object _syncLock = new object();

        protected AsyncHttpApplicationBase() {
            // NOTE: HttpApplication 이벤트 핸들러 등록은 모든 인스턴스 생성 시에 해줘야 합니다.
            //
            BeginRequest += OnBeginRequest;
            PreRequestHandlerExecute += OnPreRequestHandlerExecute;
            PostRequestHandlerExecute += OnPostRequestHandlerExecute;
            EndRequest += OnEndRequest;
        }

        /// <summary>
        /// HttpApplication Instance 중 첫번째 인스턴스만 호출되는 이벤트입니다.
        /// </summary>
        public virtual void Application_Start(object sender, EventArgs e) {
            if(log.IsInfoEnabled)
                log.Info("웹 응용프로그램을 시작합니다... ApplicationPath=[{0}]", _applicationVirtualPath);

            Task.Factory.StartNew(ctx => ApplicationStartAfter((HttpContext)ctx),
                                  HttpContext.Current,
                                  TaskCreationOptions.LongRunning);
        }

        public override void Init() {
            base.Init();

            if(log.IsInfoEnabled)
                log.Info("웹 응용프로그램을 초기화(Init) 합니다... ApplicationPath=[{0}]", _applicationVirtualPath);

            Task.Factory.StartNew(ctx => ApplicationInitAfter((HttpContext)ctx),
                                  HttpContext.Current,
                                  TaskCreationOptions.LongRunning);
        }

        /// <summary>
        /// HttpApplication에서 예외가 발생했을 때 호출되는 이벤트 핸들러입니다. 기본적으로는 예외에 대한 내용을 로그로 작성하는 작업을 수행합니다)
        /// </summary>
        public virtual void Application_Error(object sender, EventArgs e) {
            //if(Server == null)
            //    return;
            // NOTE : 예외의 InnerException을 찾아 예외를 일으킨 실제 원본 객체를 찾는다.
            var ex = Server.GetLastError();

            if(ex == null)
                return;

            while(ex.InnerException != null)
                ex = ex.InnerException;

            var context = HttpContext.Current;

            With.TryActionAsync(() =>
                                Task.Factory
                                    .StartNew(() => ApplicationErrorAfter(context, ex), TaskCreationOptions.PreferFairness)
                                    .ContinueWith(_ => Server.ClearError())
                                    .Wait());
        }

        /// <summary>
        /// Session이 시작했을 때 발생하는 이벤트입니다.
        /// </summary>
        public virtual void Session_Start(object sender, EventArgs e) {
            if(IsDebugEnabled)
                log.Debug("새로운 Session이 시작했습니다.");

            Task.Factory.StartNew(ctx => SessionStartAfter((HttpContext)ctx),
                                  HttpContext.Current,
                                  TaskCreationOptions.LongRunning);
        }

        /// <summary>
        /// Timeout에 의해, 혹은 사용자의 로그아웃에 의해 Session이 끝났을 때 발생하는 이벤트입니다.
        /// </summary>
        public virtual void Session_End(object sender, EventArgs e) {
            if(IsDebugEnabled)
                log.Debug("Session이 종료되었습니다.");

            Task.Factory.StartNew(ctx => SessionEndAfter((HttpContext)ctx),
                                  HttpContext.Current,
                                  TaskCreationOptions.LongRunning);
        }

        /// <summary>
        /// HttpApplication 인스턴스들이 Dispose 될 때마다 호출된다.
        /// </summary>
        public virtual void Application_Disposed(object sender, EventArgs e) {
            Task.Factory
                .StartNew(ctx => ApplicationDisposedAfter((HttpContext)ctx),
                          HttpContext.Current,
                          TaskCreationOptions.None)
                .Wait();

            if(log.IsInfoEnabled)
                log.Info("HttpApplication 인스턴스가 Dispose 되었습니다. Path=[{0}]", _applicationVirtualPath);
        }

        /// <summary>
        /// 마지막 HttpApplication 인스턴스가 Dispose 될 때 호출된다.
        /// </summary>
        public virtual void Application_End(object sender, EventArgs e) {
            Task.Factory
                .StartNew(ctx => ApplicationEndAfter((HttpContext)ctx),
                          HttpContext.Current,
                          TaskCreationOptions.None)
                .Wait();


            if(log.IsInfoEnabled)
                log.Info("HttpApplication 이 종료되었습니다. Path=[{0}]", _applicationVirtualPath);
        }

        /// <summary>
        /// Client 요청 처리 시작 이벤트입니다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnBeginRequest(object sender, EventArgs e) {}

        /// <summary>
        /// Client 요청 처리 사전 작업의 마지막 단계입니다. 이 이벤트 처리기 이후에는 바로 Page 관련 작업을 수행합니다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnPreRequestHandlerExecute(object sender, EventArgs e) {}

        /// <summary>
        /// Page 작업이 완료된 후 바로 수행되는 단계입니다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnPostRequestHandlerExecute(object sender, EventArgs e) {}

        /// <summary>
        /// 모든 요청을 처리하고, 마무리하는 단계입니다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEndRequest(object sender, EventArgs e) {}

        /// <summary>
        /// Application_Start 시에 실행할 비동기 작업의 본체입니다.
        /// </summary>
        protected virtual void ApplicationStartAfter(HttpContext context) {}

        /// <summary>
        /// Application_Init 시에 실행할 비동기 작업의 본체입니다.
        /// </summary>
        protected virtual void ApplicationInitAfter(HttpContext context) {}

        /// <summary>
        /// Application_Error 시에 비동기적으로 실행할 작업입니다. (기본적으로는 로그에 쓰는 작업을 합니다)
        /// overriding 시 base method를 호출해 주셔야합니다.
        /// </summary>
        /// <param name="context">현재 Context 정보</param>
        /// <param name="exception">Web Applicatoin 예외 정보</param>
        protected virtual void ApplicationErrorAfter(HttpContext context, Exception exception) {
            if(context == null)
                return;
            if(exception == null)
                return;

            try {
                if(exception is HttpException && ((HttpException)exception).GetHttpCode() == 404) {
                    // NOTE: 아무것도 하지 말자. 개발자가 알아서 하겠지...
                    //if(IsDebugEnabled)
                    //    log.Debug("A 404 occurred (page not found) - " + ex.Message, ex);
                }
                else if(exception is System.Threading.ThreadAbortException || exception is System.Threading.ThreadInterruptedException) {
                    if(log.IsInfoEnabled) {
                        log.Info("Response.End() 호출에 따른 ThreadAbortException or ThreadInterruptedException이 발생했습니다. 무시하셔도 됩니다.");
                        log.Info(exception);
                    }
                }
                else if(log.IsErrorEnabled) {
                    log.Error("시스템에 예외가 발생했습니다.");
                    log.Error(exception);
                }
            }
            catch(Exception err) {
                if(log.IsWarnEnabled) {
                    log.Warn("HttpApplication OnError 이벤트 처리시에 예외가 발생했습니다. 무시하셔도 됩니다.");
                    log.Warn(err);
                }
            }
        }

        /// <summary>
        /// Session_Start 시에 비동기적으로 실행할 작업입니다.
        /// </summary>
        protected virtual void SessionStartAfter(HttpContext context) {}

        /// <summary>
        /// Session_End 시에 비동기적으로 실행할 작업입니다.
        /// </summary>
        protected virtual void SessionEndAfter(HttpContext context) {}

        /// <summary>
        /// Application_Disposed 시에 비동기적으로 실행할 작업입니다. 
        /// </summary>
        protected virtual void ApplicationDisposedAfter(HttpContext context) {}

        /// <summary>
        /// Application_End 시에 비동기적으로 실행할 작업입니다.
        /// </summary>
        protected virtual void ApplicationEndAfter(HttpContext context) {}
    }
}