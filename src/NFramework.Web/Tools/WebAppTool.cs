using System;
using System.Text.RegularExpressions;
using System.Web;
using NSoft.NFramework.IO;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Web.Tools
{
    /// <summary>
    /// 웹 Application에서 공통으로 사용할 Utility Class입니다.
    /// </summary>
    public static partial class WebAppTool
    {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public static readonly Regex MailRegex = new Regex(@"\\w+([-+.]\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*", RegexOptions.Compiled);
        public static readonly Regex NumberRegex = new Regex(@"(^([0-9]*|\\d*\\d{1}?\\d*)$)", RegexOptions.Compiled);

        ///// <summary>
        ///// ASP.NET 응용프로그램의 가상 루트 경로 (예: http://localhost/RealAdmin/)
        ///// </summary>
        //public static string ApplicationPath
        //{
        //    get { return HttpContext.Current.Request.ApplicationPath; }
        //}

        ///// <summary>
        ///// ASP.NET 응용프로그램의 루트 가상 경로 (예: http://localhost/RealAdmin/)
        ///// </summary>
        //public static string ApplicationVirtualPath
        //{
        //    get { return HostingEnvironment.ApplicationVirtualPath; }
        //}

        /// <summary>
        /// 현재 활성화된 Page를 찾는다. (없으면 null을 반환한다.)
        /// </summary>
        public static System.Web.UI.Page CurrentPage
        {
            get
            {
                if(HttpContext.Current != null && HttpContext.Current.Handler != null)
                    return HttpContext.Current.Handler as System.Web.UI.Page;
                return null;
            }
        }

        /// <summary>
        /// 현재 처리처리중인 파일명을 반환합니다.
        /// </summary>
        public static string CurrentFileName
        {
            get
            {
                var path = HttpContext.Current.Request.PhysicalPath;
                return path.IsNotWhiteSpace() ? path.ExtractFileName() : string.Empty;
            }
        }

        ///<summary>
        /// 입력
        ///</summary>
        /// <typeparam name="T">유형</typeparam>
        ///<param name="name">명</param>
        ///<param name="value">값</param>
        ///<param name="expiredDate">만료일자(웹인 경우만 해당)</param>
        ///<param name="domain">도메인(웹인 경우만 해당)</param>
        public static void SetValue<T>(string name, T value, DateTime? expiredDate = null, string domain = null)
        {
            name.ShouldNotBeWhiteSpace("name");

            if(WebTool.IsWebContext)
                CookieTool.SetChuck(name, value, expiredDate, domain);
            else
                Local.Data[name] = value;
        }

        ///<summary>
        /// 값을 읽어온다.
        ///</summary>
        ///<typeparam name="T">유형</typeparam>
        ///<param name="name">명</param>
        ///<returns>값</returns>
        public static T LoadValue<T>(string name)
        {
            return LoadValue(name, default(T));
        }

        ///<summary>
        /// 값을 읽어온다.
        ///</summary>
        ///<typeparam name="T">유형</typeparam>
        ///<param name="name">명</param>
        ///<param name="defaultValue">기본값</param>
        ///<returns>값</returns>
        public static T LoadValue<T>(string name, T defaultValue)
        {
            name.ShouldNotBeWhiteSpace("name");

            return WebTool.IsWebContext ? CookieTool.Get(name, defaultValue) : Local.Data[name].AsValue(defaultValue);
        }

        ///<summary>
        /// 값을 제거한다.
        ///</summary>
        ///<param name="name">명</param>
        public static void DeleteValue(string name)
        {
            name.ShouldNotBeWhiteSpace("name");

            if(WebTool.IsWebContext)
            {
                var cookie = new HttpCookie(name, string.Empty)
                             {
                                 HttpOnly = true,
                                 Expires = new DateTime(0x7cf, 10, 12)
                             };

                HttpContext.Current.Response.Cookies.Remove(name);
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
            else
                Local.Data[name] = null;
        }

    }
}