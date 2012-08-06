using System;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Web
{
    /// <summary>
    /// NSoft.NFramework Web Application 환경설정의 AppSettings 섹션에 정의된 설정 정보를 제공합니다.
    /// </summary>
    public class AppSettings
    {
        
        #region << Organization >>

        /// <summary>
        /// 그룹 코드 (기업의 최상위 그룹)
        /// </summary>
        public static string EnterpriseName
        {
            get { return ConfigTool.GetAppSettings(@"NSoft.NFramework.Web.EnterpriseName", Default.EnterpriseName); }
        }

        /// <summary>
        /// 제품코드 예) RealBPM, RealBPA, RealEA
        /// </summary>
        public static string ApplicationName
        {
            get { return ConfigTool.GetAppSettings(@"NSoft.NFramework.Web.ApplicationName", Default.ApplicationName); }
        }

        /// <summary>
        /// 접속자에 대한 기본 기업코드 (그룹의 계열사)
        /// </summary>
        public static string CompanyCode
        {
            get { return ConfigTool.GetAppSettings(@"NSoft.NFramework.WebAppContext.Current.CompanyCode", Default.CompanyCode); }
        }

        #endregion

        #region << Authentication >>

        /// <summary>
        /// 로그인 인증 필요여부
        /// </summary>
        public static bool NeedLogin
        {
            get { return ConfigTool.GetAppSettings("NSoft.NFramework.Web.Authentication.NeedLogin", Default.NeedLogin); }
        }

        /// <summary>
        /// 로그인 가장(Impersonate) 사용 여부 : true명 로그인 없이도 사용 가능, NeedLogin과 XOR 관계여야 함
        /// </summary>
        public static bool Impersonate
        {
            get { return ConfigTool.GetAppSettings("NSoft.NFramework.Web.Authentication.Impersonate", Default.Impersonate); }
        }

        /// <summary>
        /// 로그인 가장(Impersonate) 사용시 사용자명
        /// </summary>
        public static string ImpersonateUserName
        {
            get { return ConfigTool.GetAppSettings("NSoft.NFramework.Web.Authentication.Impersonate.UserName", Default.ImpersonateUserName); }
        }

        #endregion

        #region << Resources >>

        /// <summary>
        /// 리소스 중 용어(Glossary)를 나타내는 클래스 Name (리소스 파일명)
        /// </summary>
        public static string ResourceGlossary
        {
            get { return ConfigTool.GetAppSettings(@"NSoft.NFramework.Web.GlobalResources.ClassKey.Glossary", Default.ResourceGlossary); }
        }

        /// <summary>
        /// 리소스 중 메시지(Message) 정보를 나타내는 클래스 Name (리소스 파일명)
        /// </summary>
        public static string ResourceMessages
        {
            get { return ConfigTool.GetAppSettings(@"NSoft.NFramework.Web.GlobalResources.ClassKey.Messages", Default.ResourceMessages); }
        }

        /// <summary>
        /// 리소스 중 명령(Command) 정보를 나타내는 클래스 Name (리소스 파일명)
        /// </summary>
        public static string ResourceCommand
        {
            get { return ConfigTool.GetAppSettings(@"NSoft.NFramework.Web.GlobalResources.ClassKey.Command", Default.ResourceCommand); }
        }

        /// <summary>
        /// 리소스 중 사이트맵(Sitemap) 정보를 나타내는 클래스 Name (리소스 파일명)
        /// </summary>
        [Obsolete("삭제 예정입니다.")]
        public static string ResourceSitemap
        {
            get { return ConfigTool.GetAppSettings(@"NSoft.NFramework.Web.Resources.Sitemap", Default.ResourceSitemap); }
        }

        #endregion

        #region << UI >>

        /// <summary>
        /// 목록화면의 기본 PageSize (default=10)
        /// </summary>
        public static int DefaultPageSize
        {
            get { return ConfigTool.GetAppSettings("NSoft.NFramework.Web.UI.PageSize.Default", Default.DefaultPageSize); }
        }

        /// <summary>
        /// 기본 날짜 포맷 (default=yyyy-MM-dd) 
        /// </summary>
        public static string DisplayDateStringFormat
        {
            get { return ConfigTool.GetAppSettings("NSoft.NFramework.Web.UI.DateRangeStringFormat", Default.DisplayDateStringFormat); }
        }

        /// <summary>
        /// 컨트롤에서 한번의 요청에 추가적으로 보여주는 요소의 수 (default=10)
        /// </summary>
        public static int ItemsPerRequest
        {
            get { return ConfigTool.GetAppSettings("NSoft.NFramework.Web.UI.ItemsPerRequest", Default.ItemsPerRequest); }
        }

        /// <summary>
        /// 시스템에서 지원하는 Locale 목록
        /// </summary>
        public static string LocaleKeys
        {
            get { return ConfigTool.GetAppSettings("NSoft.NFramework.Web.Locale.LocaleKeys", Default.LocaleKeys); }
        }

        /// <summary>
        /// 사용자 Locale 정보를 사용할지 여부 (default=true)
        /// </summary>
        public static bool UseUserLocale
        {
            get { return ConfigTool.GetAppSettings("Localization.Use.UserLocale", Default.UseUserLocale); }
        }

        /// <summary>
        /// 사용자 Locale 설정 정보를 저장할 쿠키 명
        /// </summary>
        public static string UserLocaleCookieName
        {
            get { return ConfigTool.GetAppSettings("Localization.UserLocale.CookieName", Default.UserLocaleCookieName); }
        }

        /// <summary>
        /// NSoft 기본 테마
        /// </summary>
        public static string DefaultThemeAssemblyName
        {
            get { return ConfigTool.GetAppSettings("NSoft.ThemeItem.DefaultThemeAssemblyName", Default.DefaultThemeAssemblyName); }
        }

        /// <summary>
        /// 시스템에서 지원하는 테마 목록
        /// </summary>
        public static string ThemeAssemblyNames
        {
            get { return ConfigTool.GetAppSettings("NSoft.ThemeItem.AssemblyNames", Default.ThemeAssemblyNames); }
        }

        /// <summary>
        /// 로그인 페이지 URL (default=~/Login)
        /// </summary>
        public static string LoginUrl
        {
            get { return ConfigTool.GetAppSettings("NSoft.NFramework.Web.Pages.Url.Login", Default.LoginUrl); }
        }

        /// <summary>
        /// 로그아웃 페이지 URL (default=~/Logout)
        /// </summary>
        public static string LogoutUrl
        {
            get { return ConfigTool.GetAppSettings("NSoft.NFramework.Web.Pages.Url.Logout", Default.LogoutUrl); }
        }

        /// <summary>
        /// 기본 페이지 URL (default=~/Home)
        /// </summary>
        public static string DefaultUrl
        {
            get { return ConfigTool.GetAppSettings("NSoft.NFramework.Web.Pages.Url.Default", Default.DefaultUrl); }
        }

        /// <summary>
        /// 메시지 페이지 URL (default=~/Messages/MessagePage.aspx)
        /// </summary>
        public static string MessageBoxUrl
        {
            get { return ConfigTool.GetAppSettings("NSoft.NFramework.Web.Pages.Url.MessageBox", Default.MessageBoxUrl); }
        }

        #endregion

        #region << Mail >>

        /// <summary>
        /// SMTP 메일 서버 (default=mail.NSoft21.com)
        /// </summary>
        public static string MailHost
        {
            get { return ConfigTool.GetAppSettings("NSoft.NFramework.Web.Mail.Host", Default.MailHost); }
        }

        /// <summary>
        /// SMTP 메일 서버 Port (default=25)
        /// </summary>
        public static int MailPort
        {
            get { return ConfigTool.GetAppSettings<int>("NSoft.NFramework.Web.Mail.Port", Default.MailPort); }
        }

        /// <summary>
        /// 디버깅용 메일 발송자
        /// </summary>
        public static string MailSender
        {
            get { return ConfigTool.GetAppSettings("NSoft.NFramework.Web.Mail.Sender", Default.MailSender); }
        }

        /// <summary>
        /// 디버깅용 메일 수신자
        /// </summary>
        public static string MailReceipts
        {
            get { return ConfigTool.GetAppSettings("NSoft.NFramework.Web.Mail.Receipts", Default.MailReceipts); }
        }

        #endregion

        #region << File >>

        /// <summary>
        /// 파일 저장소 Root 경로
        /// </summary>
        public static string FileRepositoryRootDirectory
        {
            get { return ConfigTool.GetAppSettings("NSoft.NFramework.Web.File.Repository.RootDirectory", Default.RepositoryRootDirectory); }
        }

        #endregion

        #region << 기타 >>

        /// <summary>
        /// 스트링기반 아이템컬렉션에서의 문자열 구분자
        /// </summary>
        public static string Delimiter
        {
            get { return ConfigTool.GetAppSettings(@"NSoft.NFramework.Web.StringCollection.Delimiter", Default.Delimiter); }
        }

        #endregion

        /// <summary>
        /// 환경설정(<see cref="AppSettings"/>)의 기본값입니다.
        /// </summary>
        public struct Default
        {
            public const string EnterpriseName = "NSoft";
            public const string ApplicationName = "NSoft.NWebApplication";
            public const string CompanyCode = @"NSoft";
            public const string Delimiter = @"||";

            public const bool NeedLogin = true;
            public const bool Impersonate = false;
            public const string ImpersonateUserName = @"";

            public const string ResourceGlossary = @"Glossary";
            public const string ResourceMessages = @"Messages";
            public const string ResourceCommand = @"Command";

            [Obsolete("삭제 예정입니다.")]
            public const string ResourceSitemap = @"Sitemap";

            public const int DefaultPageSize = 10;
            public const string DisplayDateStringFormat = @"yyyy-MM-dd";
            public const int ItemsPerRequest = 10;
            public const string LocaleKeys = @"ko-KR,한국어";
            public const bool UseUserLocale = true;
            public const string UserLocaleCookieName = @"_USER_LOCALE_";
            public const string DefaultThemeAssemblyName = @"NSoft.ThemeItem.Blue";
            public const string ThemeAssemblyNames = @"Blue,NSoft.ThemeItem.Blue|Curve,NSoft.ThemeItem.Curve|HHI,NSoft.ThemeItem.HHI|Jadegreen,NSoft.ThemeItem.Jadegreen|Muji,NSoft.ThemeItem.Muji|Wave,NSoft.ThemeItem.Wave";

            public const string LoginUrl = @"~/Login";
            public const string LogoutUrl = @"~/Logout";
            public const string DefaultUrl = @"~/Home";
            public const string MessageBoxUrl = @"~/Messages/MessagePage.aspx";

            public const string MailHost = @"";
            public const int MailPort = 25;
            public const string MailSender = @"";
            public const string MailReceipts = @"";

            public const string RepositoryRootDirectory = @"~/Repository";
        }
    }
}