using System;
using System.Runtime.Serialization;

namespace NSoft.NFramework.Licensing {
    /// <summary>
    /// 라이선스 예외의 기본 클래스입니다.
    /// </summary>
    [Serializable]
    public class LicensingException : ApplicationException {
        protected LicensingException() {}

        protected LicensingException(string message) : base(message) {}

        protected LicensingException(string message, Exception inner) : base(message, inner) {}

        protected LicensingException(SerializationInfo info, StreamingContext context) : base(info, context) {}
    }
}