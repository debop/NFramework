using System;
using System.Text;
using System.Web;
using NSoft.NFramework.Serializations.Serializers;

namespace NSoft.NFramework.Web {
    /// <summary>
    /// Cookie Size를 조절하여 4096 byte보다 큰 경우에는 쪼개서 저장한다.
    /// </summary>
    /// <remarks>
    /// Browser에서 Cookie 관련한 RFC 2109의 추천 조건:<br/>
    /// 1. 적어도 300개의 쿠키를 저장할 수 있어야 한다.<br/>
    /// 2. 적어도 쿠키 값의 크기는 4096 byte 가 되어야 한다.<br/>
    /// 3. 적어도 도메인별 쿠키는 20개를 지원해야 한다.<br/>
    /// </remarks>
    public class CookieChunker<T> {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private static readonly ISerializer<T> Serializer = new CompressSerializer<T>(new BinarySerializer<T>());

        /// <summary>
        /// 빈 쿠키 컬렉션
        /// </summary>
        public static readonly HttpCookieCollection EmptyCookieCollection = new HttpCookieCollection();

        /// <summary>
        /// 단위 Chunk의 최대 크기
        /// </summary>
        public const int MAX_SIZE = 4000;

        /// <summary>
        /// Chunk 구분자
        /// </summary>
        public const string CHUNK_DELIMITER = "_";

        /// <summary>
        /// Singleton instance of CookieChunker{T}
        /// </summary>
        public static CookieChunker<T> Instance {
            get { return SingletonTool<CookieChunker<T>>.Instance; }
        }

        /// <summary>
        /// 지정된 이름과 값을 이용하여, 대용량 값을 chunk하여, 쿠키 컬렉션을 만든다.
        /// </summary>
        /// <param name="name">쿠키 명</param>
        /// <param name="value">쿠키 값</param>
        /// <returns>collection of chuncked cookies</returns>
        public HttpCookieCollection Chunk(string name, T value) {
            if(IsDebugEnabled)
                log.Debug("Chuck cookie value. cookie name=[{0}], value=[{1}]", name, value);

            var cookies = new HttpCookieCollection();

            if(Equals(value, default(T)))
                return EmptyCookieCollection;

            var data = Serializer.Serialize(value);
            var base64Str = Convert.ToBase64String(data).Trim('\0');

            //if(base64Str.Length > 8 * MAX_SIZE)
            //    throw new InvalidOperationException("Cookie Value size is too large for Browser. Size=" + base64Str.Length);

            var chunckCount = base64Str.Length / MAX_SIZE + 1;

            for(var i = 0; i < chunckCount; i++) {
                var cookie = (i == 0) ? new HttpCookie(name) : new HttpCookie(name + CHUNK_DELIMITER + i);
                cookie.Value = base64Str.Substring(0 + i * MAX_SIZE, Math.Min(MAX_SIZE, base64Str.Length - MAX_SIZE * i));
                cookies.Add(cookie);
            }

            if(IsDebugEnabled)
                log.Debug("Chunk cookie value. chuncked size=[{0}]", chunckCount);

            return cookies;
        }

        /// <summary>
        /// chunck된 cookie collection으로부터 실제 값을 복원한다.
        /// </summary>
        /// <param name="cookies">collection of chunked cookie</param>
        /// <param name="name">값을 구할 쿠키명</param>
        /// <returns>쿠키 값</returns>
        public T Unchunk(HttpCookieCollection cookies, string name) {
            if(IsDebugEnabled)
                log.Debug("Get Cookie value by unchunking. name=[{0}]", name);

            var sb = new StringBuilder();
            var chunkName = name;
            var chunkIndex = 0;

            while(cookies[chunkName] != null) {
                sb.Append(cookies[chunkName].Value);
                chunkName = name + CHUNK_DELIMITER + ++chunkIndex;
            }

            string base64Str = sb.ToString().Trim('\0');

            if(IsDebugEnabled)
                log.Debug("Get Cookie value that is chunked. cookie name=[{0}], chunk count=[{1}]", name, chunkIndex);

            try {
                return Serializer.Deserialize(Convert.FromBase64String(base64Str));
            }
            catch(Exception ex) {
                if(log.IsWarnEnabled) {
                    log.Warn("쿠키 값을 읽어 [{0}] 수형의 인스턴스를 빌드하는데 실패했습니다!!!", typeof(T).FullName);
                    log.Warn(ex);
                }
                return default(T);
            }
        }
    }
}