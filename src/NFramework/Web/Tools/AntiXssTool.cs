using Microsoft.Security.Application;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Web.Tools {
    public static class AntiXssTool {
        /// <summary>
        /// HTML 문서상의 TEXT로 안정된 문자열로 변환한다.
        /// </summary>
        /// <param name="value">입력 문자열</param>
        /// <returns>HTML 로 인코딩된 문자열</returns>
        public static string HtmlEncode(this string value) {
            if(value.IsEmpty())
                return string.Empty;

            return Encoder.HtmlEncode(value);
        }

        /// <summary>
        /// HTML 문서상의 TEXT로 안정된 문자열로 변환한다.
        /// </summary>
        /// <param name="value">입력 값</param>
        /// <param name="defaultValue">입력 값이 null 이거나 빈 문자열일 경우 대체할 기본 값</param>
        /// <returns>HTML 로 인코딩된 문자열</returns>
        public static string HtmlEncode(this object value, string defaultValue = "") {
            return HtmlEncode(value.AsText(defaultValue));
        }

        /// <summary>
        /// HTML Element의 Attribute 값을 안전한 값으로 인코딩한다.
        /// </summary>
        /// <param name="value">일반 문자열</param>
        /// <returns>HTML Element의 Attribute 값에 지정할 안전한 문자열</returns>
        public static string HtmlAttributeEncode(this string value) {
            if(value.IsEmpty())
                return string.Empty;

            return Encoder.HtmlAttributeEncode(value);
        }

        /// <summary>
        /// HTML Element의 Attribute 값을 안전한 값으로 인코딩한다.
        /// </summary>
        /// <param name="value">Attribute에 지정될 값</param>
        /// <param name="defaultValue">지정될 값이 null이거나 빈 문자열일때 대체할 문자열</param>
        /// <returns>HTML Element의 Attribute 값에 지정할 안전한 문자열</returns>
        public static string HtmlAttributeEncode(this object value, string defaultValue = "") {
            return HtmlAttributeEncode(value.AsText(defaultValue));
        }

        /// <summary>
        /// URL 주소를 Encoding 한다.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string UrlEncode(this string value) {
            if(value.IsEmpty())
                return string.Empty;

            return Encoder.UrlEncode(value);
        }

        /// <summary>
        /// URL 주소를 Encoding 한다.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string UrlEncode(this object value, string defaultValue = "") {
            return UrlEncode(value.AsText(defaultValue));
        }

        /// <summary>
        /// XML 문법에 맞는 문자열로 인코딩한다.
        /// </summary>
        /// <param name="value">원본 문자열</param>
        /// <returns>Xml 형식으로 인코딩된 문자열</returns>
        public static string XmlEncode(this string value) {
            if(value.IsEmpty())
                return string.Empty;

            return Encoder.XmlEncode(value);
        }

        /// <summary>
        /// XML 문법에 맞는 문자열로 인코딩한다.
        /// </summary>
        /// <param name="value">원본 문자열</param>
        /// <param name="defaultValue">원본이 null이거나 빈 문자열이면, 대체할 값</param>
        /// <returns>Xml 형식으로 인코딩된 문자열</returns>
        public static string XmlEncode(this object value, string defaultValue = "") {
            return XmlEncode(value.AsText(defaultValue));
        }

        /// <summary>
        /// XML Attribute Node에 들어갈 문자열을 XML 형식에 맞게끔 인코딩한다.
        /// </summary>
        /// <param name="value">원본 문자열</param>
        /// <returns>XML 포맷으로 인코딩된 문자열</returns>
        public static string XmlAttributeEncode(this string value) {
            if(value.IsEmpty())
                return string.Empty;

            return Encoder.XmlAttributeEncode(value);
        }

        /// <summary>
        /// XML Attribute Node에 들어갈 문자열을 XML 형식에 맞게끔 인코딩한다.
        /// </summary>
        /// <param name="value">원본 문자열</param>
        /// <param name="defaultValue">원본이 null이거나 빈문자열일 경우, 대체할 기본값</param>
        /// <returns>XML 포맷으로 인코딩된 문자열</returns>
        public static string XmlAttributeEncode(this object value, string defaultValue = "") {
            return XmlAttributeEncode(value.AsText(defaultValue));
        }

        /// <summary>
        /// javascript 문을 동적으로 생성할 때 script 코드 내용을 javascript 문법에 맞게끔 인코딩한다.
        /// </summary>
        /// <param name="value">인코딩할 문자열</param>
        /// <param name="flagForQuote">인용문구를 씌울 것인가?</param>
        /// <returns>javascript 형식에 맞는 문자열</returns>
        public static string JavaScriptEncode(this string value, bool flagForQuote = true) {
            return Encoder.JavaScriptEncode(value, flagForQuote);
        }

        /// <summary>
        /// javascript 문을 동적으로 생성할 때 script 코드 내용을 javascript 문법에 맞게끔 인코딩한다.
        /// </summary>
        /// <param name="value">인코딩할 문자열</param>
        /// <param name="defaultValue">대상값이 null이거나 빈문자열인 경우, 대체할 기본값</param>
        /// <returns>javascript 형식에 맞는 문자열</returns>
        public static string JavaScriptEncode(this object value, string defaultValue = "") {
            return JavaScriptEncode(value.AsText(defaultValue));
        }

        /// <summary>
        /// Visual Basic Script 문을 동적으로 생성시, script 코드 내용을 vb script 문법에 맞게끔 인코딩한다.
        /// </summary>
        /// <param name="value">대상 문자열</param>
        /// <returns>Visual Basic Script 로 인코딩한 문자열</returns>
        public static string VBScriptEncode(this string value) {
            return Encoder.VisualBasicScriptEncode(value);
        }

        /// <summary>
        /// Visual Basic Script 문을 동적으로 생성시, script 코드 내용을 vb script 문법에 맞게끔 인코딩한다.
        /// </summary>
        /// <param name="value">대상 문자열</param>
        /// <param name="defaultValue">대상 문자열이 null이거나 빈 문자열인 경우, 대체할 기본값</param>
        /// <returns>Visual Basic Script 로 인코딩한 문자열</returns>
        public static string VBScriptEncode(this object value, string defaultValue = "") {
            return VBScriptEncode(value.AsText(defaultValue));
        }
    }
}