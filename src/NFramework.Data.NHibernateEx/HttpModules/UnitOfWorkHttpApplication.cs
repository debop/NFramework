using System;
using System.Linq;
using System.Web;
using NSoft.NFramework.IO;
using NSoft.NFramework.InversionOfControl;
using NSoft.NFramework.Tools;
using NSoft.NFramework.Web.HttpApplications;

namespace NSoft.NFramework.Data.NHibernateEx {
    /// <summary>
    /// Web Application에서 NHibernate용 Unit-Of-Work 을 편리하게 사용하기 위해 구현하였다.<br/>
    /// 모든 요청에 대해 UnitOfWork를 시작/완료를 자동으로 수행하게 하므로서, 
    /// 실제 page/web service 개발자가 UnitOfWork의 초기화 및 종료 처리를 수행하지 않아도 된다.<br/><br/>
    /// Web Application의 global.asax에서 Inherits="NSoft.NFramework.Data.NH.UnitOfWorkHttpApplication" 를 추가하면 됩니다.
    /// </summary>
    /// <remarks>
    /// Session 사용이 가능한 Web Application에서는 여러 페이지에 걸쳐 UnitOfWork를 유지할 수 있다. 
    /// 이를 Long Conversation이라 하는데, Web Application에서의 다중 작업에 대해 Transaction을 유지 할 수 있다. 
    /// </remarks>
    /// <example>
    /// <code>
    ///	// globals.asax를 다음과 같이 정의하면, UnitOfWork 와 관련된 처리를 자동으로 수행해준다.
    /// <%@ Application Language="C#" Inherits="NSoft.NFramework.Data.NH.UnitOfWorkHttpApplication" %>
    /// <script RunAt="server">
    /// 
    /// 	// 실제 처리는 IocHttpApplication에서 수행한다.
    /// 	private void Application_Start(object sender, EventArgs e)
    /// 	{
    /// 		// log4net을 사용하기 위해
    /// 		log4net.Config.XmlConfigurator.Configure();
    /// 	}
    /// 	
    /// 	private void Application_End(object sender, EventArgs e)
    /// 	{
    /// 		log4net.LogManager.Shutdown();	  
    /// 	}
    /// </script>
    /// </code>
    /// </example>
    /// <seealso cref="HttpApplication.BeginRequest"/>
    /// <seealso cref="HttpApplication.EndRequest"/>
    /// <see cref="AsyncHttpApplicationBase"/>
    public class UnitOfWorkHttpApplication : WindsorAsyncHttpApplication {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// Static constructor
        /// </summary>
        static UnitOfWorkHttpApplication() {
            if(log.IsInfoEnabled)
                log.Info("UnitOfWorkHttpApplication이 생성되었습니다...");
        }

        private readonly object _syncLock = new object();

