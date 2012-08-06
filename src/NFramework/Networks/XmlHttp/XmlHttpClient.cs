using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using System.Xml;
using NSoft.NFramework.Parallelism.Tools;
using NSoft.NFramework.Tools;
using NSoft.NFramework.Xml;

namespace NSoft.NFramework.Networks {
    /// <summary>
    /// 서버와의 통신을 XML로 수행하는 통신 모듈 (Micosoft의 MSXML 의 IXmlHttp 를 구현한 것)
    /// </summary>
    /// <remarks>
    /// .NET Framework에서는 <see cref="HttpWebRequest"/>, <see cref="HttpWebResponse"/> class를 사용한다.
    /// <b>주의사항</b>
    /// ashx 같은 IHttpHandler에게 PostXml을 보내는 것은 상관없으나, 
    /// aspx 와 같은 Page에게 PostXml로 Xml 문자열을 보내게 되면, 보안 문제로 에러를 낼 수 있다. 
    /// 이를 방지하기 위해 두가지 방법이 있다.
    /// 1. aspx Page의 RequestValidate=false 로 설정하던가.
    /// 2. web.config에서 전체적으로 RequestValidate = false로 설정해야 한다.
    /// </remarks>
    public partial class XmlHttpClient {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// XmlHttp 통신시의 최소 Timeout (15000 msec)
        /// </summary>
        public const int MIN_TIMEOUT = 15000; // msec (15초)

        /// <summary>
        /// XmlHttp 통신시의 최대 Timeout. <see cref="System.Threading.Timeout.Infinite"/>를 사용
        /// </summary>
        public const int MAX_TIMEOUT = System.Threading.Timeout.Infinite;

        /// <summary>
        /// Http Post 방식 전송시의 Content Type
        /// </summary>
        public const string POST_CONTENT_TYPE = "application/x-www-form-urlencoded";

        /// <summary>
        /// URL String에서 사용되는 구분자들
        /// </summary>
        public static char[] UrlDelimeters = { '?', '=', '&' };

        private HttpWebRequest _request;

        private string _uri;
        private int _timeout = MAX_TIMEOUT;

