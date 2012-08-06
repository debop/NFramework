using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Hosting;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Web.HttpModules {
    /// <summary>
    /// 웹 Application이 다국어를 지원할 경우, 사용자별 Locale 지정을 관리한다.
    /// </summary>
    /// <remarks>
    /// 로깅관련 포맷 및 레벨을 환경설정에서 설정할 수 있도록 하면 좋겠다.
    /// </remarks>
    /// <example>
    /// IIS 7.0 클래식 모드와 IIS 7.0 이전 버전에 적용할 때
    /// <code>
    /// <configuration>
    ///   <system.web>
    ///		<httpModule>
    ///			<add name="UserLocalizationModule" type="NSoft.NFramework.Web.HttpModules.UserLocalizationModule, NSoft.NFramework.Web"/>
    ///		</httpModule>
    ///   </system.web>
    ///	</configuration>
    /// </code>
    /// IIS 7.0 통합모드에 적용할 때
    /// <code>
    /// <configuration>
    ///   <system.webServer>
    ///		<module>
    ///			<add name="UserLocalizationModule" type="NSoft.NFramework.Web.HttpModules.UserLocalizationModule, NSoft.NFramework.Web"/>
    ///		</module>
    ///   </system.webServer>
    ///	</configuration>
    /// </code>
    /// 
    /// AppSetting에 설정할 정보 ( User Locale을 사용할 것인지, User Locale 정보가 저장될 쿠키 명)
    /// <code>
    /// <appSettings>
    ///     <add key="Use.UserLocale" value="True"/>
    ///		<add key="UserLocale.CookieName" value="_USER_COOKIE_"/>
    /// </appSettings>
    /// </code>
    /// </example>
    [Serializable]
    public class UserLocalizationModule : IHttpModule {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 모듈을 초기화하고 요청을 처리할 수 있도록 준비합니다.
        /// </summary>
        /// <param name="context">
        /// ASP.NET 응용 프로그램 내의 모든 응용 프로그램 개체에 공통되는 메서드, 속성 및 이벤트에 액세스할 수 있도록 하는 <see cref="T:System.Web.HttpApplication" />입니다. 
        /// </param>
        public void Init(HttpApplication context) {
            if(log.IsInfoEnabled)
                log.Info("새로운 UserLocalizationModule 인스턴스를 초기화합니다... Application=[{0}]", HostingEnvironment.ApplicationVirtualPath);

            context.BeginRequest += OnBeginRequest;
        }

        /// <summary>
        /// <see cref="T:System.Web.IHttpModule" />을 구현하는 모듈에서 사용하는 리소스(메모리 제외)를 삭제합니다.
        /// </summary>
        public void Dispose() {
            if(IsDebugEnabled)
                log.Debug("UserLocalizationModule 인스턴스를 Dispose 합니다. Application=[{0}]", HostingEnvironment.ApplicationVirtualPath);
        }

        /// <summary>
        /// 사용자 Locale 정보를 사용할지 여부에 대한 AppSetting의 키 (Localization.Use.UserLocale)
        /// </summary>
        public const string USE_USERLOCALE_KEY = "Localization.Use.UserLocale";

        /// <summary>
        /// 사용자 Locale 정보를 저장할 쿠키 이름에 대한 AppSetting의 키 (Localization.UserLocale.CookieName)
        /// </summary>
        public const string USER_LOCALE_COOKIE = "Localization.UserLocale.CookieName";

        /// <summary>
        /// 사용자 Locale 정보를 저장할 쿠키 이름의 기본값 (_USER_LOCALE_) (다른 것으로 변경하려면 <see cref="UserLocaleCookieName"/> 설명을 참조하세요.
        /// </summary>
        public const string DEFAULT_USER_LOCALE_COOKIE_NAME = "_USER_LOCALE_";

        private static readonly Lazy<bool> _lazyUseUserLocale
            = new Lazy<bool>(() => ConfigTool.GetAppSettings(USE_USERLOCALE_KEY, true));

        /// <summary>
        /// Web Application의 Culture를 사용하는 것이 아니라, 사용자별로 다국어를 지원할 것인지 여부를 판단합니다.<br/>
        /// AppSettings에 정의되어 있지 않다면, True를 기본값으로 합니다.
        /// </summary>
        /// <remarks>
        /// 사용자별로 Culture를 사용한다면, 다음의 정보를 이용하여
        /// 1. <see cref="UserLocaleCookieName"/>의 쿠키 정보<br/>
        /// 2. 사용자 Browser의 사용가능한 Language중의 첫번째 것 (<see cref="HttpRequest.UserLanguages"/>)<br/>
        /// Culture를 지정합니다.
        /// </remarks>
        /// <example>
        /// <code>
        /// // web.Config appSettings에 Use.UserLocale이 정의되어 있어야 합니다. (없으면 True로 간주합니다.)
        /// <appSettings>
        ///     <add key="Localization.Use.UserLocale" value="True"/>
        /// </appSettings>
        /// </code>
        /// </example>
        public static bool UseUserLocale {
            get { return _lazyUseUserLocale.Value; }
        }

        private static readonly Lazy<string> _lazyUserLocaleCookieName
            = new Lazy<string>(() => ConfigTool.GetAppSettings(USER_LOCALE_COOKIE, DEFAULT_USER_LOCALE_COOKIE_NAME));

        /// <summary>
        /// 사용자 Locale 정보가 저장될 쿠키 명<br/>
        /// appSettings에 UserLocale.CookieName 의 값을 가져옵니다.<br/>
        /// AppSettings에 정의되어 있지 않다면 <see cref="DEFAULT_USER_LOCALE_COOKIE_NAME"/> 을 기본값으로 사용합니다.
        /// </summary>
        /// <example>
        /// <code>
        /// // web.Config appSettings에 UserLocale.CookieName이 정의되어 있어야 합니다.
        /// <appSettings>
        ///     <add key="Localization.UserLocale.CookieName" value="_USER_LOCALE_"/>
        /// </appSettings>
        /// </code>
        /// </example>
        public static string UserLocaleCookieName {
            get { return _lazyUserLocaleCookieName.Value; }
        }

        /// <summary>
        /// ASP.NET에서 현재 요청과 관련된 현재 상태(예: 세션 상태)를 가져오는 경우 발생하는 이벤트에 대한 핸들러입니다.<br/>
        /// 사용자의 Locale 정보를 읽어서 현재 Context의 CultureInfo를 설정합니다. 
        /// </summary>
        public virtual void OnBeginRequest(object sender, EventArgs e) {
            CultureInfo culture = null;

            // 사용자가 지정한 Locale을 사용할 경우
            if(UseUserLocale) {
                try {
                    var cultureName = CookieTool.Get(UserLocaleCookieName);

                    // 쿠키에 User Locale 정보가 저장되어 있다면
                    if(cultureName.IsNotWhiteSpace()) {
                        culture = new CultureInfo(cultureName);

                        if(IsDebugEnabled)
                            log.Debug("쿠키로부터 User Locale 정보를 얻었습니다. User Locale=[{0}]", cultureName);
                    }
                    else if(HttpContext.Current.Request.UserLanguages != null) {
                        var language = HttpContext.Current.Request.UserLanguages.FirstOrDefault();

                        if(language.IsNotWhiteSpace()) {
                            culture = new CultureInfo(language);
                            if(IsDebugEnabled)
                                log.Debug("사용자 Browser의 UserLanguage중에 첫번째 것을 User Locale로 사용합니다. language=[{0}", language);
                        }
                    }
                }
                catch(Exception ex) {
                    if(log.IsInfoEnabled) {
                        log.Info("쿠키로부터 User Locale 정보를 수집하는데 실패했습니다. 무시하고 시스템 기준값을 사용합니다.");
                        log.Info(ex);
                    }
                }
            }

            // 사용자 culture를 지정하지 못했다면, 현재 시스템의 Thread Context의 Culture를 지정합니다.
            if(culture.IsNullCulture()) {
                culture = CultureInfo.CurrentCulture;

                if(IsDebugEnabled)
                    log.Debug("시스템 설정 기준의 Locale을 사용합니다. Culture=[{0}]", culture.Name);
            }

            SetCurrentCulture(culture);
        }

        /// <summary>
        /// CurrentCulture를 지정한 culture로 설정합니다.
        /// </summary>
        /// <param name="culture"></param>
        private static void SetCurrentCulture(CultureInfo culture) {
            // UICulture = 'ko'
            // Culture = 'ko-KR' 

            Thread.CurrentThread.CurrentUICulture = culture ?? Thread.CurrentThread.CurrentUICulture;

            // NOTE: Culture는 neutral culture를 지정할 수 없고, SpecificCulture를 지정해야 한다.
            //
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(Thread.CurrentThread.CurrentUICulture.Name);

            if(IsDebugEnabled)
                log.Debug("Current Thread Culture를 지정했습니다. CurrentUICulture=[{0}], CurrrentCulture=[{1}]",
                          Thread.CurrentThread.CurrentUICulture,
                          Thread.CurrentThread.CurrentCulture);

            if(UseUserLocale)
                CookieTool.Set(UserLocaleCookieName, Thread.CurrentThread.CurrentCulture.Name);
        }
    }
}