        /// <summary>
        /// ASP.NET에서 페이지 또는 XML Web services 같은 이벤트 처리기의 실행을 시작하기 바로 전에 발생하는 Event에 대한 Handler<br/>
        /// 1. Application 생성 시 한번만 IoC 관련 정보를 초기화 합니다.<br/>
        /// 2. UnitOfWork를 Start 시킵니다.
        /// </summary>
        /// <remarks>
        /// 기본 설정 파일이 아닌 다른 파일에 대해 초기화를 하려면 재정의를 해야 한다.
        /// </remarks>
        protected override void OnBeginRequest(object sender, EventArgs e) {
            base.OnBeginRequest(sender, e);

            if(NeedStartUnitOfWork() == false) {
                if(HttpContext.Current != null)
                    if(IsDebugEnabled)
                        log.Debug("Unit of Work를 시작할 필요가 없는 단순 파일 요청이므로, UnitOfWork를 시작하지 않습니다!!! RawUrl=[{0}]",
                                  HttpContext.Current.Request.RawUrl);
                return;
            }

            //lock(_syncLock)
            {
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
                            log.Debug("UnitOfWork를 시작했습니다!!!");
                    }
                }
            }
        }

        /// <summary>
        /// Web Application의 요청 처리 후 사후 처리를 한다.
        /// UnitOfWork 를 Stop 시키고, 
        /// </summary>
        protected override void OnEndRequest(object sender, EventArgs e) {
            if(IsDebugEnabled)
                log.Debug("사용자 요청을 모두 처리하고, UnitOfWork에 대해 종료처리를 시작합니다...");

            // lock(_syncLock)
            {
                if(HttpContext.Current != null && UnitOfWork.IsStarted) {
                    // no error in asp.net pages and long conversion mode in Unit Of Work.
                    //
                    if(HttpContext.Current.Server.GetLastError() == null && UnitOfWork.InLongConversation) {
                        if(IsDebugEnabled)
                            log.Debug("Disconnect current NHibernate session. not close, just disconnect!!!.");

                        if(UnitOfWork.CurrentSession != null && UnitOfWork.CurrentSession.IsConnected)
                            UnitOfWork.CurrentSession.Disconnect();

                        if(IsAspSessionAvailable == false)
                            throw new InvalidOperationException(
                                @"HttpSession must be enabled when using Long Conversation in Unit Of Work pattern!!!"
                                + @"If you are using web services, make sure use [WebMethod(EnabledSession=true)]");

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
            }

            if(IsDebugEnabled)
                log.Debug("사용자 요청을 모두 처리하고, UnitOfWork에 대해 종료처리를 완료했습니다.");

            // Finalize 
            base.OnEndRequest(sender, e);
        }

        /// <summary>
        /// ASP.NET Session is available
        /// </summary>
        private static bool IsAspSessionAvailable {
            get { return (HttpContext.Current != null && HttpContext.Current.Session != null); }
        }

        /// <summary>
        /// UnitOfWork를 시작시켜야할 요청인지 판단합니다. gif 파일이나 css 파일 요청 시에는 굳이 UnitOfWork를 시작할 필요가 없습니다.
        /// </summary>
        /// <returns></returns>
        internal static bool NeedStartUnitOfWork() {
            if(HttpContext.Current == null)
                return false;

            var request = HttpContext.Current.Request;
            var scriptFileExt = request.CurrentExecutionFilePath.ExtractFileExt();

            if(scriptFileExt.IsNotWhiteSpace())
                if(NoNeedUnitOfWorkExtensions.Any(scriptFileExt.Contains))
                    return false;

            return true;
        }

        /// <summary>
        /// Application_Start 시에 실행할 비동기 작업의 본체입니다.
        /// </summary>
        protected override void ApplicationStartAfter(HttpContext context) {
            base.ApplicationStartAfter(context);

            // 성능을 위해 미리 NHibernate SessionFactory를 준비합니다.
            //
            try {
                if(IoC.IsNotInitialized)
                    return;

                if(IsDebugEnabled)
                    log.Debug("성능을 위해 미리 SessionFactory를 생성합니다...");

                using(UnitOfWork.Start(UnitOfWorkNestingOptions.CreateNewOrNestUnitOfWork)) {
                    var factory = UnitOfWork.CurrentSessionFactory;

                    if(IsDebugEnabled)
                        log.Debug("SessionFactory [{0}]이 생성되었습니다!!!", factory.GetSessionFactoryName());
                }
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled) {
                    log.Error("NHibernate Session Factory를 빌드하는데 실패했습니다.");
                    log.Error(ex);
                }
            }
        }

        /// <summary>
        /// UnitOfWork.Start()를 수행하지 않아도 될 요청
        /// </summary>
        internal static readonly string[] NoNeedUnitOfWorkExtensions
            = new string[]
              {
                  "gif",
                  "png",
                  "jpg",
                  "jpeg",
                  "ico",
                  "js",
                  "css",
                  "txt",
                  "xml",
                  "sql",
                  "htm",
                  "html"
              };
    }
}