        private readonly object _syncLock = new object();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="uri">통신할 서버 주소</param>
        public XmlHttpClient(string uri) {
            if(IsDebugEnabled)
                log.Debug("Initialize new instance of XmlHttpClient with uri string:" + uri);

            _uri = uri;
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="uri">통신할 서버 주소</param>
        /// <param name="isAsync">비동기 여부</param>
        /// <param name="timeout">제한 시간</param>
        /// <param name="userID">사용자 계정</param>
        /// <param name="password">사용자 비밀변호</param>
        public XmlHttpClient(string uri, bool isAsync = false, int timeout = MIN_TIMEOUT, string userID = null, string password = null)
            : this(uri) {
            IsAsync = isAsync;
            _timeout = timeout;
            UserId = userID;
            Password = password;
        }

        /// <summary>
        /// <see>System.Net.WebRequest</see> 객체를 나타낸다.
        /// 선행 조건으로 URL을 먼저 설정한다.
        /// </summary>
        /// <exception cref="InvalidOperationException"><see cref="Uri"/>값을 설정 안했을 시</exception>
        public WebRequest Request {
            get {
                if(_request == null) {
                    lock(_syncLock) {
                        if(_request == null) {
                            _uri.ShouldNotBeWhiteSpace("_uri");

                            var request = (HttpWebRequest)WebRequest.Create(_uri);
                            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                            Thread.MemoryBarrier();
                            _request = request;
                        }
                    }
                }
                return _request;
            }
        }

        /// <summary>
        /// XmlHttpClient로 통신할 서버 URI를 지정한다.
        /// </summary>
        public string Uri {
            get { return _uri; }
            set {
                value.ShouldNotBeWhiteSpace("Uri");
                _uri = value;
            }
        }

        /// <summary>
        /// 동기/비동기 통신 방식 지정
        /// </summary>
        public bool IsAsync { set; get; }

        /// <summary>
        /// 서버 접속 인증 방법이 Basic Authentication일 경우에는 사용자 아이디를 설정한다. 
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 서버 접속 인증 방법이 Basic Authentication일 경우에는 사용자 Password를 설정해 주어야 한다.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 서버와의 통신시의 통신 제한 시간 (단위: milliseconds)
        /// </summary>
        public int Timeout {
            get { return _timeout; }
            set { _timeout = Math.Max(MIN_TIMEOUT, value); }
        }

        /// <summary>
        /// Build NetworkCredential by account information.
        /// </summary>
        /// <returns></returns>
        protected NetworkCredential GetCredentials() {
            if(UserId != null && Password != null)
                return new NetworkCredential(UserId, Password);

            return new NetworkCredential();
        }

        /// <summary>
        /// 포스트 되는 DATA에 대해 URLEncode를 수행한다.
        /// </summary>
        /// <param name="payload">HttpPost되는 DATA</param>
        /// <returns></returns>
        protected virtual string EncodePayLoad(string payload) {
            if(payload.IsWhiteSpace())
                return payload;

            var encoded = new StringBuilder();

            var i = 0;

            while(i < payload.Length) {
                var j = payload.IndexOfAny(UrlDelimeters, i);

                if(j == -1) {
                    encoded.Append(HttpUtility.UrlEncode(payload.Substring(i, payload.Length - 1)));
                    break;
                }

                encoded.Append(HttpUtility.UrlEncode(payload.Substring(i, j - 1)));
                encoded.Append(payload.Substring(j, 1));

                i = j + 1;
            }

            return encoded.ToString();
        }

        /// <summary>
        /// Get방식의 통신으로 서버로부터 XML Stream 객체를 받환받는다.
        /// </summary>
        /// <returns></returns>
        protected string Get(Encoding enc = null) {
            using(var responseStream = GetStream()) {
                return responseStream.ToText(enc ?? XmlTool.XmlEncoding);
                // return StringTool.ToString(responseStream, enc);
            }
        }

        /// <summary>
        /// POST 방식으로 서버에 접속하여 XML 통신을 수행한다.
        /// </summary>
        /// <returns>반환받은 문자열 (UTF8 방식)</returns>
        protected virtual Stream GetStream() {
            Request.Credentials = GetCredentials();
            Request.Method = WebRequestMethods.Http.Get;

            if(IsAsync)
                return With.TryFunctionAsync(() => Request.GetResponseStreamAsync().Result);

            WebResponse response = Request.GetResponse();
            return response.GetResponseStream();
        }

        /// <summary>
        /// POST 방식으로 서버에 접속하여 응답 문자열을 반환 받는다.
        /// </summary>
        /// <param name="payload"></param>
        /// <param name="enc"></param>
        /// <returns></returns>
        protected string Post(string payload, Encoding enc = null) {
            using(var stream = PostStream(payload)) {
                return stream.ToText(enc ?? XmlTool.XmlEncoding);
            }
        }

        /// <summary>
        /// XmlDocument 객체의 InnerXml string을 POST 방식으로 전송하고 응답 문자열을 반환 받는다.
        /// </summary>
        /// <param name="document">보내고자하는 XML 객체</param>
        /// <returns>서버로부터 반환받은 결과 문자열</returns>
        protected string Post(XmlDocument document) {
            return Post(document.OuterXml);
        }

        /// <summary>
        /// POST 방식으로 서버에 접속하여 응답 Stream을 반환 받는다.
        /// </summary>
        /// <param name="payload">Url Encoding된 Parameters</param>
        /// <returns></returns>
        protected virtual Stream PostStream(string payload) {
            if(IsDebugEnabled)
                log.Debug("Post Stream... payload=[{0}]", payload);

            try {
                Request.Credentials = GetCredentials();
                Request.Method = WebRequestMethods.Http.Post;
                Request.ContentType = POST_CONTENT_TYPE;

                if(payload.IsWhiteSpace()) {
                    Request.ContentLength = 0;
                }
                else {
                    using(var stream = Request.GetRequestStream())
                    using(var sw = new StreamWriter(stream, XmlTool.XmlEncoding)) {
                        sw.Write(payload);
                        sw.Flush();
                    }
                }

                if(IsAsync) {
                    return With.TryFunctionAsync(() => Request.GetResponseStreamAsync().Result);
                }

                return Request.GetResponse().GetResponseStream();
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled)
                    log.ErrorException("스트림을 POST 방식으로 전송할 때 예외가 발생했습니다.", ex);

                throw;
            }
        }

