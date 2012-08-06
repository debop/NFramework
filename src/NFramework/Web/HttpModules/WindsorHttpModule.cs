using System;
using System.Web;
using System.Web.Hosting;
using NSoft.NFramework.InversionOfControl;
using NSoft.NFramework.Web.HttpApplications;

namespace NSoft.NFramework.Web.HttpModules {
    /// <summary>
    /// Web Application에서 Castle.Windsor 를 이용한 IoC 를 초기화를 담당한다.<br/>
    /// IoCHttpModule을 사용하면, <see cref="WindsorHttpApplication"/>와 같이 상속을 받을 필요 없이, 
    /// IoC Container에 대한 초기화를 담당해주므로, 확장성이 뛰어나고, DNN 처럼 CustomHttpApplication이 있는 경우에 PlugIn 방식으로 끼워 넣을 수도 있다.
    /// </summary>
    /// <remarks>
    /// 이 모듈을 사용하려면 환경설정에 기본적으로 Castle.Windsor 환경 설정 정보가 있어야 합니다. 
    /// <see cref="IoC"/> 사용시에 초기화 여부를 검사할 필요가 없다.
    /// </remarks>
    /// <example>
    /// IIS 7.0 클래식 모드와 IIS 7.0 이전 버전에 적용할 때
    /// <code>
    /// <configuration>
    ///   <system.web>
    ///		<httpModule>
    ///			<add name="WindsorHttpModule" type="NSoft.NFramework.Web.WindsorHttpModule, NSoft.NFramework"/>
    ///		</httpModule>
    ///   </system.web>
    ///	</configuration>
    /// </code>
    /// IIS 7.0 통합모드에 적용할 때
    /// <code>
    /// <configuration>
    ///   <system.webServer>
    ///		<module>
    ///			<add name="WindsorHttpModule" type="NSoft.NFramework.Web.WindsorHttpModule, NSoft.NFramework"/>
    ///		</module>
    ///   </system.webServer>
    ///	</configuration>
    /// </code>
    /// </example>
    /// <seealso cref="WindsorHttpApplication"/>
    public class WindsorHttpModule : IHttpModule {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        static WindsorHttpModule() {
            if(log.IsInfoEnabled)
                log.Info("IoCHttpModule에서 IoC 를 초기화를 시작합니다... ");

            try {
                if(IoC.IsNotInitialized) {
                    IoC.Initialize();

                    if(log.IsInfoEnabled)
                        log.Info("IoCHttpModule에서 IoC 를 초기화를 완료했습니다!!!");
                }
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled)
                    log.ErrorException("IoCHttpModule에서 IoC 초기화시에 예외가 발생했습니다.", ex);

                throw;
            }
        }

        /// <summary>
        /// 모듈을 초기화하고 요청을 처리할 수 있도록 준비합니다.
        /// </summary>
        /// <param name="context">
        /// ASP.NET 응용 프로그램 내의 모든 응용 프로그램 개체에 공통되는 메서드, 속성 및 이벤트에 액세스할 수 있도록 하는 <see cref="T:System.Web.HttpApplication" />입니다. 
        /// </param>
        public void Init(HttpApplication context) {
            if(log.IsInfoEnabled)
                log.Info("새로운 IoCHttpModule 인스턴스를 초기화합니다. Application:" + HostingEnvironment.ApplicationVirtualPath);

            context.BeginRequest += OnBeginRequest;
            context.EndRequest += OnEndRequest;
        }

        /// <summary>
        /// <see cref="T:System.Web.IHttpModule" />을 구현하는 모듈에서 사용하는 리소스(메모리 제외)를 삭제합니다.
        /// </summary>
        public void Dispose() {
            // 매 호출시마다 IoC를 Reset하는 게 아니라 Application이 끝나는 시점에 한번만 하면 된다.
            // IoC.Reset();
            if(IsDebugEnabled)
                log.Debug("IoCHttpModule을 dispose 했습니다.");
        }

        /// <summary>
        /// ASP.NET에서 페이지 또는 XML Web services 같은 이벤트 처리기의 실행을 시작하기 바로 전에 발생하는 Event에 대한 Handler<br/>
        /// IoC 관련 정보를 초기화 합니다.
        /// </summary>
        /// <remarks>
        /// 기본 설정 파일이 아닌 다른 파일에 대해 초기화를 하려면 재정의를 해야 한다.
        /// </remarks>
        public virtual void OnBeginRequest(object sender, EventArgs e) {
            if(IsDebugEnabled)
                log.Debug("HttpRequest 요청이 시작되었습니다...");
        }

        /// <summary>
        /// Web Application의 요청 처리 후 사후 처리를 한다. (실제 처리하는 것은 없고, 재정의가 가능하도록 남겨두었다.)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEndRequest(object sender, EventArgs e) {
            if(IsDebugEnabled)
                log.Debug("HttpRequest 요청이 완료되었습니다...");
        }
    }
}