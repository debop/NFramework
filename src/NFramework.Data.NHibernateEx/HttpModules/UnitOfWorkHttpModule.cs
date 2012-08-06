using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using NSoft.NFramework.InversionOfControl;

namespace NSoft.NFramework.Data.NHibernateEx {
    /// <summary>
    /// Web Application에서 NHibernate용 Unit-Of-Work 을 편리하게 사용하기 위해 구현하였다.<br/>
    /// 모든 요청에 대해 UnitOfWork를 시작/완료를 자동으로 수행하게 하므로서, 
    /// 실제 page/web service 개발자가 UnitOfWork의 초기화 및 종료 처리를 수행하지 않아도 된다.<br/><br/>
    /// UnitOfWorkHttpModule을 사용하면, <see cref="UnitOfWorkHttpApplication"/>와 같이 상속을 받을 필요 없이, 
    /// UnitOfWork를 초기화를 담당해주므로, 확장성이 있으므로, DNN 처럼 CustomHttpApplication이 있는 경우에 PlugIn 방식으로 끼워 넣을 수도 있다.
    /// </summary>
    /// <remarks>
    /// 이 모듈을 사용하려면 환경설정에 기본적으로 Castle.Windsor 환경 설정 정보가 있어야 합니다.
    /// <see cref="UnitOfWork"/>를 사용할 때, Web Application 개발자가 명시적으로 <see cref="UnitOfWork.Start()"/>를 호출 할 필요 없다.<br/><br/>
    /// Session 사용이 가능한 Web Application에서는 여러 페이지에 걸쳐 UnitOfWork를 유지할 수 있다. 
    /// 이를 Long Conversation이라 하는데, Web Application에서의 다중 작업에 대해 Transaction을 유지 할 수 있다. 
    /// </remarks>
    /// <example>
    /// IIS 7.0 클래식 모드와 IIS 7.0 이전 버전에 적용할 때
    /// <code>
    /// &lt;configuration&gt;
    ///   &lt;system.web&gt;
    ///		&lt;httpModule&gt;
    ///			&lt;add name="UnitOfWorkHttpModule" type="NSoft.NFramework.Data.NH.UnitOfWorkHttpModule, NSoft.NFramework.Data"/&gt;
    ///		&lt;/httpModule&gt;
    ///   &lt;/system.web&gt;
    ///	&lt;/configuration&gt;
    /// </code>
    /// IIS 7.0 통합모드에 적용할 때
    /// <code>
    /// &lt;configuration&gt;
    ///   &lt;system.webServer&gt;
    ///		&lt;module&gt;
    ///			&lt;add name="UnitOfWorkHttpModule" type="NSoft.NFramework.Data.NH.UnitOfWorkHttpModule, NSoft.NFramework.Data"/&gt;
    ///		&lt;/module&gt;
    ///   &lt;/system.webServer&gt;
    ///	&lt;/configuration&gt;
    /// </code>
    /// </example>
    /// <seealso cref="UnitOfWorkHttpApplication"/>
    /// <seealso cref="WindsorHttpApplication"/>
    /// <seealso cref="IoCHttpModule"/>
    public class UnitOfWorkHttpModule : IHttpModule {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        // private const string Line = "UOW -------------------------------------------------------------------------------- UOW";

        /// <summary>
        /// Current Long Conversation Key
        /// </summary>
        public const string CurrentLongConversationKey = "NSoft.NFramework.Data.NH.CurrentLongConversation.Module.Key";

        ///// <summary>
        ///// Current NHibernate Session Key
        ///// </summary>
        //public const string CurrentNHibernateSessionKey = "NSoft.NFramework.Data.CurrentNHibernateSession.Key";

        private readonly object _syncLock = new object();

        /// <summary>
        /// 모듈을 초기화하고 요청을 처리할 수 있도록 준비합니다.
        /// </summary>
        /// <param name="context">
        /// ASP.NET 응용 프로그램 내의 모든 응용 프로그램 개체에 공통되는 메서드, 속성 및 이벤트에 액세스할 수 있도록 하는 <see cref="T:System.Web.HttpApplication" />입니다. 
        /// </param>
        public void Init(HttpApplication context) {
            if(log.IsInfoEnabled)
                log.Info("새로운 UnitOfWorkHttpModule 인스턴스를 초기화합니다. Application=[{0}]" + HostingEnvironment.ApplicationVirtualPath);

            context.BeginRequest += OnBeginRequest;
            context.EndRequest += OnEndRequest;

            // IoC를 Initialize 합니다.
            if(IoC.IsNotInitialized)
                IoC.Initialize();

            //! NOTE: SessionFactory를 Future 패턴을 이용하여, 미리 생성해 놓습니다.
            //
            Task.Factory.StartNew(() => {
                                      using(UnitOfWork.Start(UnitOfWorkNestingOptions.CreateNewOrNestUnitOfWork)) {
                                          var factory = UnitOfWork.CurrentSessionFactory;

                                          if(IsDebugEnabled)
                                              log.Debug("SessionFactory [{0}]이 생성되었습니다...", factory.GetSessionFactoryName());
                                      }
                                  },
                                  TaskCreationOptions.LongRunning);
        }