        /// <summary>
        /// POST 방식으로 서버에 접속하여 응답 Stream을 반환 받는다.
        /// </summary>
        /// <param name="stream">서버에 전달할 내용</param>
        /// <returns></returns>
        protected virtual Stream PostStream(Stream stream) {
            stream.ShouldNotBeNull("stream");

            if(IsDebugEnabled)
                log.Debug("지정된 스트림을 전송합니다.");

            Request.Credentials = GetCredentials();
            Request.Method = WebRequestMethods.Http.Post;
            Request.ContentType = POST_CONTENT_TYPE;

            if(stream == null) {
                Request.ContentLength = 0;
            }
            if(stream != null) {
                byte[] payload = stream.ToBytes();

                using(var requestStream = Request.GetRequestStream())
                using(var bw = new BinaryWriter(requestStream, XmlTool.XmlEncoding)) {
                    bw.Write(payload);
                }
            }

            return IsAsync
                       ? With.TryFunctionAsync(() => Request.GetResponseStreamAsync().Result)
                       : Request.GetResponse().GetResponseStream();
        }

        /// <summary>
        /// XmlDocument 객체의 InnerXml string을 POST 방식으로 전송하고 응답 스트림을 반환 받는다.
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        protected Stream PostStream(XmlDocument document) {
            return PostStream(document.OuterXml);
        }

        /// <summary>
        /// GET 방식의 XML HTTP 통신
        /// </summary>
        /// <param name="enc">반환 문자열의 Encoding 방식</param>
        /// <returns>반환문자열</returns>
        public string GetText(Encoding enc = null) {
            return Get(enc);
        }

        /// <summary>
        /// GET 방식의 XML HTTP 통신
        /// </summary>
        /// <returns>XML문자열인 경우 XmlDocument 객체로 만들어서 반환</returns>
        public XmlDocument GetXml() {
            XmlDocument result = null;
            Stream stream = GetStream();

            if(stream != null)
                result = XmlTool.CreateXmlDocument(stream);

            return result;
        }

        /// <summary>
        /// POST 방식의 XML HTTP 통신. text/plain 값을 받는다.
        /// </summary>
        /// <param name="payload"></param>
        /// <param name="enc"></param>
        /// <returns></returns>
        public string PostText(string payload, Encoding enc = null) {
            return Post(payload, enc);
        }

        /// <summary>
        /// Post 방식의 XmlHttp 통신. XmlDocument를 받는다.
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public XmlDocument PostXml(XmlDocument document) {
            document.ShouldNotBeNull("requestDoc");
            Guard.Assert(document.IsValidDocument(), @"document is not valid xml document.");

            using(Stream stream = PostStream(document)) {
                return XmlTool.CreateXmlDocument(stream);
            }
        }

        /// <summary>
        /// Posting request xml document to server
        /// </summary>
        /// <param name="requestDoc"></param>
        /// <returns></returns>
        public XmlDoc Send(XmlDoc requestDoc) {
            requestDoc.ShouldNotBeNull("requestDoc");
            Guard.Assert(requestDoc.IsValidDocument(), @"requestDoc is not valid xml document.");

            XmlDoc responseDoc = null;

            try {
                responseDoc = (XmlDoc)PostXml(requestDoc);
            }
            catch(Exception ex) {
                if(responseDoc.IsValidDocument())
                    responseDoc.AddElement(responseDoc.Root, "ERROR", ex.Message);

                else {
                    if(log.IsErrorEnabled)
                        log.ErrorException("Error occurred in Send() method.", ex);

                    throw;
                }
            }

            return responseDoc;
        }
    }
}