using System;
using System.Threading;
using System.Web;
using NSoft.NFramework.Web.Access;
using NSoft.NFramework.Web.Tools;

namespace NSoft.NFramework.Web
{
    /// <summary>
    /// Current Thread Context에서 유지할 정보를 제공합니다.
    /// </summary>
    public static partial class WebAppContext
    {
        internal const string CurrentCompanyCodeKey = "NSoft.NFramework.Web.Current.CompanyCode.Key";
        internal const string CurrentOrganizationCodeKey = "NSoft.NFramework.Web.Current.OrganizationCode.Key";
        internal const string CurrentLoginIdKey = "NSoft.NFramework.Web.Current.LoginId.Key";
        internal const string CurrentLocaleKeyKey = "NSoft.NFramework.Web.Current.LocaleKey.Key";
        internal const string CurrentThemeKey = "NSoft.NFramework.Web.Current.ThemeItem.Key";

        /// <summary>
        /// Current Thread Context에 보관된 정보
        /// </summary>
        public static class Current
        {
            /// <summary>
            /// Current User Identity
            /// </summary>
            public static IAccessIdentity Identity
            {
                get { return Local.Data[CurrentLocaleKeyKey] as IAccessIdentity; }
                set { Local.Data[CurrentLocaleKeyKey] = value; }
            }

            /// <summary>
            /// Current User ThemeItem
            /// </summary>
            public static string ThemeName
            {
                get { return Local.Data[CurrentThemeKey].AsText(AppSettings.DefaultThemeAssemblyName); }
                set
                {
                    Local.Data[CurrentThemeKey] = value;
                    WebAppTool.SetValue(AppSettings.ApplicationName + @"_" + CurrentThemeKey,
                                       value.AsText(),
                                       DateTime.MinValue,
                                       string.Empty);
                }
            }

            /// <summary>
            /// Current User ThemeItem
            /// </summary>
            public static string LocaleKey
            {
                get { return Thread.CurrentThread.CurrentCulture.Name.AsText(); }
                set { CookieTool.Set(AppSettings.UserLocaleCookieName, value.AsText()); }
            }

            /// <summary>
            /// Current 정보를 초기화 한다.
            /// </summary>
            public static void Init()
            {
                var identity = Services.Authentication.Identity;

                if(identity != null)
                    SetCurrent(identity);
            }

            /// <summary>
            /// Current를 셋팅한다.
            /// </summary>
            /// <param name="identity"></param>
            public static void SetCurrent(IAccessIdentity identity)
            {
                if(log.IsDebugEnabled)
                    log.Debug("접속정보를 할당합니다. identity=[{0}]", identity);

                identity.ShouldNotBeNull("identity");

                Identity = identity;

                ThemeName = WebAppTool.LoadValue(AppSettings.ApplicationName + @"_" + CurrentThemeKey, AppSettings.DefaultThemeAssemblyName);

                var principal = Services.Authentication.CreatePrincipal(Services.Authentication.Identity);
                if(Local.IsInWebContext)
                    HttpContext.Current.User = principal;
                Thread.CurrentPrincipal = principal;
            }
        }
    }
}