using System;
using System.Runtime.Serialization;

namespace NSoft.NFramework.Licensing {
    /// <summary>
    /// 유효기간이 지난 라이선스일 경우 발생하는 예외입니다.
    /// </summary>
    [Serializable]
    public class LicenseExpiredException : LicensingException {
        public LicenseExpiredException() {}

        public LicenseExpiredException(string message) : base(message) {}

        public LicenseExpiredException(string message, Exception inner) : base(message, inner) {}

        public LicenseExpiredException(SerializationInfo info, StreamingContext context) : base(info, context) {}
    }
}