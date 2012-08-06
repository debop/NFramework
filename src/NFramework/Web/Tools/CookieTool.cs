using System;
using System.Text;
using System.Web;
using NSoft.NFramework.Tools;
using NSoft.NFramework.Web.Tools;

namespace NSoft.NFramework.Web {
    /// <summary>
    /// Utility class for <see cref="HttpCookie"/>
    /// </summary>
    /// <remarks>
    /// 쿠키 사이즈 제한을 어느정도 없앨 수 있도록, 압축 및 Chunking을 지원합니다.
    /// </remarks>
    public static class CookieTool {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 쿠키에 문자열 값을 저장합니다.
        /// </summary>
        /// <param name="name">쿠키 명</param>
        /// <param name="value">쿠키 값 (Plain Text여야 합니다.)</param>
        /// <param name="expireDate">유효기간</param>
        /// <param name="domain">쿠키 도메인명</param>
        public static void Set(string name, string value, DateTime? expireDate = null, string domain = null) {
            if(IsDebugEnabled)
                log.Debug("Set Cookie to HttpResponse... cookie name=[{0}], value=[{1}], expireDate=[{2}], domain=[{3}]",
                          name, value, expireDate, domain);

            name.ShouldNotBeNull("name");

            var cookie = new HttpCookie(name)
                         {
                             Value = Convert.ToBase64String(Encoding.UTF8.GetBytes(value))
                         };

            if(expireDate.HasValue)
                cookie.Expires = expireDate.Value;

            if(domain.IsNotWhiteSpace())
                cookie.Domain = domain;

            HttpContext.Current.Response.AppendCookie(cookie);

            if(IsDebugEnabled)
                log.Debug("Set Cookie to HttpResponse is SUCCESS!!! cookie name=[{0}], value=[{1}], expireDate=[{2}], domain=[{3}]",
                          cookie.Name, cookie.Value, cookie.Expires, cookie.Domain);
        }

        /// <summary>
        /// 지정된 쿠키에 저장된 값을 반환합니다.
        /// </summary>
        /// <param name="name">쿠키 명</param>
        /// <returns>쿠키 값</returns>
        public static string Get(string name) {
            name.ShouldNotBeNull("name");

            try {
                var cookie = HttpContext.Current.Request.Cookies[name];
                if(cookie != null)
                    return Encoding.UTF8.GetString(Convert.FromBase64String(cookie.Value));
            }
            catch(Exception ex) {
                if(log.IsWarnEnabled) {
                    log.Warn("Fail to retrieve cookie value. cookie name=[{0}]", name);
                    log.Warn(ex);
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 지정된 쿠키에 저장된 값을 형변환을 하여 반환합니다.
        /// </summary>
        /// <typeparam name="T">반환받는 값의 형식</typeparam>
        /// <param name="name">쿠키명</param>
        /// <param name="defaultValue">쿠키 값 얻기에 실패했을 시에 사용할 기본값</param>
        /// <returns>쿠키 값</returns>
        public static T Get<T>(string name, T defaultValue) {
            name.ShouldNotBeNull("name");

            try {
                var cookie = GetChuck<T>(name);
                return Equals(cookie, default(T)) ? defaultValue : cookie;
            }
            catch(Exception ex) {
                if(log.IsWarnEnabled) {
                    log.Warn("Fail to retrieve cookie value. cookie name=[{0}]", name);
                    log.Warn(ex);
                }
            }
            return defaultValue;
        }

        /// <summary>
        /// 쿠키 정보를 생성하여 HttpResponse에 추가합니다. 대용량 값인 경우 <see cref="CookieChunker{T}"/>를 이용하여 값을 조각내어 저장합니다.
        /// </summary>
        /// <typeparam name="T">쿠키 값의 수형</typeparam>
        /// <param name="name">쿠키 명</param>
        /// <param name="value">쿠키 값</param>
        /// <param name="expireDate">폐기일자</param>
        /// <param name="domain">Domain name</param>
        public static void SetChuck<T>(string name, T value, DateTime? expireDate = null, string domain = null) {
            if(WebTool.IsWebContext == false)
                return;

            value.ShouldNotBeNull("value");

            if(IsDebugEnabled)
                log.Debug("Set Cookie. name=[{0}], value=[{1}], expireDate=[{2}], domain=[{3}]", name, value, expireDate, domain);

            var cookies = CookieChunker<T>.Instance.Chunk(name, value);

            var response = HttpContext.Current.Response;
            response.ShouldNotBeNull("response");

            for(var i = 0; i < cookies.Count; i++) {
                if(domain.IsNotWhiteSpace())
                    cookies[i].Domain = domain;

                if(expireDate.HasValue)
                    cookies[i].Expires = expireDate.Value;

                response.AppendCookie(cookies[i]);
            }
        }

        /// <summary>
        /// HttpRequeest로부터 쿠키 정보를 읽어옵니다.
        /// </summary>
        /// <typeparam name="T">쿠키 값의 수형</typeparam>
        /// <param name="name">쿠키 명</param>
        /// <returns>해당 쿠키 값, 없으면 기본값 반환</returns>
        public static T GetChuck<T>(string name) {
            if(IsDebugEnabled)
                log.Debug("쿠키 값을 얻습니다... name=[{0}]", name);

            if(HttpContext.Current.Request.Cookies[name] == null)
                return default(T);

            return CookieChunker<T>.Instance.Unchunk(HttpContext.Current.Request.Cookies, name);
        }

        /// <summary>
        /// Delete cookie by name
        /// </summary>
        /// <param name="name">cookie name</param>
        /// <returns>삭제여부</returns>
        public static bool DeleteChuck(string name) {
            if(IsDebugEnabled)
                log.Debug("쿠키를 삭제합니다. name=[{0}]", name);

            try {
                if(WebTool.IsWebContext) {
                    string cookieName = name;
                    int count = 0;

                    while(HttpContext.Current.Response.Cookies[cookieName] != null) {
                        HttpContext.Current.Response.Cookies.Remove(cookieName);

                        if(IsDebugEnabled)
                            log.Debug("Delete a cookie is SUCCESS. cookie name=[{0}]", cookieName);

                        cookieName = name + CookieChunker<Object>.CHUNK_DELIMITER + ++count;
                    }
                    return count > 0;
                }
            }
            catch(Exception ex) {
                if(log.IsWarnEnabled) {
                    log.Warn("Fail to delete a cookie. cookie name=[{0}]", name);
                    log.Warn(ex);
                }
            }
            return false;
        }
    }
}