using System;
using System.Threading.Tasks;
using System.Web;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Installer;
using NSoft.NFramework.InversionOfControl;

namespace NSoft.NFramework.Web.HttpApplications {
    /// <summary>
    /// Castle.Windsor의 Container를 제공하는 HttpApplication 입니다.<br/>
    /// IoC와 관련한 초기화를 수행해 준다.
    /// Web Application의 global.asax에서 Inherits="NSoft.NFramework.Web.IocHttpApplication" 를 추가하면 됩니다.
    /// </summary>
    /// <example>
    /// <code>
    ///	// globals.asax를 다음과 같이 정의하면, IoC와 관련된 처리를 자동으로 수행해준다.
    /// <%@ Application Language="C#" Inherits="NSoft.NFramework.Web.IocHttpApplication" %>
    /// <script RunAt="server">
    /// 
    /// 	// 실제 처리는 IocHttpApplication에서 수행한다.
    /// 	protected override void Application_Start(object sender, EventArgs e)
    /// 	{
    /// 		// 제일 먼저 해줘야 로그를 사용할 수 있다.
    /// 		log4net.Config.XmlConfigurator.Configure();
    /// 	}
    /// 	
    /// 	protected override void Application_End(object sender, EventArgs e)
    /// 	{
    /// 		log4net.LogManager.Shutdown();	  
    /// 	}
    /// </script>
    /// </code>
    /// </example>
    /// <seealso cref="HttpApplication.BeginRequest"/>
    /// <seealso cref="HttpApplication.EndRequest"/>
    public class WindsorHttpApplication : HttpApplication, IContainerAccessor {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;
        private static readonly bool IsInfoEnabled = log.IsInfoEnabled;

        #endregion

        private static readonly string _applicationVirtualPath = string.Empty;
                                       //HostingEnvironment.ApplicationVirtualPath ?? string.Empty;

        private static readonly object _syncLock = new object();

        /// <summary>
        /// Initialize a new instance of IocHttpApplication.
        /// </summary>
        public WindsorHttpApplication() {
            if(IsInfoEnabled)
                log.Info("WindsorHttpApplication 인스턴스가 생성되었습니다.");

            InitializeIoC();

            // Application 요청 시작/완료 이벤트를 처리한다. 
            //
            BeginRequest += OnBeginRequest;
            EndRequest += OnEndRequest;

            Disposed += OnDisposed;
            Error += OnError;

            // 미리 실행하면, 성능상 잇점이 있는 작업을 비동기적으로 수행하도록 합니다.
            //
            With.TryActionAsync(() => Task.Factory.StartNew(ExecutePrefetchTask));
        }

        public virtual void Application_Start(object sender, EventArgs e) {
            if(IsInfoEnabled)
                log.Info("Application_Start()가 호출되었습니다. Application=[{0}]", _applicationVirtualPath);

            // 만약 Initialize가 안 되었을 수 있기 때문에
            InitializeIoC();
        }

        [Obsolete("초기화는 생성자 또는 Init()에서 수행하십시요. 비동기 작업을 하려면 ExecutePrefetchTask()를 수행하십시요")]
        protected virtual void Initialize() {
            // Nothing to do
        }

        /// <summary>
        /// <see cref="HttpApplication"/> 시작 시에 미리 실행하면, 성능상 잇점이 있는 작업을 비동기적으로 수행하도록 합니다.
        /// </summary>
        protected virtual void ExecutePrefetchTask() {
            if(IsDebugEnabled)
                log.Debug("HttpApplication 시작 시 미리 준비해야 할 작업을, 비동기적으로 미리 실행해 놓습니다...");
        }

        /// <summary>
        /// Web Application의 요청 처리 전에 사전 준비 사항을 처리한다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnBeginRequest(object sender, EventArgs e) {
            if(IsDebugEnabled)
                log.Debug("사용자 요청처리를 시작합니다...");
        }

        /// <summary>
        /// Web Application의 요청 처리 후 사후 처리를 한다. (실제 처리하는 것은 없고, 재정의가 가능하도록 남겨두었다.)
        /// </summary>
        protected virtual void OnEndRequest(object sender, EventArgs e) {
            if(IsDebugEnabled)
                log.Debug("사용자 요청처리를 완료합니다...");
        }

        /// <summary>
        /// <see cref="HttpApplication"/>이 메모리에서 제거될 때 발생하는 이벤트의 처리기
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnDisposed(object sender, EventArgs e) {
            if(IsInfoEnabled)
                log.Info("WindsorHttpApplication 인스턴스가 Dispose되었습니다. Application : " + _applicationVirtualPath);
        }

        /// <summary>
        /// Web Application에서 예외가 발생한 경우 발생하는 이벤트의 처리기
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnError(object sender, EventArgs e) {
            var ex = Server.GetLastError();

            if(ex != null) {
                if(log.IsErrorEnabled) {
                    log.Error("웹 응용 프로그램에서 예외가 발생했습니다!!!");
                    log.Error(ex);
                }
                Server.ClearError();
            }
        }

        /// <summary>
        /// 현재 사용하는 Castle.Windsor의 Container
        /// </summary>
        public IWindsorContainer Container {
            get { return IoC.Container; }
        }

        /// <summary>
        /// <see cref="IoC"/>를 초기화합니다.
        /// </summary>
        protected void InitializeIoC() {
            if(IoC.IsNotInitialized) {
                try {
                    IoC.Initialize(SetUpContainer());
                }
                catch(Exception ex) {
                    if(log.IsErrorEnabled)
                        log.Error("IoC를 초기화하는데 실패했습니다", ex);
                }
            }
        }

        /// <summary>
        /// Windsor Container를 초기화하여 반환합니다. <see cref="IWindsorInstaller"/> 를 이용한 초기화를 수행할 수 있습니다.
        /// </summary>
        /// <returns></returns>
        protected virtual IWindsorContainer SetUpContainer() {
            var container = new WindsorContainer();
            container.Install(Configuration.FromAppConfig());
            return container;
        }
    }
}