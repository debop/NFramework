using System;
using System.Runtime.Serialization;

namespace NSoft.NFramework.Licensing {
    /// <summary>
    /// 라이선스 파일을 찾지 못했을 때 발생하는 예외입니다.
    /// </summary>
    [Serializable]
    public class LicenseFileNotFoundException : LicensingException {
        public LicenseFileNotFoundException() {}

        public LicenseFileNotFoundException(string message) : base(message) {}

        public LicenseFileNotFoundException(string message, Exception inner) : base(message, inner) {}

        public LicenseFileNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) {}
    }
}