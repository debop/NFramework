using System.IO;
using System.Net;
using System.Text;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Networks {
    /// <summary>
    /// HTTP Protocal을 이용한 통신을 수행하기 위한 Utility Class
    /// </summary>
    public static class HttpTool {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 주어진 URL에 HTTP GET을 수행한다.
        /// </summary>
        /// <param name="url">Server URL</param>
        /// <param name="enc">Encoding Type</param>
        /// <param name="timeout">Connection Timeout (milliseconds)</param>
        /// <param name="userId">UserId for Authentication</param>
        /// <param name="password">Password for Authentication</param>
        /// <returns></returns>
        public static string GetString(string url, Encoding enc = null, int timeout = HttpConsts.HTTP_DEFAULT_TIMEOUT,
                                       string userId = null, string password = null) {
            url.ShouldNotBeWhiteSpace("url");

            enc = enc ?? StringTool.DefaultEncoding;

            if(IsDebugEnabled)
                log.Debug("Http GetString... url=[{0}], enc=[{1}], timeout=[{2}], userId=[{3}], password=[{4}]", url, enc, timeout,
                          userId, password);

            var httpClient = new HttpClient(url, timeout, userId, password);
            var result = httpClient.Get();

            if(IsDebugEnabled)
                log.Debug("HTTP GET 방식의 반환 문자열=[{0}]", result.EllipsisChar(255));

            return result;
        }

        /// <summary>
        /// 지정된 Web 주소로부터 Http Get을 이용하여 정보를 다운 받는다.
        /// </summary>
        /// <param name="url">서버 주소</param>
        /// <param name="timeout">제한 시간</param>
        /// <param name="userId">인증을 위한 사용자 Id</param>
        /// <param name="password">인증을 위한 비밀번호</param>
        /// <returns>응답 개체 (<c>WebResponse</c> 개체)</returns>
        public static WebResponse GetResponse(string url, int timeout = HttpConsts.HTTP_DEFAULT_TIMEOUT, string userId = null,
                                              string password = null) {
            if(IsDebugEnabled)
                log.Debug("Get HttpResponse... url=[{0}], timeout=[{1}], userId=[{2}], password=[{3}]", url, timeout, userId, password);

            var httpClient = new HttpClient(url, timeout, userId, password);
            return httpClient.GetResponse(WebRequestMethods.Http.Get);
        }

        /// <summary>
        /// 지정된 Web 주소로부터 Http Post을 이용하여 정보를 전달하고, 결과 정보를 얻는다.
        /// </summary>
        /// <param name="url">서버 주소</param>
        /// <param name="payload">POST 될 데이타 (형식 : PARAM1=VALUE1&amp;PARAM2=VALUE2&amp;PARAM3=VALUE3)</param>
        /// <param name="enc">인코딩 방식</param>
        /// <param name="timeout">제한 시간</param>
        /// <param name="userId">인증을 위한 사용자 Id</param>
        /// <param name="password">인증을 위한 비밀번호</param>
        /// <returns>응답 개체 (<c>WebResponse</c> 개체)</returns>
        public static string PostString(string url,
                                        string payload,
                                        Encoding enc = null,
                                        int timeout = HttpConsts.HTTP_DEFAULT_TIMEOUT,
                                        string userId = null,
                                        string password = null) {
            url.ShouldNotBeWhiteSpace("url");
            enc = enc ?? StringTool.DefaultEncoding;

            if(IsDebugEnabled)
                log.Debug("Post String... url=[{0}], timeout=[{1}], userId=[{2}], password=[{3}]", url, timeout, userId, password);

            var result = new HttpClient(url, timeout, userId, password).Post(payload, enc);

            if(IsDebugEnabled)
                log.Debug("Post result string=[{0}]", result.EllipsisChar(255));

            return result;
        }

        /// <summary>
        /// 가장 일반적으로 사용할 함수, 서버에 HTTP로 접속하여 Response를 얻는다.
        /// </summary>
        /// <param name="method">전송 방법 (<see cref="WebRequestMethods.Http"/>참조)</param>
        /// <param name="url">서버 주소</param>
        /// <param name="timeout">제한 시간</param>
        /// <param name="userId">UserId</param>
        /// <param name="password">Password</param>
        /// <returns>결과 <c>WebResponse</c> 인스턴스 개체</returns>
        /// <remarks>WebResponse는 사용한 후 꼭 Close를 호출해야 한다.</remarks>
        public static WebResponse GetResponse(string method,
                                              string url,
                                              int timeout = HttpConsts.HTTP_DEFAULT_TIMEOUT,
                                              string userId = null,
                                              string password = null) {
            method.ShouldNotBeWhiteSpace("method");
            url.ShouldNotBeWhiteSpace("url");

            if(IsDebugEnabled)
                log.Debug("Get HttpResponse... method=[{0}], url=[{1}], timeout=[{2}], userId=[{3}], password=[{4}]", method, url,
                          timeout, userId, password);

            var httpClient = new HttpClient(url, timeout, userId, password);
            return httpClient.GetResponse(method);
        }

        /// <summary>
        /// 주어진 주소에서 Web을 통해 파일을 다운로드 받는다.
        /// </summary>
        /// <param name="address">Web 주소</param>
        /// <param name="localFullFileName">다운 받아 저자할 로칼 파일 전체 경로</param>
        /// <returns>성공여부</returns>
        /// <exception cref="WebException">address 주소 잘못 등</exception>
        public static bool SimpleDownloadFile(string address, string localFullFileName) {
            address.ShouldNotBeWhiteSpace("address");
            localFullFileName.ShouldNotBeWhiteSpace("localFullFileName");

            if(IsDebugEnabled)
                log.Debug("파일을 다운로드 받습니다. address=[{0}], localFile=[{1}]", address, localFullFileName);

            using(var client = new WebClient()) {
                client.DownloadFile(address, localFullFileName);
                return File.Exists(localFullFileName);
            }
        }

        /// <summary>
        /// 웹으로부터 Data를 다운로드 받는다.
        /// </summary>
        /// <param name="address">주소</param>
        /// <returns>응답받은 Data, 일차원 바이트 배열입니다.</returns>
        public static byte[] SimpleDownloadData(string address) {
            address.ShouldNotBeWhiteSpace("address");

            if(IsDebugEnabled)
                log.Debug("데이타를 다운로드 받습니다. address=[{0}]", address);

            using(var client = new WebClient()) {
                return client.DownloadData(address);
            }
        }

        /// <summary>
        /// 지정된 주소로 파일을 전송합니다.
        /// </summary>
        /// <param name="address">전송할 URL 주소</param>
        /// <param name="method">전송 방법 (GET, POST, PUT...)</param>
        /// <param name="filename">전송활 파일 경로</param>
        /// <returns>전송 결과</returns>
        public static byte[] SimpleUploadFile(string address, string filename, string method = WebRequestMethods.Http.Post) {
            address.ShouldNotBeWhiteSpace("address");
            filename.ShouldNotBeWhiteSpace("filename");
            Guard.Assert<FileNotFoundException>(File.Exists(filename), "File not found. filename=[{0}]", filename);

            using(var client = new WebClient()) {
                return client.UploadFile(address, method ?? WebRequestMethods.Http.Post, filename);
            }
        }
    }
}