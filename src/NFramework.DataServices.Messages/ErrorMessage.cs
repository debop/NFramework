using System;

namespace NSoft.NFramework.DataServices.Messages {
    /// <summary>
    /// 예외 정보를 나타냅니다.
    /// </summary>
    [Serializable]
    public class ErrorMessage : MessageObjectBase {
        public ErrorMessage() {}

        public ErrorMessage(Exception ex) {
            Code = ex.GetHashCode();
            Message = ex.Message;
            StackTrace = ex.StackTrace;
        }

        /// <summary>
        /// 에러 코드
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 에러 메시지
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Stack Trace 정보
        /// </summary>
        public string StackTrace { get; set; }

        public override int GetHashCode() {
            return Hasher.Compute(Code, Message);
        }

        public override string ToString() {
            return string.Format("Error# Code=[{0}], Message=[{1}], StatckTrace=[{2}]", Code, Message, StackTrace);
        }
    }
}