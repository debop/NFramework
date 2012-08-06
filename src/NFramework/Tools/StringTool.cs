using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NSoft.NFramework.Tools {
    /// <summary>
    /// String 관련 Utility class
    /// </summary>
    public static partial class StringTool {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        #region << Const / Variables >>

        /// <summary>
        /// MultiByte 언어를 Stream 으로 변환 시에, MultiByte 임을 나타내는 Byte 접두사 (0xEF, 0xBB, 0xBF), FusionCharts에서도 이 bytes 가 쓰인다.
        /// </summary>
        public static readonly byte[] MultiBytesPrefixBytes = new byte[] { 0xEF, 0xBB, 0xBF };

        /// <summary>
        /// 문자열 축약을 표현하는 문자열 ("...")
        /// </summary>
        public const string TrimmingString = @"...";

        /// <summary>
        /// Default Encoding (<see cref="Encoding.Default"/>) 를 나타낸다.
        /// </summary>
        public static readonly Encoding DefaultEncoding = Encoding.UTF8;

        /// <summary>
        /// 시스템 개행 문자
        /// </summary>
        /// <value>Windows 시스템에서는 \r\n 값을 갖는다.</value>
        public static string NewLine = Environment.NewLine;

        /// <summary>
        /// C language에서 string의 끝을 나타내는 char
        /// </summary>
        public const char NullTerminatorChar = '\0';

        /// <summary>
        /// null 을 표현하는 문자열
        /// </summary>
        public const string Null = @"null";

        /// <summary>
        /// 공백
        /// </summary>
        public const string WhiteSpace = @" ";

        /// <summary>
        /// 문자열에서 공백과 같이 취급되는 제어 문자들 (개행문자, 탭, 라인피드 등)
        /// </summary>
        public static readonly char[] WhiteSpaceChars = new char[] { '\n', '\r', '\f', '\t', '\b' };

        public const string AsciiEncodeName = @"us-ascii";

        #endregion

        #region << IsNull / IsEmpty / IsNotEmpty / IsWhiteSpace / IsNotWhiteSpace / IsMultiByteString >>

        /// <summary>
        /// 지정된 문자열이 Null인지 확인
        /// </summary>
        /// <param name="s">검사할 문자열</param>
        /// <returns>null 인지 검사 결과</returns>
        public static bool IsNull(this string s) {
            return (s == null);
        }

        /// <summary>
        /// 지정된 문자열이 Null이 아닌지 확인
        /// </summary>
        /// <param name="s"></param>
        /// <returns>s가 null이 아니면 true, null이면 false</returns>
        public static bool IsNotNull(this string s) {
            return (s != null);
        }

        /// <summary>
        /// 지정된 문자열이 Null이거나 Trim을 수행하여 길이가 0가 된 문자열인지 확인.
        /// </summary>
        /// <param name="s">검사할 문자열</param>
        /// <param name="withTrim">문자열을 Trim후에 길이 검사를 할 것인지 여부</param>
        /// <returns>빈 문자열 여부</returns>
        public static bool IsEmpty(this string s, bool withTrim = false) {
            return s == null || (((withTrim) ? s.Trim() : s).Length == 0);
        }

        /// <summary>
        /// 지정된 문자열에 내용이 있으면 True, Null이거나 길이가 0인 문자열이면 False를 반환
        /// </summary>
        /// <param name="s">검사할 문자열</param>
        /// <param name="withTrim">공백 제거 여부</param>
        /// <returns></returns>
        public static bool IsNotEmpty(this string s, bool withTrim = false) {
            return s != null && (IsEmpty(s, withTrim) == false);
        }

        /// <summary>
        /// 문자열이 null이거나, 빈 문자열 또는 공백만 있는 문자열, \n, \r, \t, \f 같은 제어문자만 있는 문자열이라면 이라면 true를 반환한다.
        /// </summary>
        /// <param name="s">검사할 문자열</param>
        /// <returns></returns>
        public static bool IsWhiteSpace(this string s) {
            return
                s == null ||
                s.Trim().Length == 0 ||
                s.Any(c => !WhiteSpaceChars.Contains(c)) == false;
        }

        /// <summary>
        /// 문자열이 null이거나, 빈 문자열 또는 공백만 있는 문자열, \n, \r, \t, \f 같은 제어문자만 있는 문자열이라면 이라면 false를 반환한다.
        /// </summary>
        /// <param name="s">검사할 문자열</param>
        /// <returns></returns>
        public static bool IsNotWhiteSpace(this string s) {
            return s != null && (s.IsWhiteSpace() == false);
        }

        /// <summary>
        /// 지정된 바이트 배열이 2-byte 문자열 정보인지 파악합니다. 선두의 3개의 요소가 <see cref="MultiBytesPrefixBytes"/>라면 2-byte 문자열입니다.
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static bool IsMultiByteString(this byte[] bytes) {
            if(bytes == null || bytes.Length < MultiBytesPrefixBytes.Length)
                return false;

            return
                Enumerable
                    .Range(0, MultiBytesPrefixBytes.Length)
                    .All(i => MultiBytesPrefixBytes[i] == bytes[i]);
        }

        /// <summary>
        /// 지정된 문자열이 ASCII가 아닌 2 바이트 문자열인지 검사한다.
        /// </summary>
        /// <param name="s">검사할 문자열</param>
        /// <returns>2Byte 문자열이면 true, ascii 문자열이면 false</returns>
        public static bool IsMultiByteString(this string s) {
            bool result = false;

            if(IsNotEmpty(s, true)) {
                try {
#if !SILVERLIGHT
                    var ms = Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(s));
                    result = (s != ms);
#else
					var encoding = Encoding.GetEncoding(AsciiEncodeName);
					var bytes = encoding.GetBytes(s);
					var ms = encoding.GetString(bytes, 0, bytes.Length);
					result = (s != ms);
#endif
                }
                catch(Exception) {
                    result = true;
                }
            }
            return result;
        }

        #endregion

        #region << String 조작 >>

        /// <summary>
        /// 원본 문자열과 같은 문자열을 생성합니다. 메모리 번지가 다른 인스턴스를 새로 만듭니다.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string Copy(this string source) {
            return string.Copy(source);
        }

        /// <summary>
        /// 지정된 대상 문자열에서 지정된 문자들을 삭제한다.
        /// </summary>
        /// <param name="s">대상 문자열</param>
        /// <param name="charArray">삭제할 문자들</param>
        /// <returns>대상문자열에서 지정된 문자를 삭제한 문자열</returns>
        public static string DeleteCharAny(this string s, params char[] charArray) {
            if(IsEmpty(s))
                return s;

            int pos = 0;

            while((pos = s.IndexOfAny(charArray, pos)) > -1) {
                s = s.Remove(pos, 1);
            }
            return s;
        }

        /// <summary>
        /// 지정된 문자열에서 지정된 문자배열에 있는 문자들을 삭제한다.
        /// </summary>
        /// <param name="s">대상 문자열</param>
        /// <param name="charArray">삭제할 문자들</param>
        /// <returns>대상문자열에서 지정된 문자를 삭제한 문자열</returns>
        public static string DeleteChar(this string s, char[] charArray) {
            if(IsEmpty(s))
                return s;

            int pos = 0;
            while((pos = s.IndexOfAny(charArray, pos)) > -1) {
                s = s.Remove(pos, 1);
            }

            return s;
        }

        /// <summary>
        /// 지정된 문자열에서 지정된 문자를 삭제한다.
        /// </summary>
        /// <param name="s">대상 문자열</param>
        /// <param name="c">삭제할 문자</param>
        /// <returns></returns>
        public static string DeleteChar(this string s, char c) {
            return DeleteChar(s, new[] { c });
        }

        /// <summary>
        /// 문자열 배열을 구분자를 중간에 넣어서 결합시킨다.
        /// <see cref="string.Join(string,string[])"/> 함수는 string[] 만을 인자로 받는다.
        /// </summary>
        /// <param name="separator">문장 구분자</param>
        /// <param name="value">결합할 문자열 배열</param>
        /// <returns>결합된 문자열</returns>
        public static string Join(this string separator, params string[] value) {
            return string.Join(separator, value);
        }

        /// <summary>
        /// 문자열 시퀀스를 구분자를 중간에 넣어서 결합시킨다.
        /// </summary>
        /// <param name="strs"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string Join(this IEnumerable<string> strs, string separator) {
            strs.ShouldNotBeNull("strs");

            var strArray = strs.ToArray();

            if(strArray.Length == 0)
                return string.Empty;

            return string.Join(separator, strArray);
        }

        /// <summary>
        /// 지정된 문자열을 SQL 문법의 문자열에 맞게 홑따옴표 형식으로 바꾼다.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string QuotedStr(this string s) {
            return IsNull(s) ? "NULL" : string.Format("\'{0}\'", s.Replace("\'", "\'\'"));
        }

        /// <summary>
        /// 지정된 문자열을 SQL 문법의 문자열에 맞게 홑따옴표 형식으로 바꾼다.
        /// 지정된 문자열이 null이거나 빈 문자열이면 기본값의 홑따옴표 형식을 반환한다.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="defaultStr"></param>
        /// <returns></returns>
        public static string QuotedStr(this string s, string defaultStr) {
            return (IsEmpty(s)) ? QuotedStr(defaultStr) : QuotedStr(s);
        }

        /// <summary>
        /// 문자열의 char 순서를 역순으로 변경한다.
        /// </summary>
        /// <param name="s">대상 문자열</param>
        /// <returns>역순 문자열</returns>
        /// <remarks>Encoding 방법과 무관하게 실행되므로 한글등에서는 문제가 생길 수 있다.</remarks>
        public static string Reverse(this string s) {
            if(IsEmpty(s))
                return s;

            var cs = new char[s.Length];
            for(int i = s.Length - 1, j = 0; i >= 0; i--, j++) {
                cs[i] = s[j];
            }

            return new string(cs);
        }

        /// <summary>
        /// 지정한 횟수만큼 문자 식을 반복합니다.
        /// </summary>
        /// <param name="s">문자식</param>
        /// <param name="n">반복 횟수</param>
        /// <returns>지정된 문자열이 반복되어진 문자열</returns>
        public static string Replicate(this string s, int n) {
            if(IsEmpty(s))
                return string.Empty;

            var result = new StringBuilder(s.Length * n * 2);

            for(int i = 0; i < n; i++)
                result.Append(s);

            return result.ToString();
        }

