using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Networks {
    /// <summary>
    /// Html Document 형식과 관련된 Utility Class
    /// </summary>
    public static class HtmlTool {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// email 주소 추출
        /// </summary>
        public const string PATTERN_MAIL = @"(?<UserId>\w+)@(?<Domain>[\w.]+)(?x)";

        /// <summary>
        /// web 주소 추출 Web 주소 (ex. www.realweb21.com ) 와 같은 것을 앞 뒤로 공백을 줘야 제대로 찾는다.
        /// </summary>
        public const string PATTERN_WWW = @"\s(?<HostName>w{3})\.(?<Domain>[\w.]+\/?)\S*(?x)";

        /// <summary>
        /// url 주소 추출
        /// </summary>
        public const string PATTERN_URL = @"(?<Protocol>\w+):\/\/(?<Domain>[\w.]+\/?)\S*(?x)";

        /// <summary>
        /// mail 주소 변환 형식
        /// </summary>
        public const string REPLACE_PATTERN_MAIL = "<a href=\"mailto:${1}@${2}\">${0}</a>";

        /// <summary>
        /// web site 주소 변환 형식
        /// </summary>
        public const string REPLACE_PATTERN_WWW = " <a href=\"http://${0}\">${0}</a> ";

        /// <summary>
        /// URL 변환 형식
        /// </summary>
        public const string REPLACE_PATTERN_URL = "<a href=\"${0}\">${0}</a>";

        /// <summary>
        /// pre tag로 감싸여진 문장
        /// </summary>
        private const string PATTERN_PRE_TAG = @"<pre>(\S|\s)*</pre>";

        private const string PATTERN_CODE_TAG = @"<code>(\S|\s)*</code>";

        /// <summary>
        /// 문장 첫 줄에 있는 space나 tab 문자
        /// </summary>
        private const string PATTERN_NBSP_TAG = @"^\s[ \t]*";

        private static readonly char[] _reserved = { '?', '=', '&' };

        /// <summary>
        /// 문자열을 Html 형식으로 Encoding한다.
        /// </summary>
        /// <param name="s">문자열</param>
        /// <returns>Html형식으로 Encoding된 문자열</returns>
        public static string HtmlEncode(string s) {
            return HttpUtility.HtmlEncode(s);
        }

        /// <summary>
        /// 지정된 스트림을 Html 형식으로 Encoding한다.
        /// </summary>
        /// <param name="stream">스트림</param>
        /// <returns>인코딩된 문자열</returns>
        public static string HtmlEncode(Stream stream) {
            stream.ShouldNotBeNull("stream");

            return HtmlEncode(stream.ToText());

            // return HtmlEncode(StringTool.ToString(stream));
        }

        /// <summary>
        /// 지정된 스트림을 Html 형식으로 Encoding 한다.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="output">인코딩된 문자가 담긴 stream</param>
        public static void HtmlEncode(string s, Stream output) {
            output.ShouldNotBeNull("output");

            using(var writer = new StreamWriter(output)) {
                HttpUtility.HtmlEncode(s, writer);
                writer.Flush();
            }
        }

        /// <summary>
        /// Html 형식으로 인코딩된 문자열을 디코딩을 수행한다.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string HtmlDecode(string s) {
            return HttpUtility.HtmlDecode(s);
        }

        /// <summary>
        /// Html 형식으로 인코딩된 스트림을 디코딩한다.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static string HtmlDecode(Stream stream) {
            return HtmlDecode(stream.ToText());
            // return HtmlDecode(StringTool.ToString(stream));
        }

        /// <summary>
        /// Html 형식으로 인코딩된 문자열을 디코딩하여 스트림을 반환한다.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="output"></param>
        public static void HtmlDecode(string s, Stream output) {
            using(var sw = new StreamWriter(output)) {
                HttpUtility.HtmlDecode(s, sw);
                sw.Flush();
            }
        }

        /// <summary>
        /// Url을 Encoding한다. 특히 인자가 한글인 경우 문자열이 깨지기 쉽고, 
        /// 연결자 (?, &amp;) 등과 겹쳐서 다른 문자로 해석되어 질 수 있다.
        /// 이를 방지하기 위해 위의 문자들을 제거하여 작업을 수행한다.
        /// </summary>
        /// <param name="payload">인코딩할 문자열</param>
        /// <param name="enc">인코딩 방식</param>
        /// <returns>UrlEncoded payload.</returns>
        public static string UrlEncode(string payload, Encoding enc = null) {
            enc = enc ?? StringTool.DefaultEncoding;
            var encoded = new StringBuilder();

            if(payload.IsNotEmpty() && enc != null) {
                var i = 0;
                while(i < payload.Length) {
                    var j = payload.IndexOfAny(_reserved, i);
                    if(j == -1) {
                        encoded.Append(HttpUtility.UrlEncode(payload.Substring(i, payload.Length - i), enc));
                        break;
                    }

                    encoded.Append(HttpUtility.UrlEncode(payload.Substring(i, j - i), enc));
                    encoded.Append(payload.Substring(j, 1));

                    i = j + 1;
                }
            }
            return encoded.ToString();
        }

        /// <summary>
        /// Url을 UTF8 방식으로 Encoding한다. 특히 인자가 한글인 경우 문자열이 깨지기 쉽고, 
        /// 연결자 (?, &amp;) 등과 겹쳐서 다른 문자로 해석되어 질 수 있다.
        /// 이를 방지하기 위해 위의 문자들을 제거하여 작업을 수행한다.
        /// </summary>
        /// <param name="payload">URL 인자값</param>
        /// <param name="output">url encoded bytes by <see cref="Encoding.UTF8"/></param>
        public static void UrlEncode(string payload, out byte[] output) {
            output = Encoding.UTF8.GetBytes(UrlEncode(payload));
        }

        /// <summary>
        /// Url을 Encoding한다. 특히 인자가 한글인 경우 문자열이 깨지기 쉽고, 
        /// 연결자 (?, &amp;) 등과 겹쳐서 다른 문자로 해석되어 질 수 있다.
        /// 이를 방지하기 위해 위의 문자들을 제거하여 작업을 수행한다.
        /// </summary>
        /// <param name="payload">URL 인자값</param>
        /// <param name="enc">byte로 변환할 시에 인코더</param>
        /// <param name="output">url encoded bytes by a specified <paramref name="enc"/></param>
        public static void UrlEncode(string payload, Encoding enc, out byte[] output) {
            output = enc.GetBytes(UrlEncode(payload, enc));
        }

        /// <summary>
        /// Url 인코딩된 데이터를 지정된 범위를 지정된 인코딩 방식으로 디코딩을 수행한다.
        /// </summary>
        /// <param name="bytes">인코딩된 데이타</param>
        /// <param name="offset">시작 위치</param>
        /// <param name="count">변경할 바이트 수</param>
        /// <param name="enc">인코딩 방식</param>
        /// <returns>Url 디코딩된 문자열</returns>
        public static string UrlDecode(byte[] bytes, int offset = 0, int count = 0, Encoding enc = null) {
            return HttpUtility.UrlDecode(bytes, offset, (count > 0) ? count : bytes.Length - offset, enc ?? StringTool.DefaultEncoding);
        }

        /// <summary>
        /// Url 인코딩된 데이터를 지정된 범위를 지정된 인코딩 방식으로 디코딩을 수행한다.
        /// </summary>
        /// <param name="s">string to decoding</param>
        /// <param name="enc">인코딩 방식</param>
        /// <returns>Url 디코딩된 문자열</returns>
        public static string UrlDecode(string s, Encoding enc = null) {
            return HttpUtility.UrlDecode(s, enc ?? StringTool.DefaultEncoding);
        }

        /// <summary>
        ///	HTML 문자열중에 web, mail, ftp, news 등과 같은 주소가 있는 경우 link를 만들어 준다.
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        /// <remarks>
        ///	이부분은 RegularExpression을 사용하여 Web, mail, ftp, news등의 주소를 
        ///	걸러내어서 작업 처리할 수 있도록 한다.
        /// </remarks>
        [Obsolete("완전한 기능이 아닙니다.")]
        public static string MakeAutoLink(string html) {
            return MakeAutoLink(html, true);
        }

        /// <summary>
        /// 게시판 글 들 중에 web address, email 등에 자동으로 link를 생성한다.
        /// </summary>
        /// <param name="html"></param>
        /// <param name="displayHtml"></param>
        /// <returns></returns>
        [Obsolete("완전한 기능이 아닙니다.")]
        public static string MakeAutoLink(string html, bool displayHtml) {
            if(IsDebugEnabled)
                log.Debug("Make Auto link the specified html text. html=[{0}], displayHtml=[{1}]",
                          html.EllipsisChar(80), displayHtml);

            string result = html;

            const RegexOptions regexOptions = RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace;

            result = Regex.Replace(result, PATTERN_URL, REPLACE_PATTERN_URL, regexOptions);
            result = Regex.Replace(result, PATTERN_MAIL, REPLACE_PATTERN_MAIL, regexOptions);
            result = Regex.Replace(result, PATTERN_WWW, REPLACE_PATTERN_WWW, regexOptions);

            if(displayHtml) {
                result = HtmlDecode(result);

                result = ToNbsp(result, regexOptions);
                result = ToSpace(result, regexOptions);
                result = ConvertToHtml(result);
            }
            else {
                result = ConvertToHtml(result);
            }

            if(IsDebugEnabled)
                log.Debug("Auto linked html text is [{0}]", result.EllipsisChar(80));

            return result;
        }

        /// <summary>
        /// 일반 문자열을 HTML 문자열로 변환하기 위하여 Environment.NewLine을 &lt;BR&gt;으로 변환한다.
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static string ConvertToHtml(string src) {
            if(src.IsNotEmpty()) {
                return src
                    .Replace("\r\n", "\n")
                    .Replace("\r", "\n")
                    .Replace("\n", "<BR/>");
            }

            return string.Empty;
        }

        /// <summary>
        /// displayHtml이 true인 경우
        /// NewLine 뒤의 첫 단어가 나오기까지의 공백은 &amp;nbsp;로 교체된다.
        /// &lt; &gt; &amp; 값을 원래 Html Tag로 변경한다.
        /// </summary>
        private static string ToNbsp(string encSrc, RegexOptions ops) {
            string result = encSrc;

            if(result.IsNotEmpty()) {
                MatchEvaluator matchEvaluator = (match) => SpaceToNbsp(match);
                result = Regex.Replace(result, PATTERN_NBSP_TAG, matchEvaluator, ops);
            }

            return result;
        }

        /// <summary>
        /// pre 태그나 code 태그가 붙은 것들은 원래 문장으로 복원한다.
        /// </summary>
        /// <param name="html">Html 형식의 문자열</param>
        /// <param name="ops">정규식 옵션</param>
        /// <returns></returns>
        private static string ToSpace(string html, RegexOptions ops) {
            string result = html;

            if(result.IsNotEmpty()) {
                MatchEvaluator matchEvaluator = NbspToSpace;

                result = Regex.Replace(result, PATTERN_PRE_TAG, matchEvaluator, ops);
                result = Regex.Replace(result, PATTERN_CODE_TAG, matchEvaluator, ops);
            }
            return result;
        }

        /// <summary>
        /// space 갯수에 따라 &amp;nbsp;를 넣어야하므로 delegate로 변환 처리를 위임한다. 
        /// </summary>
        /// <param name="match">result of single regular expression match.</param>
        /// <param name="tabNbspCount">Tab을 space로 바꿀때의 크기</param>
        /// <returns></returns>
        private static string SpaceToNbsp(Match match, int tabNbspCount = 4) {
            var sb = new StringBuilder(match.Groups[0].Value);

            var tabNbsp = (tabNbspCount > 0) ? "&nbsp;".Replicate(tabNbspCount) : string.Empty;

            sb.Replace(" ", "&nbsp;");
            sb.Replace("\t", tabNbsp);

            return sb.ToString();
        }

        /// <summary>
        /// Html의 space를 나타내는 문장 (&amp;nbsp;) 을 ASCII space로 변환한다.
        /// </summary>
        /// <param name="match">result of single regular expression match.</param>
        /// <returns>Html의 &amp;nbsp;를 공백 문자로 변경한 문자열</returns>
        private static string NbspToSpace(Match match) {
            var sb = new StringBuilder(match.Groups[0].Value);

            sb.Replace("&nbsp;", " ");

            return sb.ToString();
        }
    }
}