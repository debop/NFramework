using System;
using System.Xml.Serialization;

namespace NSoft.NFramework.XmlData.Messages {
    /// <summary>
    /// Error 정보를 나타내는 클래스
    /// </summary>
    [Serializable]
    public class XdsError {
        #region << logger >>

        [NonSerialized] private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        /// <summary>
        /// 에러 메시지 포맷
        /// </summary>
        public const string ErrorFormat = @"Error Code=[{0}], Message=[{1}], StackTrace=[{2}], Source=[{3}]";

        #region << Constructors >>

        /// <summary>
        /// Constructor
        /// </summary>
        public XdsError() : this(0, string.Empty, string.Empty, string.Empty) {}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="code">Error code</param>
        /// <param name="msg">Error message</param>
        /// <param name="stackTrace">Stack trace</param>
        /// <param name="source">Error source</param>
        public XdsError(int code, string msg, string stackTrace, string source) {
            Code = code;
            Message = msg;
            StackTrace = StackTrace;
            Source = source;
        }

        /// <summary>
        /// Initialize a new instance of <see cref="XdsError"/> with <see cref="Exception"/>
        /// </summary>
        /// <param name="ex">instance of <see cref="Exception"/></param>
        public XdsError(Exception ex) {
            if(ex == null)
                return;

            Code = (ex.Message ?? string.Empty).GetHashCode();
            Message = ex.Message;
            StackTrace = ex.StackTrace;
            Source = ex.Source;
        }

        #endregion

        /// <summary>
        /// 에러 코드
        /// </summary>
        [XmlElement("CODE")]
        public int Code { get; set; }

        /// <summary>
        /// 에러 메시지
        /// </summary>
        [XmlElement("MESSAGE")]
        public string Message { get; set; }

        /// <summary>
        /// Stack Trace 정보
        /// </summary>
        [XmlElement("DESCRIPTION")]
        public string StackTrace { get; set; }

        /// <summary>
        /// 에러 소스
        /// </summary>
        [XmlElement("SOURCE")]
        public string Source { get; set; }

        /// <summary>
        /// Return string that represent current object
        /// </summary>
        /// <returns>string that represent current object</returns>
        public override string ToString() {
            return string.Format(ErrorFormat, Code, Message, StackTrace, Source);
        }
    }
}