#if !SILVERLIGHT

        /// <summary>
        /// 문자열의 WORD 단위로 첫문자만 대문자로 만든다.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        /// <example>
        ///	<code>
        ///		string s = "abc PROJECT ID 한글 oH nO file name fileName"
        ///		Console.WriteLine("{0} to Capitalize : {1}", s, s.Captitalize());
        /// 
        ///		// "aBC PROJECT_ID 한글 oH nO file name fileName" to Capitalize : "Abc Project_Id 한글 Oh No File Name Filename"
        ///	</code>
        /// </example>
        public static string Capitalize(this string s) {
            if(s == null)
                return string.Empty;

            if(s.IsWhiteSpace())
                return s;

            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(s.ToLowerInvariant());
        }
#else
    /// <summary>
    /// 문자열의 WORD 단위로 첫문자만 대문자로 만든다.
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    /// <example>
    ///	<code>
    ///		string s = "abc PROJECT ID 한글 oH nO file name fileName"
    ///		Console.WriteLine("{0} to Capitalize : {1}", s, s.Captitalize());
    /// 
    ///		// "aBC PROJECT_ID 한글 oH nO file name fileName" to Capitalize : "Abc Project_Id 한글 Oh No File Name Filename"
    ///	</code>
    /// </example>
		public static string Capitalize(this string s)
		{
			if(s == null)
				return string.Empty;

			if(s.IsWhiteSpace())
				return s;

			var strs = s.Split(StringSplitOptions.RemoveEmptyEntries, ' ');
			var builder = new StringBuilder();

			foreach(var str in strs)
			{
				builder.Append(str[0].ToString().ToUpper());
				if(str.Length > 1)
					builder.Append(str.Substring(1).ToLower());
			}
			return builder.ToString();
		}