        /// <summary>
        /// <see cref="T:System.Web.IHttpModule" />을 구현하는 모듈에서 사용하는 리소스(메모리 제외)를 삭제합니다.
        /// </summary>
        public void Dispose() {
            if(IsDebugEnabled)
                log.Debug("UnitOfWorkHttpModule을 dispose 했습니다. Application=[{0}]", HostingEnvironment.ApplicationVirtualPath);
        }

        /// <summary>
        /// ASP.NET에서 페이지 또는 XML Web services 같은 이벤트 처리기의 실행을 시작하기 바로 전에 발생하는 Event에 대한 Handler<br/>
        /// 1. IoC 관련 정보를 초기화 합니다.<br/>
        /// 2. UnitOfWork를 Start 시킵니다.
        /// </summary>
        /// <remarks>
        /// 기본 설정 파일이 아닌 다른 파일에 대해 초기화를 하려면 재정의를 해야 한다.
        /// </remarks>
        public virtual void OnBeginRequest(object sender, EventArgs e) {
            if(UnitOfWorkHttpApplication.NeedStartUnitOfWork() == false) {
                if(HttpContext.Current != null)
                    if(IsDebugEnabled)
                        log.Debug("Unit of Work를 시작할 필요가 없는 단순 파일 요청이므로, UnitOfWork를 시작하지 않습니다!!! RawUrl=[{0}]",
                                  HttpContext.Current.Request.RawUrl);
                return;
            }

            lock(_syncLock) {
                bool loadedConversation = false;

                // Session이 사용 가능하다면, 저장된 IUnitOfWork 객체가 있는지 확인한다.
                if(IsAspSessionAvailable) {
                    if(IsDebugEnabled)
                        log.Debug("ASP.NET Session is available. Retrieve instance of IUnitOfWork from ASP.NET Session.");

                    loadedConversation = LongConversationManager.LoadConversation();
                }

                if(loadedConversation == false) {
                    if(UnitOfWork.IsNotStarted) {
                        UnitOfWork.Start();

                        if(IsDebugEnabled)
                            log.Debug("UnitOfWork 를 시작했습니다!!!");
                    }
                }
            }
        }

        /// <summary>
        /// Web Application의 요청 처리 후 사후 처리를 한다. (실제 처리하는 것은 없고, 재정의가 가능하도록 남겨두었다.)
        /// </summary>
        public virtual void OnEndRequest(object sender, EventArgs e) {
            if(IsDebugEnabled)
                log.Debug("사용자 요청을 모두 처리하고, UnitOfWork에 대해 종료처리를 시작합니다.");

            lock(_syncLock) {
                // no error in asp.net pages and long conversion mode in Unit Of Work.
                //
                if(HttpContext.Current.Server.GetLastError() == null && UnitOfWork.InLongConversation) {
                    if(IsDebugEnabled)
                        log.Debug("Disconnect current NHibernate session. not close, just disconnect!!!.");

                    UnitOfWork.CurrentSession.Disconnect();

                    if(IsAspSessionAvailable == false)
                        throw new InvalidOperationException(
                            "HttpSession must be enabled when using Long Conversation in Unit Of Work pattern!!! " +
                            "If you are using web services, make sure use [WebMethod(EnabledSession=true)]");

                    LongConversationManager.SaveConversation();
                }
                else {
                    if(IsDebugEnabled)
                        log.Debug("if not in long conversation mode. current unit of work instance is disposing.");

                    if(UnitOfWork.IsStarted) {
                        UnitOfWork.Stop();

                        if(IsDebugEnabled)
                            log.Debug("UnitOfWork를 끝냈습니다!!!");
                    }
                }
            }

            if(IsDebugEnabled)
                log.Debug("사용자 요청을 모두 처리하고, UnitOfWork에 대해 종료처리를 완료했습니다.");
        }

        /// <summary>
        /// ASP.NET Session is available
        /// </summary>
        private static bool IsAspSessionAvailable {
            get { return (HttpContext.Current != null && HttpContext.Current.Session != null); }
        }
    }
}