using System;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Net;
using System.Security;
using System.Text;
using System.Web;
using NSoft.NFramework.IO;
using NSoft.NFramework.Reflections;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Networks {
    /// <summary>
    /// Http 통신을 수행하는 Client Class
    /// </summary>
    public class HttpClient {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private Uri _baseUri;
        private int _timeOut = HttpConsts.HTTP_DEFAULT_TIMEOUT;
        private readonly string _userId;
        private string _passwd;
        private HttpWebRequest _request;
        private ICredentials _credentials;

        // private NameValueCollection m_RequestParameters;
        private WebHeaderCollection _requestHeaders;
        private WebHeaderCollection _responseHeaders;

        /// <summary>
        /// Proxy가 없는 경우에는 <see cref="System.Net.WebRequest.DefaultWebProxy"/> 사용한다.
        /// </summary>
        private IWebProxy _webProxy = WebRequest.DefaultWebProxy;

        /// <summary>
        /// Initialize a new instance of HttpClient.
        /// </summary>
        public HttpClient() {
            _baseUri = new Uri(HttpConsts.HTTP_LOCALHOST);
        }

        /// <summary>
        /// Initialize a new instance of HttpClient with baseUri.
        /// </summary>
        /// <param name="baseUri">서버주소</param>
        public HttpClient(Uri baseUri) {
            _baseUri = baseUri;
        }

        /// <summary>
        /// Initialize a new instance of HttpClient with baseUri, timeout, user id and password
        /// </summary>
        /// <param name="baseUri">서버주소</param>
        /// <param name="timeout">통신 timeout (msec 단위)</param>
        /// <param name="userId">사용자 Id</param>
        /// <param name="passwd">비밀번호</param>
        public HttpClient(Uri baseUri, int timeout = HttpConsts.HTTP_DEFAULT_TIMEOUT, string userId = null, string passwd = null)
            : this(baseUri) {
            _timeOut = timeout;
            _userId = userId;
            _passwd = passwd;
        }

        /// <summary>
        /// Initialize a new instance of HttpClient with uri string.
        /// </summary>
        /// <param name="baseUriString">uri string</param>
        public HttpClient(string baseUriString)
            : this(new Uri(baseUriString)) {}

        /// <summary>
        /// Initialize a new instance of HttpClient with uri, timeout, user id and password
        /// </summary>
        /// <param name="baseUriString">uri string</param>
        /// <param name="timeout">time for http communication.</param>
        /// <param name="userId">user id</param>
        /// <param name="passwd">password</param>
        public HttpClient(string baseUriString, int timeout = HttpConsts.HTTP_DEFAULT_TIMEOUT, string userId = null,
                          string passwd = null)
            : this(new Uri(baseUriString), timeout, userId, passwd) {}

        /// <summary>
        /// Base <see cref="Uri"/>
        /// </summary>
        public Uri BaseUri {
            get { return _baseUri; }
            set { _baseUri = value; }
        }

        /// <summary>
        /// Base <see cref="Uri"/> String
        /// </summary>
        public string BaseUriString {
            get { return _baseUri != null ? _baseUri.ToString() : string.Empty; }
            set { _baseUri = value.IsEmpty() ? null : new Uri(value); }
        }

        /// <summary>
        /// 요청 제한 시간 (단위 : millisecond)
        /// </summary>
        public int Timeout {
            get { return _timeOut; }
            set {
                if(_timeOut != value) {
                    _timeOut = (value == System.Threading.Timeout.Infinite)
                                   ? value
                                   : Math.Max(HttpConsts.HTTP_MIN_TIMEOUT, value);
                }
            }
        }

        /// <summary>
        /// 기본 인증 적용시 사용되는 UserId
        /// </summary>
        public string UserId {
            get { return _userId; }

            set {
                if(_userId != value) {
                    _passwd = value;
                    OnChanged();
                }
            }
        }

        /// <summary>
        /// 기본 인증 적용시 사용되는 비밀번호
        /// </summary>
        public string Password {
            get { return _passwd; }

            set {
                if(_passwd != value) {
                    _passwd = value;
                    OnChanged();
                }
            }
        }

        /// <summary>
        /// WebRequest 객체
        /// </summary>
        public HttpWebRequest Request {
            get { return _request = _request ?? BuildRequest(); }
        }

        /// <summary>
        /// 웹 클라이언트 인증을 위한 자격 증명을 검색할 수 있는 기본 인증 인터페이스를 제공합니다.
        /// </summary>
        public ICredentials Credentials {
            get { return GetCredentials(); }
            set {
                if(value != null && !value.Equals(_credentials)) {
                    DisposeRequest();
                    _credentials = value;
                }
            }
        }

        /// <summary>
        /// Proxy 서버 정보를 가지는 개체
        /// </summary>
        /// <value></value>
        public IWebProxy WebProxy {
            get { return _webProxy; }
            set {
                if(value != null && !value.Equals(_webProxy)) {
                    DisposeRequest();
                    _webProxy = value;
                }
            }
        }

        /// <summary>
        /// 요청 헤데 정보 컬렉션
        /// </summary>
        public WebHeaderCollection RequestHeaders {
            get { return _responseHeaders = _responseHeaders ?? new WebHeaderCollection(); }
        }

        /// <summary>
        /// 응답 헤더 정보 컬렉션
        /// </summary>
        /// <value></value>
        public WebHeaderCollection ResponseHeaders {
            get { return _responseHeaders; }
        }

        private void OnChanged() {
            if(_userId != null && _passwd != null)
                _credentials = new NetworkCredential(_userId, _passwd);
        }

        /// <summary>
        /// 내부 Request 개체를 해제한다.
        /// </summary>
        private void DisposeRequest() {
            if(IsDebugEnabled)
                log.Debug("Disposing instance of HttpWebRequest is starting...");

            if(_request == null)
                return;

            try {
                _request.Abort();
            }
            catch(Exception ex) {
                if(log.IsWarnEnabled)
                    log.Warn(ex);
            }
            finally {
                _request = null;

                if(IsDebugEnabled)
                    log.Debug("Instance of HttpWebRequest is disposed.");
            }
        }

        private ICredentials GetCredentials() {
            if(_userId != null && _passwd != null)
                return _credentials = _credentials ?? new NetworkCredential(_userId, _passwd);

            return _credentials;
        }

        /// <summary>
        /// Response 정보를 byte array로 변환한다.
        /// </summary>
        /// <remarks>
        /// 길이가 아주 길고, 실제 크기를 알 수 없을 때 Response 개체의 stream을 처리하는 로직이 있음.
        /// </remarks>
        /// <param name="response"></param>
        /// <returns></returns>
        public static byte[] ResponseToArray(WebResponse response) {
            response.ShouldNotBeNull("response");

            if(IsDebugEnabled)
                log.Debug("Convert content of WebResponse to byte array...");

            using(var stream = response.GetResponseStream()) {
                stream.ShouldNotBeNull("ResponseStream");

                // 길이를 알 수 없을 때 사용한다.
                var length = response.ContentLength;
                if(length == 0)
                    return new byte[0];

                var flag = false;
                // 실제 크기를 알지 못할 때, 기본 크기로 계속 늘리기 위해
                //
                if(length == -1) {
                    flag = true;
                    length = HttpConsts.DEFAULT_BUFFER_LENGTH;
                }

                var buffer = new byte[(uint)(int)length];
                var offset = 0;

                int readCount;

                while((readCount = stream.Read(buffer, offset, (int)length - offset)) > 0) {
                    offset += readCount;

                    if(flag && (offset == length)) {
                        length += HttpConsts.DEFAULT_BUFFER_LENGTH;
                        var buffer2 = new byte[(uint)(int)length];
                        Buffer.BlockCopy(buffer, 0, buffer2, 0, offset);

                        buffer = buffer2;
                    }
                }

                //do
                //{
                //    readCount = stream.Read(buffer, offset, (int) length - offset);
                //    offset += readCount;

                //    // stream의 크기가 기본 크기보다 더 크므로 버퍼의 크기를 늘린다.
                //    if(flag && (offset == length))
                //    {
                //        length += HttpConsts.DEFAULT_BUFFER_LENGTH;
                //        var buffer2 = new byte[(uint) (int) length];
                //        Buffer.BlockCopy(buffer, 0, buffer2, 0, offset);

                //        buffer = buffer2;
                //    }
                //} while(readCount != 0);

                stream.Close();

                // 실제 크기를 알 수 없으므로 남아있는 여분의 Array를 제거한다.
                if(flag) {
                    var tmpBuffer = new byte[(uint)offset];
                    Buffer.BlockCopy(buffer, 0, tmpBuffer, 0, offset);
                    buffer = tmpBuffer;
                }

                return buffer;
            }
        }

        /// <summary>
        /// Http 메소드를 실행하기 위해 Http 요청 객체(<see cref="HttpWebRequest"/>)의 헤더 부분을 조정한다.
        /// </summary>
        /// <param name="request"></param>
        private void PrepareRequestHeaders(WebRequest request) {
            if(_requestHeaders != null && request is HttpWebRequest) {
                string accept = _requestHeaders["Accept"];
                string connection = _requestHeaders["Connection"];
                string contentType = _requestHeaders["Content-Type"];
                string expect = _requestHeaders["Expect"];
                string referer = _requestHeaders["Referer"];
                string userAgent = _requestHeaders["User-Agent"];

                request.Headers = _requestHeaders;

                if(accept.IsNotEmpty()) {
                    _requestHeaders.Remove("Accept");
                    ((HttpWebRequest)request).Accept = accept;
                }
                if(connection.IsNotEmpty()) {
                    _requestHeaders.Remove("Connection");
                    ((HttpWebRequest)request).Connection = connection;
                }
                if(contentType.IsNotEmpty()) {
                    _requestHeaders.Remove("Content-Type");
                    (request).ContentType = contentType;
                }
                if(expect.IsNotEmpty()) {
                    _requestHeaders.Remove("Expect");
                    ((HttpWebRequest)request).Expect = expect;
                }
                if(referer.IsNotEmpty()) {
                    _requestHeaders.Remove("Referer");
                    ((HttpWebRequest)request).Referer = referer;
                }
                if(userAgent.IsNotEmpty()) {
                    _requestHeaders.Remove("User-Agent");
                    ((HttpWebRequest)request).UserAgent = userAgent;
                }
            }
        }

        /// <summary>
        /// 기본 Uri와 script path를 가지고 완전한 Uri를 만든다.
        /// </summary>
        /// <param name="relativePath"></param>
        /// <returns></returns>
        private Uri GetUri(string relativePath) {
            Uri uri;

            try {
                uri = (_baseUri != null) ? new Uri(_baseUri, relativePath) : new Uri(relativePath);

                //				if (m_RequestParameters != null)
                //				{
                //					StringBuilder sb = new StringBuilder();
                //					string s = string.Empty;
                //
                //					foreach (DictionaryEntry de in m_RequestParameters)
                //					{
                //						sb.Append(s + de.Key + "=" + de.Value);
                //						
                //						if( s.Length == 0)
                //							s = "&";
                //					}
                //					UriBuilder ub = new UriBuilder(uri);
                //					ub.Query = sb.ToString();
                //					uri = ub.Uri;
                //				}
            }
            catch(UriFormatException ex) {
                if(log.IsWarnEnabled)
                    log.Warn(ex);

                uri = new Uri(Path.GetFullPath(relativePath));
            }

            if(IsDebugEnabled)
                log.Debug("Get uri from relativePath=[{0}]. uri.AbsolutePath=[{1}]", relativePath, uri.AbsolutePath);

            return uri;
        }

        private Uri GetUri(Uri relativeUri) {
            return GetUri(relativeUri.AbsolutePath);
        }

        /// <summary>
        /// 요청 객체를 생성한다.
        /// </summary>
        /// <returns></returns>
        private HttpWebRequest BuildRequest() {
            if(IsDebugEnabled)
                log.Debug("Build HttpWebRequest...");

            _baseUri.ShouldNotBeNull("_baseUri");

            var request = (HttpWebRequest)WebRequest.Create(_baseUri);

            request.AllowAutoRedirect = true;
            request.AllowWriteStreamBuffering = true;
            request.KeepAlive = true;

            request.Timeout = _timeOut;
            request.Proxy = WebProxy;
            request.Credentials = Credentials;

            return request;
        }

        /// <summary>
        /// 응답 객체를 반환한다.
        /// </summary>
        /// <param name="method"><see cref="WebRequestMethods.Http"/> 참고</param>
        /// <returns></returns>
        /// <remarks>WebResponse는 사용한 후 꼭 Close를 호출해야 한다.</remarks>
        public WebResponse GetResponse(string method = "Get") {
            if(IsDebugEnabled)
                log.Debug("Get WebResponse with method=[{0}]", method);

            HttpWebRequest request = BuildRequest();
            request.Method = method;

            WebResponse response = Request.GetResponse();
            _request = null;

            return response;
        }

        /// <summary>
        /// 응답 Header 내용을 반환한다.
        /// </summary>
        /// <returns></returns>
        public WebHeaderCollection Head() {
            if(IsDebugEnabled)
                log.Debug("Execute Http HEAD method.");

            using(WebResponse response = GetResponse(WebRequestMethods.Http.Head))
                _responseHeaders = (response != null) ? response.Headers : new WebHeaderCollection();

            if(IsDebugEnabled)
                log.Debug("Execute Http HEAD method is SUCCESS. headers=[{0}]", _responseHeaders.CollectionToString());

            return _responseHeaders;
        }

        /// <summary>
        /// URL정보를 기초로 HTTP GET 을 수행한다. 
        /// </summary>
        /// <returns>응답 문자열</returns>
        public string Get() {
            if(IsDebugEnabled)
                log.Debug("Execute Http GET method.");

            var result = string.Empty;
            using(var response = (HttpWebResponse)GetResponse(WebRequestMethods.Http.Get)) {
                if(response != null) {
                    using(var responseStream = response.GetResponseStream()) {
                        result = responseStream.ToText();
                    }
                }
            }

            if(IsDebugEnabled)
                log.Debug("Http GET result=[{0}]", result.EllipsisChar(80));

            return result;
        }

        /// <summary>
        /// 지정된 서버에 DATA를 POST 방식으로 전송한다.
        /// </summary>
        /// <param name="inputs">전송 데이타</param>
        /// <param name="enc">인코딩 방식</param>
        /// <returns>응답 결과 문자열</returns>
        public string Post(NameValueCollection inputs, Encoding enc = null) {
            enc = enc ?? StringTool.DefaultEncoding;

            var builder = new StringBuilder();
            var delimeter = string.Empty;

            foreach(string name in inputs.AllKeys) {
                builder.Append(delimeter);

                builder.Append(name);
                builder.Append("=");
                builder.Append(inputs[name]);

                if(delimeter.Length == 0)
                    delimeter = "&";
            }

            return Post(builder.ToString(), enc);
        }

        /// <summary>
        /// 주어진 URL를 기초로 Http Post 를 수행한다.
        /// ASP.NET 서버인 경우 web.config의 Request/Response Encoding 정보를 잘 보고 해야 한다.
        /// </summary>
        /// <param name="payload">전송할 DATA (ex PARAM1=VALUE1&amp;PARAM2=VALUE2)</param>
        /// <param name="enc">전송할 DATA의 Encoding 방식</param>
        /// <returns>응답 문자열</returns>
        public string Post(string payload, Encoding enc = null) {
            enc = enc ?? StringTool.DefaultEncoding;

            if(IsDebugEnabled)
                log.Debug("Execute Http POST method. payload=[{0}], encoding=[{1}]", payload, enc.BodyName);

            string result;

            try {
                Request.Method = WebRequestMethods.Http.Post.ToUpper();

                // request 로 보낼 때 문자 인코딩 정보도 같이 보내야 한다.
                Request.ContentType = HttpConsts.HTTP_POST_CONTENT_TYPE + "; charset=" + enc.WebName;
                Request.ContentLength = 0;

                if(payload.IsNotEmpty()) {
                    byte[] data = enc.GetBytes(payload);
                    Request.ContentLength = data.Length;


                    var requestStream = Request.GetRequestStream();
                    requestStream.Write(data, 0, data.Length);
                    requestStream.Close();
                }

                using(var response = (HttpWebResponse)Request.GetResponse())
                using(var responseStream = response.GetResponseStream()) {
                    result = responseStream.ToText();
                }
            }
            finally {
                _request = null;
            }

            if(IsDebugEnabled)
                log.Debug("Http POST result=[{0}]", result.EllipsisChar(80));

            return result;
        }

        /// <summary>
        /// 해당주소에서 Data를 받습니다.
        /// </summary>
        /// <returns></returns>
        public byte[] DownloadData() {
            try {
                _responseHeaders = null;

                PrepareRequestHeaders(Request);
                WebResponse response = Request.GetResponse();
                _responseHeaders = response.Headers;

                return ResponseToArray(response);
            }
            catch(Exception e) {
                if(log.IsErrorEnabled)
                    log.ErrorException("데이타 다운로드에 실패했습니다", e);

                throw;
            }
        }

        /// <summary>
        /// 지정한 주소의 내용을 받아서 파일로 저장한다.
        /// </summary>
        /// <param name="address">서버 주소</param>
        /// <param name="filename">로칼 파일 경로</param>
        /// <returns></returns>
        public bool DownloadFile(string address, string filename) {
            if(IsDebugEnabled)
                log.Debug("Download a file from the specified address. address=[{0}], filename=[{1}]", address, filename);

            bool downloaded = false;
            try {
                _responseHeaders = null;
                WebRequest request = WebRequest.Create(GetUri(address));
                request.Credentials = Credentials;
                request.Proxy = WebProxy;

                WebResponse response = request.GetResponse();

                _responseHeaders = response.Headers;

                // 적당한 버퍼 크기를 결정한다.
                var length = response.ContentLength;

                if(length == -1 || length > int.MaxValue) {
                    length = int.MaxValue;
                }

                var buffer = new byte[(uint)Math.Min(HttpConsts.DEFAULT_BUFFER_LENGTH, (int)length)];

                using(var stream = response.GetResponseStream())
                using(var bs = FileTool.GetBufferedFileStream(filename, FileOpenMode.Write)) {
                    int readCount;
                    do {
                        readCount = stream.Read(buffer, 0, buffer.Length);
                        if(readCount > 0)
                            bs.Write(buffer, 0, readCount);
                    } while(readCount > 0);

                    bs.Flush();
                }

                if(IsDebugEnabled)
                    log.Debug("Download File is SUCCESS!!! filename=[{0}]", filename);

                downloaded = true;
                return true;
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled) {
                    log.Error("Fail to download a filename=[{0}]", filename);
                    log.Error(ex);
                }

                throw;
            }
            finally {
                // 다운로드에 실패하고, 파일이 존재한다면, 완전한 파일이 아니므로 삭제해버린다.
                //
                if(downloaded == false && filename.FileExists())
                    filename.DeleteFile(false);
            }
        }

        /// <summary>
        /// Client의 지정된 파일을 서버로 전송한다.
        /// </summary>
        /// <param name="filename">서버로 전송할 파일명</param>
        /// <param name="method">Http communication method. (GET, POST, PUT...)</param>
        /// <returns>파일 전송 결과 Data</returns>
        public byte[] UploadFile(string filename, string method = "POST") {
            if(File.Exists(filename))
                throw new FileNotFoundException("File not found", filename);

            if(IsDebugEnabled)
                log.Debug("Upalod the local file. filename=[{0}], http method=[{1}]", filename, method);

            BufferedStream bufferedStream = null;
            WebResponse response = null;

            try {
                filename = Path.GetFullPath(filename);
                string boundary = string.Concat(HttpConsts.HTTP_CONTENT_BOUNDARY, DateTime.Now.Ticks.ToString("x"));
                string contentType = Request.Headers["Content-Type"];

                if(contentType.IsWhiteSpace())
                    contentType = HttpConsts.HTTP_DEFAULT_FILE_CONTENT_TYPE;
                else if(contentType.ToLower(CultureInfo.InvariantCulture).StartsWith("multipart"))
                    throw new HttpException("HttpClient.Request has invalid content-type.");

                Request.Headers["Content-Type"] = HttpConsts.HTTP_FILE_CONTENT_TYPE + "; boundary=" + boundary;

                bufferedStream = FileTool.GetBufferedFileStream(filename, FileOpenMode.Read);

                var fileData = new StringBuilder();

                fileData.Append("--");
                fileData.Append(boundary).Append("\r\n");
                fileData.Append("Content-Disposition: form-data; name=\"file\";");
                fileData.AppendFormat("filename=\"{0}\"\r\n", Path.GetFileName(filename));
                fileData.AppendFormat("Conent-Type:{0}\r\n\r\n", contentType);

                byte[] fileBytes = Encoding.UTF8.GetBytes(fileData.ToString());
                byte[] boundaryBytes = Encoding.ASCII.GetBytes("\r\n" + boundary + "\r\n");

                long fileLength = bufferedStream.Length;

                try {
                    Request.ContentLength = fileLength + fileBytes.Length + boundaryBytes.Length;
                }
                catch(Exception ex) {
                    if(log.IsWarnEnabled)
                        log.WarnException("파일 크기 계산에 실패했습니다.", ex);
                }

                var buffer = new byte[(uint)Math.Min(8192, (int)fileLength)];
                var requestStream = Request.GetRequestStream();

                try {
                    // 파일 내용 읽어서 RequestStream에 넣기

                    int readCount;
                    requestStream.Write(fileBytes, 0, fileBytes.Length);
                    do {
                        readCount = bufferedStream.Read(buffer, 0, buffer.Length);

                        if(readCount != 0)
                            requestStream.Write(buffer, 0, readCount);
                    } while(readCount != 0);

                    requestStream.Write(boundaryBytes, 0, boundaryBytes.Length);
                }
                finally {
                    requestStream.Close();
                }
                bufferedStream.Close();

                // 파일 전송
                response = Request.GetResponse();
                _requestHeaders = response.Headers;
                return ResponseToArray(response);
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled) {
                    log.Error("Fail to upload a local file. filename=[{0}]" + filename);
                    log.Error(ex);
                }

                if(ex is HttpException || ex is WebException || ex is SecurityException)
                    throw;
                else
                    throw new HttpException("Error in UploadFile.", ex);
            }
            finally {
                if(bufferedStream != null)
                    bufferedStream.Close();

                if(response != null)
                    response.Close();

                _request = null;
            }
        }
    }
}