using System.Xml;
using NSoft.NFramework.Xml;

namespace NSoft.NFramework.Networks {
    /// <summary>
    /// XML 형식의 정보를 HTTP Protocol을 통해 통신 할 때 사용되는 Utility class.
    /// </summary>
    public static class XmlHttpTool {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// HTTP GET 방식으로 서버로부터 XML 정보를 얻는다.
        /// </summary>
        /// <param name="uri">서버 주소</param>
        /// <param name="isAsync">비동기 여부</param>
        /// <param name="timeoutMilliseconds">통신 Timeout (단위: milliseconds)</param>
        /// <param name="userId">계정 Id</param>
        /// <param name="password">계정 비밀번호</param>
        /// <returns>응답 내용 (XML 문자열)</returns>
        public static string GetText(string uri,
                                     bool isAsync = false,
                                     int timeoutMilliseconds = System.Threading.Timeout.Infinite,
                                     string userId = null,
                                     string password = null) {
            uri.ShouldNotBeWhiteSpace("uri");

            if(IsDebugEnabled)
                log.Debug(
                    "XmlHttp 통신... 일반 Text 형식으로 GET... uri=[{0}], isAsync=[{1}], timeoutMilliseconds=[{2}], userId=[{3}], password=[{4}]",
                    uri, isAsync, timeoutMilliseconds, userId, password);

            var xmlHttp = new XmlHttpClient(uri, isAsync, timeoutMilliseconds, userId, password);
            return xmlHttp.GetText();
        }

        /// <summary>
        /// HTTP Get 방식으로 응답을 받아 <see cref="XmlDocument"/> 의 인스턴스로 빌드하여 반환한다.
        /// </summary>
        /// <param name="uri">응답받을 서버 주소</param>
        /// <param name="isAsync">비동기 여부</param>
        /// <param name="timeoutMilliseconds">통신 Timeout</param>
        /// <param name="userId">계정 Id</param>
        /// <param name="password">계정 비밀번호</param>
        /// <returns>응답정보를 담은 <see cref="XmlDocument"/></returns>
        public static XmlDocument GetXml(string uri,
                                         bool isAsync = false,
                                         int timeoutMilliseconds = System.Threading.Timeout.Infinite,
                                         string userId = null,
                                         string password = null) {
            log.Debug(
                "XmlHttp 통신... XmlDocument 형식으로 GET... uri=[{0}], isAsync=[{1}], timeoutMilliseconds=[{2}], userId=[{3}], password=[{4}]",
                uri, isAsync, timeoutMilliseconds, userId, password);

            var xmlHttp = new XmlHttpClient(uri, isAsync, timeoutMilliseconds, userId, password);
            return xmlHttp.GetXml();
        }

        /// <summary>
        /// HTTP Post 방식으로 응답을 받아 문자열로 반환한다.
        /// </summary>
        /// <param name="uri">응답받을 서버 주소</param>
        /// <param name="payload">응답 요청시의 Parameter 정보</param>
        /// <param name="isAsync">비동기 여부</param>
        /// <param name="timeoutMilliseconds">통신 Timeout</param>
        /// <param name="userId">계정 Id</param>
        /// <param name="password">계정 비밀번호</param>
        /// <returns>응답정보를 담은 문자열</returns>
        public static string PostText(string uri,
                                      string payload,
                                      bool isAsync = false,
                                      int timeoutMilliseconds = System.Threading.Timeout.Infinite,
                                      string userId = null,
                                      string password = null) {
            uri.ShouldNotBeWhiteSpace("uri");

            if(IsDebugEnabled)
                log.Debug(
                    "PostText... uri=[{0}], isAsync=[{1}], timeoutMilliseconds=[{2}], userId=[{3}], password=[{4}], payload=[{5}]",
                    uri, isAsync, timeoutMilliseconds, userId, password, payload);

            var xmlHttp = new XmlHttpClient(uri, isAsync, timeoutMilliseconds, userId, password);
            return xmlHttp.PostText(payload);
        }

        /// <summary>
        /// HTTP POST 방식으로 응답을 받아 <see cref="XmlDocument"/> 의 인스턴스로 빌드하여 반환한다.
        /// </summary>
        /// <param name="uri">응답받을 서버 주소</param>
        /// <param name="document">응답 요청시의 Form Parameter 정보</param>
        /// <param name="isAsync">비동기 여부</param>
        /// <param name="timeoutMilliseconds">통신 Timeout</param>
        /// <param name="userId">계정 Id</param>
        /// <param name="password">계정 비밀번호</param>
        /// <returns>응답정보를 담은 <see cref="XmlDocument"/></returns>
        public static XmlDocument PostXml(string uri,
                                          XmlDocument document,
                                          bool isAsync = false,
                                          int timeoutMilliseconds = System.Threading.Timeout.Infinite,
                                          string userId = null,
                                          string password = null) {
            uri.ShouldNotBeWhiteSpace("uri");
            document.ShouldNotBeNull("document");
            Guard.Assert(document.IsValidDocument(), "document is not valid xml document.");

            if(IsDebugEnabled) {
                log.Debug("PostXml... uri=[{0}], isAsync=[{1}], timeout=[{2}], userId=[{3}], password=[{4}], document=[{5}]",
                          uri, isAsync, timeoutMilliseconds, userId, password, document);

                if(document != null)
                    log.Debug("PostXml FormData=[{0}]", document.InnerXml);
            }

            var xmlHttp = new XmlHttpClient(uri, isAsync, timeoutMilliseconds, userId, password);
            return xmlHttp.PostXml(document);
        }

        /// <summary>
        /// RealWeb XML HTTP 통신 모듈 설정이다.
        /// </summary>
        /// <param name="uri">서버 URI</param>
        /// <param name="document">요청용 RclXmlDocument</param>
        /// <param name="isAsync">비동기 통신 여부</param>
        /// <param name="timeoutMilliseconds">통신 제한시간 (default : System.Threading.Timeout.Infinite)</param>
        /// <param name="userId"></param>
        /// <param name="password"></param>
        /// <returns>응답 XmlDocument</returns>
        public static XmlDoc Send(string uri, XmlDoc document,
                                  bool isAsync = false,
                                  int timeoutMilliseconds = System.Threading.Timeout.Infinite,
                                  string userId = null,
                                  string password = null) {
            uri.ShouldNotBeWhiteSpace("uri");
            document.ShouldNotBeNull("document");
            Guard.Assert(document.IsValidDocument(), "document is not valid xml document.");

            if(IsDebugEnabled)
                log.Debug("Send... uri=[{0}], isAsync=[{1}], timeoutMilliseconds=[{2}], userId=[{3}], password=[{4}], document=[{5}]",
                          uri, isAsync, timeoutMilliseconds, userId, password, document);

            var xmlHttp = new XmlHttpClient(uri, isAsync, timeoutMilliseconds, userId, password);
            return xmlHttp.Send(document);
        }
    }
}