#endif

        /// <summary>
        /// Class나 Property의 Naming 방식인 Pascal Naming 또는 Camelcase Naming 방식의 문자열을 ORACLE Naming 방식의 문자열로 변환합니다.
        /// </summary>
        /// <param name="s">변환할 문자열</param>
        /// <param name="delimiter">단어 사이의 구분자 (기본값은 '_')</param>
        /// <returns>변환된 문자열</returns>
        /// <example>
        /// <code>
        ///		"CompanyName".ToOracleNaming().Should().Be("COMPANY_NAME");
        ///		"UserId".ToOracleNaming().Should().Be("USER_ID");
        ///     "ExAttr1".ToOracleNaming().Should().Be("EX_ATTR1");
        ///		"IX_CompanyName.ToOracleNaming().Should().Be("IX_COMPANY_NAME");
        ///		"IX_User_CompanyId".ToOracleNaming().Should().Be("IX_USER_COMPANY_ID");
        /// </code>
        /// </example>
        public static string ToOracleNaming(this string s, string delimiter = "_") {
            if(s.IsWhiteSpace())
                return s.AsText();

            delimiter = delimiter ?? "_";
            var builder = new StringBuilder();

            var prevCharIsCapital = false;

            foreach(var c in s) {
                var isCapital = (c >= 'A' && c <= 'Z') || (c.ToString() == delimiter);

                if(isCapital) {
                    if(prevCharIsCapital == false)
                        builder.Append(delimiter);
                }

                builder.Append(c);
                prevCharIsCapital = isCapital;
            }

            return
                builder
                    .Replace(delimiter + delimiter, delimiter)
                    .ToString()
                    .TrimStart('_')
                    .ToUpper(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// 지정된 문자열을 구분자로 구분하여 문자열 배열을 만든다.
        /// </summary>
        /// <param name="s">대상 문자열</param>
        /// <param name="separator">구분할 문자열</param>
        /// <returns>빈문자열을 준 경우 길이가 0인 문자열 배열을 반환한다.</returns>
        public static string[] Split(this string s, params char[] separator) {
            if(IsEmpty(s))
                return new string[0];

            return s.Split(separator);
        }

        /// <summary>
        /// 지정된 문자열을 구분자로 구분하여 문자열 배열을 만든다.
        /// </summary>
        /// <param name="s">대상 문자열</param>
        /// <param name="splitOptions">Split 시 옵션</param>
        /// <param name="separator">구분할 문자열</param>
        /// <returns>빈문자열을 준 경우 길이가 0인 문자열 배열을 반환한다.</returns>
        public static string[] Split(this string s, StringSplitOptions splitOptions, params char[] separator) {
            if(IsEmpty(s))
                return new string[0];

            return s.Split(separator, splitOptions);
        }

        /// <summary>
        /// 지정된 문자열을 지정된 패턴을 구분자로 하여 문자열 배열로 만든다.
        /// </summary>
        /// <param name="s">대상 문자열</param>
        /// <param name="pattern">구분할 문자열</param>
        /// <param name="ignoreCase">대소문자 구분 여부 </param>
        /// <param name="culture"><c>CultureInfo</c></param>
        /// <returns>문자열 1차원 배열</returns>
        public static string[] Split(this string s, string pattern, bool ignoreCase = true, CultureInfo culture = null) {
            if(IsEmpty(s))
                return new string[0];

            if(IsEmpty(pattern))
                return new[] { s };

            culture = culture.GetOrInvaliant();

            string searchStr, searchPattern;

            if(ignoreCase) {
                searchStr = s.ToUpper(culture);
                searchPattern = pattern.ToUpper(culture);
            }
            else {
                searchStr = s;
                searchPattern = pattern;
            }

            //StringList sl = new StringList();

            var sl = new List<string>();

            int startIndex;
            int offset = 0;
            int length = s.Length;

            do {
                startIndex = searchStr.IndexOf(searchPattern, offset, StringComparison.Ordinal);

                if(startIndex < 0) {
                    sl.Add(s.Substring(offset, length - offset));
                    break;
                }

                if(startIndex - offset > 0)
                    sl.Add(s.Substring(offset, startIndex - offset));

                offset = startIndex + pattern.Length;
            } while(startIndex > 0);

            return sl.ToArray();
        }

        /// <summary>
        /// 원본 문자열에서 oldPattern에 해당하는 문자열을 newPattern의 문자열로 교체한다.
        /// </summary>
        /// <param name="S">원본 문자열</param>
        /// <param name="oldPattern">바뀔 문자열</param>
        /// <param name="newPattern">새로운 문자열</param>
        /// <param name="ignoreCase">대소문자 구분 여부</param>
        /// <param name="culture"><c>CultureInfo</c></param>
        /// <returns>바뀐 문자열</returns>
        public static string Replace(this string S, string oldPattern, string newPattern, bool ignoreCase = true,
                                     CultureInfo culture = null) {
            if(S.IsEmpty())
                return S;

            oldPattern.ShouldNotBeEmpty("oldPattern"); // 공백일 수도 있으므로

            culture = culture.GetOrInvaliant();

            string searchStr, pattern;
            var result = new StringBuilder(S.Length * 2);

            if(ignoreCase) {
                searchStr = S.ToUpper(culture);
                pattern = oldPattern.ToUpper(culture);
            }
            else {
                searchStr = S;
                pattern = oldPattern;
            }

            var startIndex = 0;
            var length = S.Length;
            var offset = startIndex;

            do {
                startIndex = searchStr.IndexOf(pattern, offset, StringComparison.Ordinal);

                if(startIndex < 0) {
                    result.Append(S.Substring(offset, length - offset));
                    break;
                }

                if(startIndex - offset > 0)
                    result.Append(S.Substring(offset, startIndex - offset));

                result.Append(newPattern);

                offset = startIndex + pattern.Length;
            } while(startIndex >= 0);

            return result.ToString();
        }

        /// <summary>
        /// 지정된 포맷 문자열을 이용하여, 서식 지정 문자열로 됩니다.
        /// </summary>
        public static string FormatWith(this string format, object arg0) {
            return string.Format(format, arg0);
        }

        /// <summary>
        /// 지정된 포맷 문자열을 이용하여, 서식 지정 문자열로 됩니다.
        /// </summary>
        public static string FormatWith(this string format, object arg0, object arg1) {
            return string.Format(format, arg0, arg1);
        }

        /// <summary>
        /// 지정된 포맷 문자열을 이용하여, 서식 지정 문자열로 됩니다.
        /// </summary>
        public static string FormatWith(this string format, params object[] args) {
            return string.Format(format, args);
        }

        /// <summary>
        /// 지정된 포맷 문자열을 이용하여, 서식 지정 문자열로 됩니다.
        /// </summary>
        public static string FormatWith(this string format, IFormatProvider provider, params object[] args) {
            return string.Format(provider, format, args);
        }

        /// <summary>
        /// null 이거나 빈 문자열인 경우에는 null을 반환합니다.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string WhiteSpaceAsNull(this string text) {
            return text.IsWhiteSpace() ? null : text;
        }

        #endregion

        #region << String 검사 >>

        /// <summary>
        /// 지정된 문자열에 지정된 패턴이 검색되는 횟수를 반환한다.
        /// </summary>
        /// <param name="text">대상 문자열</param>
        /// <param name="pattern">검색 문자열 패턴</param>
        /// <param name="ignoreCase">대소문자 구분 여부</param>
        /// <param name="culture">특정 CultureInfo</param>
        /// <returns>검색 횟수</returns>
        public static int WordCount(string text, string pattern, bool ignoreCase = true, CultureInfo culture = null) {
            if(IsEmpty(text) || IsEmpty(pattern))
                return 0;

            culture = culture.GetOrInvaliant();

            StringComparison comparision;

            if(ignoreCase)
                comparision = (culture == CultureInfo.InvariantCulture)
                                  ? StringComparison.InvariantCultureIgnoreCase
                                  : StringComparison.CurrentCultureIgnoreCase;
            else
                comparision = (culture == CultureInfo.InvariantCulture)
                                  ? StringComparison.InvariantCulture
                                  : StringComparison.CurrentCulture;

            int count = 0;
            int index = 0;
            int pattenLength = pattern.Length;
            int maxLength = text.Length - pattenLength;

            do {
                index = text.IndexOf(pattern, index, comparision);

                if(index < 0)
                    break;

                count++;
                index += pattenLength;
            } while(index <= maxLength);

            return count;
        }

        /// <summary>
        /// 지정된 문자열에 지정된 문자들의 갯수를 구한다.
        /// </summary>
        /// <param name="s">대상 문자열</param>
        /// <param name="word">찾을 char</param>
        /// <returns>문자열에서 찾은 수</returns>
        public static int WordCount(string s, params char[] word) {
            if(IsEmpty(s) || word.Length == 0)
                return 0;

            return s.Count(t => Array.IndexOf(word, t) != -1);
        }

        /// <summary>
        /// 문자열을 <see cref="System.Globalization.CultureInfo.CurrentCulture"/>를 이용하여 비교한다.
        /// </summary>
        /// <remarks>
        /// string.Compare 에 관련된 많은 함수들이 제공되므로 더 자세한 제어는 그 함수를 이용하세요.
        /// </remarks>
        /// <param name="s1">비교 문자열 1</param>
        /// <param name="s2">비교 문자열 2</param>
        /// <param name="ignoreCase">대소문자 구분 여부</param>
        /// <param name="culture">문화권</param>
        /// <returns>s1과 s2가 같다면 0, s1 &gt; s2 면 양수, s1 &lt; s2 이면 음수</returns>
        public static int Compare(this string s1, string s2, bool ignoreCase = true, CultureInfo culture = null) {
            culture = culture.GetOrInvaliant();
#if !SILVERLIGHT
            return string.Compare(s1, s2, ignoreCase, culture);
#else
			return string.Compare(s1, s2, CultureInfo.InvariantCulture, (ignoreCase) ? CompareOptions.IgnoreCase : CompareOptions.None);
#endif
        }

        /// <summary>
        /// 지정된 두 문자열이 같은지 검사합니다.
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <param name="ignoreCase">대소문자 구분</param>
        /// <param name="culture">문화권</param>
        /// <returns></returns>
        public static bool EqualTo(this string s1, string s2, bool ignoreCase = true, CultureInfo culture = null) {
            return Compare(s1, s2, ignoreCase, culture) == 0;
        }

        /// <summary>
        /// 지정된 문자열의 첫번째 라인의 문자열만 (개행문자는 제외하고) 추출한다.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string GetFirstLine(this string text) {
            if(IsEmpty(text))
                return text;

            var index = text.IndexOf(Environment.NewLine);

            if(index > -1)
                return text.Substring(0, index);

            return text;
        }

        /// <summary>
        /// 문자열에서 N 번째 찾을 sub string의 인덱스 (인덱스는 0부터 시작합니다.) 를 반환합니다.
        /// </summary>
        /// <param name="s">원본 문자열</param>
        /// <param name="sub">찾을 문자열</param>
        /// <param name="nth">nth 번째 검색</param>
        /// <param name="ignoreCase">대소문자 구분 여부</param>
        /// <param name="culture">문화권</param>
        /// <returns>찾은 위치, 검색 실패시에는 -1을 반환, 인덱스는 0부터 시작합니다.</returns>
        public static int IndexOfN(this string s, string sub, int nth, bool ignoreCase = true, CultureInfo culture = null) {
            if(IsEmpty(s) || IsEmpty(sub))
                return -1;

            culture = culture.GetOrInvaliant();

            string sourceStr, subStr;

            if(ignoreCase) {
                sourceStr = s.ToUpper(culture);
                subStr = sub.ToUpper(culture);
            }
            else {
                sourceStr = s;
                subStr = sub;
            }

            int index = 0;
            int length = sourceStr.Length;
            int sublen = subStr.Length;

            for(int i = 0; i < nth; i++) {
                index = sourceStr.IndexOf(subStr, index, length - index, StringComparison.Ordinal);

                if(index < 0)
                    return -1;

                if(i == (nth - 1))
                    return index;

                index += sublen;
                if(index >= length)
                    break;
            }

            return -1;
        }

        /// <summary>
        /// 첫번째 찾은 문자열 위치로부터 왼쪽에 있는 문자열을 반환
        /// </summary>
        /// <param name="text">대상 문자열</param>
        /// <param name="c">찾을 문자열</param>
        /// <returns>string to the left of c.</returns>
        public static string FirstOf(this string text, string c) {
            int idx = text.IndexOf(c);

            if(idx > -1)
                return text.Substring(0, idx);

            return text;
        }

        /// <summary>
        /// n 번째 찾은 문자열 위치로부터 왼쪽에 있는 문자열 반환
        /// </summary>
        /// <param name="text">대상 문자열</param>
        /// <param name="c">찾을 문자열</param>
        /// <param name="n">찾는 횟수</param>
        /// <returns></returns>
        public static string FirstOf(this string text, string c, int n) {
            int idx = IndexOfN(text, c, n);

            if(idx > -1)
                return text.Substring(0, idx);

            return text;
        }

        /// <summary>
        ///	첫번째 찾은 위치의 오른쪽에 있는 문자열 반환
        /// </summary>
        /// <param name="text">대상 문자열</param>
        /// <param name="c">찾을 문자열</param>
        /// <returns>string to the right of c.</returns>
        public static string LastOf(this string text, string c) {
            var idx = text.IndexOf(c);

            if(idx > -1)
                return text.Substring(idx + c.Length);

            return string.Empty;
        }

        /// <summary>
        /// n 번째 찾은 문자열 위치로부터 오른쪽에 남은 문자열을 반환
        /// </summary>
        /// <param name="text">대상 문자열</param>
        /// <param name="c">찾을 문자열</param>
        /// <param name="n">찾는 횟수</param>
        /// <returns></returns>
        public static string LastOf(this string text, string c, int n) {
            var idx = IndexOfN(text, c, n);

            if(idx > -1)
                return text.Substring(idx + c.Length);

            return string.Empty;
        }

        /// <summary>
        /// 대상문자열의 처음부터 ~ 뒤에서부터 찾은 위치 전까지의 문자열을 찾는다.
        /// </summary>
        /// <param name="text">대상 문자열</param>
        /// <param name="sub">찾을 문자열</param>
        /// <returns>대상문자열의 처음부터 ~ 뒤부터 찾은 위치 전까지의 문자열</returns>
        public static string FirstOfLastIndexOf(this string text, string sub) {
            var idx = text.LastIndexOf(sub);

            if(idx > -1)
                return text.Substring(0, idx);

            return text;
        }

        /// <summary>
        /// 대상문자열의 뒤부터 찾은 위치부터 마지막 문자까지
        /// </summary>
        /// <param name="text">대상 문자열</param>
        /// <param name="sub">찾을 문자열</param>
        /// <returns></returns>
        public static string LastOfLastIndexOfthis(this string text, string sub) {
            var idx = text.LastIndexOf(sub);

            if(idx > -1)
                return text.Substring(idx + sub.Length);

            return string.Empty;
        }

        /// <summary>
        /// 대상문자열에서 첫번째 찾은 위치 ~ 두번째 찾은 위치 중간의 문자열을 반환한다.
        /// </summary>
        /// <param name="text">대상 문자열</param>
        /// <param name="start">첫번째 찾을 문자열</param>
        /// <param name="end">두번째 찾을 문자열</param>
        /// <returns>둘 중 하나라도 찾지 못한다면, 빈 문자열을 반환한다.</returns>
        public static string Between(this string text, string start, string end) {
            var idxStart = text.IndexOf(start);

            if(idxStart > -1) {
                idxStart += start.Length;
                int idxEnd = text.IndexOf(end, idxStart, StringComparison.Ordinal);

                if(idxEnd != -1)
                    return text.Substring(idxStart, idxEnd - idxStart);
            }

            return string.Empty;
        }

        /// <summary>
        /// 대상 문자열의 마지막 문자를 반환한다.
        /// </summary>
        /// <param name="text">대상 문자열</param>
        /// <returns>빈 문자열인 경우 C의 '\0' (null char)가 반환된다.</returns>
        public static char LastChar(this string text) {
            char c = '\0';

            if(text.Length > 0)
                c = text[text.Length - 1];

            return c;
        }

        #endregion

        #region << Ellipsis String >>

        /// <summary>
        /// 문자열 축약이 필요한지 검사한다
        /// </summary>
        /// <param name="text"></param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        private static bool NeedEllipsis(this string text, int maxLength) {
            return text.IsNotEmpty() && text.Length > maxLength;
        }

        /// <summary>
        /// 문자열 최대크기만큼만 SubString을 수행하고, 문자열 마지막에 줄임표(...)를 붙인다.
        /// </summary>
        /// <param name="text">트리밍할 문자열</param>
        /// <param name="maxLength">트리밍의 최대 크기</param>
        /// <returns>트리밍된 문자열</returns>
        /// <seealso cref="EllipsisPath"/>
        /// <seealso cref="EllipsisFirst"/>
        /// <example>
        /// <code>
        ///	string text="1234567890";
        /// string trimed = text.EllipsisChar(5); // trimed is "12345...";
        /// </code>
        /// </example>
        public static string EllipsisChar(this string text, int maxLength = 80) {
            if(text.IsEmpty())
                return string.Empty;

            if(text.NeedEllipsis(maxLength) == false)
                return text;

            return text.Substring(0, maxLength) + TrimmingString;
        }

        /// <summary>
        /// 문자열 최대 크기만큼만 문자열 앞과 뒷 문장을 남기고, 중간에 줄임표(...)를 붙인다.
        /// </summary>
        /// <param name="text">트리밍할 문자열</param>
        /// <param name="maxLength">트리밍의 최대 크기</param>
        /// <returns>트리밍된 문자열</returns>
        /// <seealso cref="EllipsisChar"/>
        /// <seealso cref="EllipsisFirst"/>
        /// <example>
        /// <code>
        ///	string text="1234567890";
        /// string trimed = text.EllipsisChar(5); // trimed is "123...90";
        /// </code>
        /// </example>
        public static string EllipsisPath(this string text, int maxLength = 80) {
            if(text.NeedEllipsis(maxLength) == false)
                return text;

            var length = maxLength / 2;

            var str = text.Substring(0, length) + TrimmingString;

            if(maxLength % 2 == 0)
                str += text.Substring(text.Length - length);
            else
                str += text.Substring(text.Length - length - 1);

            return str;
            // return text.Substring(0, maxLength / 2) + TrimmingString + text.Substring(text.Length - maxLength / 2);
        }

        /// <summary>
        /// 문자열 최대 크기만큼만 문자열 앞과 뒷 문장을 남기고, 중간에 줄임표(...)를 붙인다.
        /// </summary>
        /// <param name="text">트리밍할 문자열</param>
        /// <param name="maxLength">트리밍의 최대 크기</param>
        /// <returns>트리밍된 문자열</returns>
        /// <seealso cref="EllipsisChar"/>
        /// <seealso cref="EllipsisPath"/>
        /// <example>
        /// <code>
        ///	string text="1234567890";
        /// 
        /// text.EllipsisChar(5);	// return "12345...";
        /// text.EllipsisPath(5);	// return "123...90";
        /// text.EllipsisFirst(5);	// return "...67890";
        /// </code>
        /// </example>
        public static string EllipsisFirst(this string text, int maxLength = 80) {
            if(text.NeedEllipsis(maxLength) == false)
                return text;

            return TrimmingString + text.Substring(text.Length - maxLength);
        }

        #endregion

        #region << Highlight Text >>

        /// <summary>
        /// 문자열에서 검색한 단어에 Highlight를 주는 함수입니다.
        /// </summary>
        /// <param name="text">검색 대상 문자열</param>
        /// <param name="searchWord">검색할 단어</param>
        /// <param name="highlightFunc">검색된 문자를 Highlight 하는 함수</param>
        /// <returns>Highlight된 문자열</returns>
        /// <example>
        /// <code>
        /// var highlightedText = text.HighlightText("function", true, str => string.Concat("[", str, "]"));
        /// </code>
        /// </example>
        public static string HighlightText(this string text,
                                           string searchWord,
                                           Func<string, string> highlightFunc) {
            return HighlightText(text, searchWord, true, highlightFunc);
        }

        /// <summary>
        /// 문자열에서 검색한 단어에 Highlight를 주는 함수입니다.
        /// </summary>
        /// <param name="text">검색 대상 문자열</param>
        /// <param name="searchWord">검색할 단어</param>
        /// <param name="ignoreCase">검색 시 대소문자 구분 여부(기본은 True)</param>
        /// <param name="highlightFunc">검색된 문자를 Highlight 하는 함수</param>
        /// <returns>Highlight된 문자열</returns>
        /// <example>
        /// <code>
        /// var highlightedText = text.HighlightText("function", true, str => string.Concat("[", str, "]"));
        /// </code>
        /// </example>
        public static string HighlightText(this string text,
                                           string searchWord,
                                           bool ignoreCase = true,
                                           Func<string, string> highlightFunc = null) {
            if(searchWord.IsWhiteSpace())
                return text;

            var options = ((ignoreCase) ? RegexOptions.IgnoreCase : RegexOptions.None);
            var expression = new Regex(searchWord, options);

            highlightFunc = highlightFunc ?? (s => s);

            return expression.Replace(text, m => highlightFunc(m.Value));
        }

        #endregion
    }
}