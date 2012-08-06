using System;
using System.Net;

namespace NSoft.NFramework.Web {
    /// <summary>
    /// <see cref="WebClient"/> 와 <see cref="HttpWebRequest"/>에 대해서, 일반 브라우저와 같은 속성 값을 미리 설정해서 생성해줍니다.
    /// </summary>
    public class WebClientFactory {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// FireFox의 User-Agent 값
        /// </summary>
        public const string UserAgentFireFox =
            @"Mozilla/5.0 (Windows NT 6.1; WOW64; rv:5.0) Gecko/20100101 Firefox/5.0";

        /// <summary>
        /// Chrome의 User-Agent 값
        /// </summary>
        public const string UserAgentChrome =
            @"Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/534.30 (KHTML, like Gecko) Chrome/12.0.742.100 Safari/534.30";

        /// <summary>
        /// IE8의 User-Agent 값
        /// </summary>
        public const string UserAgentIE8 =
            @"Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; WOW64; Trident/4.0; Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1)";

        /// <summary>
        /// IE9의 User-Agent 값
        /// </summary>
        public const string UserAgentIE9 = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)";

        public const string HeaderUserAgent = @"User-Agent";
        public const string HeaderAcceptEncoding = @"Accept-Encoding";

        private static readonly WebClientFactory _factory = new WebClientFactory();
        private static readonly object _syncLock = new object();

        /// <summary>
        /// WebClientFactory의 Singleton Instance입니다.
        /// </summary>
        public static WebClientFactory Instance {
            get { return _factory; }
        }

        public virtual WebClient CreateWebClient() {
            return CreateWebClientAsIE8();
        }

        /// <summary>
        /// <see cref="WebClient"/> 인스턴스를 빌드합니다. FireFox와 같은 형태의	 브라우저로 취급할 수 있도록 요청 Header를 설정합니다.
        /// </summary>
        /// <returns></returns>
        public virtual WebClient CreateWebClientAsFirefox() {
            var client = CreateWebClientCore();
#if !SILVERLIGHT
            client.Headers.Add(HeaderUserAgent, UserAgentFireFox);
#endif
            return client;
        }

        /// <summary>
        /// <see cref="WebClient"/> 인스턴스를 빌드합니다. 구글 크롬과 같은 형태의 브라우저로 취급할 수 있도록 요청 Header를 설정합니다.
        /// </summary>
        /// <returns></returns>
        public virtual WebClient CreateWebClientAsChrome() {
            var client = CreateWebClientCore();
#if !SILVERLIGHT
            client.Headers.Add(HeaderUserAgent, UserAgentChrome);
#endif
            return client;
        }

        /// <summary>
        /// <see cref="WebClient"/> 인스턴스를 빌드합니다. IE8와 같은 형태의 브라우저로 취급할 수 있도록 요청 Header를 설정합니다.
        /// </summary>
        /// <returns></returns>
        public virtual WebClient CreateWebClientAsIE8() {
            var client = CreateWebClientCore();

#if !SILVERLIGHT
            client.Headers.Add(HeaderUserAgent, UserAgentIE8);
#endif
            return client;
        }

        /// <summary>
        /// <see cref="WebClient"/> 인스턴스를 빌드합니다. IE9와 같은 형태의 브라우저로 취급할 수 있도록 요청 Header를 설정합니다.
        /// </summary>
        /// <returns></returns>
        public virtual WebClient CreateWebClientAsIE9() {
            var client = CreateWebClientCore();

#if !SILVERLIGHT
            client.Headers.Add(HeaderUserAgent, UserAgentIE9);
#endif
            return client;
        }

        /// <summary>
        /// 압축을 지원하는 WebClient를 생성합니다.
        /// </summary>
        /// <returns></returns>
        private static WebClient CreateWebClientCore() {
            var client = new WebClient();

#if !SILVERLIGHT
            client.Headers.Add("Accept", "*/*");
            client.Headers.Add(HeaderAcceptEncoding, "gzip,deflate");
#endif
            return client;
        }

        /// <summary>
        /// User-Agent를 IE8으로 하는 HttpWebRequest를 생성합니다.
        /// </summary>
        /// <param name="uriString">요청할 웹 주소</param>
        /// <returns></returns>
        public virtual HttpWebRequest CreateHttpRequest(string uriString) {
            return CreateHttpRequestAsIE8(uriString);
        }

        /// <summary>
        /// FireFox 3.6.8 에서 요청하는 HttpWebRequest를 생성합니다. (Mozilla 5.0 호환)
        /// </summary>
        /// <param name="uriString">요청할 웹 주소</param>
        /// <returns></returns>
        public virtual HttpWebRequest CreateHttpRequestAsFirefox(string uriString) {
            var request = CreateHttpRequestCore(new Uri(uriString));
#if !SILVERLIGHT
            request.UserAgent = UserAgentFireFox;
#endif
            return request;
        }

        /// <summary>
        /// Chrome 에서 요청하는 HttpWebRequest를 생성합니다. (Mozilla 5.0 호환)
        /// </summary>
        /// <param name="uriString">요청할 웹 주소</param>
        /// <returns></returns>
        public virtual HttpWebRequest CreateHttpRequestAsChrome(string uriString) {
            var request = CreateHttpRequestCore(new Uri(uriString));
#if !SILVERLIGHT
            request.UserAgent = UserAgentChrome;
#endif
            return request;
        }

        /// <summary>
        /// IE8 에서 요청하는 HttpWebRequest를 생성합니다. (Mozilla 5.0 호환)
        /// </summary>
        /// <param name="uriString">요청할 웹 주소</param>
        /// <returns></returns>
        public virtual HttpWebRequest CreateHttpRequestAsIE8(string uriString) {
            var request = CreateHttpRequestCore(new Uri(uriString));
#if !SILVERLIGHT
            request.UserAgent = UserAgentIE8;
#endif
            return request;
        }

        /// <summary>
        /// IE9 에서 요청하는 HttpWebRequest를 생성합니다. (Mozilla 5.0 호환)
        /// </summary>
        /// <param name="uriString">요청할 웹 주소</param>
        /// <returns></returns>
        public virtual HttpWebRequest CreateHttpRequestAsIE9(string uriString) {
            var request = CreateHttpRequestCore(new Uri(uriString));
#if !SILVERLIGHT
            request.UserAgent = UserAgentIE9;
#endif

            return request;
        }

        /// <summary>
        /// HttpWebRequest를 빌드합니다. 압축된 응답스트림을 받을 수 있도록 요청헤더에 gzip 가능을 표시합니다.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        protected virtual HttpWebRequest CreateHttpRequestCore(Uri uri) {
            uri.ShouldNotBeNull("uri");

            if(IsDebugEnabled)
                log.Debug("HttpWebRequest를 생성합니다. AbsoluteUri=[{0}]", uri.AbsoluteUri);

            var request = (HttpWebRequest)WebRequest.Create(uri);

#if !SILVERLIGHT
            request.Headers[HeaderAcceptEncoding] = "gzip,deflate";
            // request.Headers.Add(HeaderAcceptEncoding, "gzip,deflate");
            //request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            request.AllowAutoRedirect = true;
            request.AllowWriteStreamBuffering = true;
            request.KeepAlive = true;
#endif

            // request.MaximumAutomaticRedirections = 5;
            // request.MaximumResponseHeadersLength = 4096;
            // request.ReadWriteTimeout = 1000;
            // request.Timeout = 30000;

            return request;
        }
    }
}