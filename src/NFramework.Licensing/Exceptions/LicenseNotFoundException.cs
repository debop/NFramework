using System;
using System.Runtime.Serialization;

namespace NSoft.NFramework.Licensing {
    /// <summary>
    /// 라이선스를 찾을 수 없을 때 발생하는 예외.
    /// </summary>
    public class LicenseNotFoundException : LicensingException {
        public LicenseNotFoundException() {}

        public LicenseNotFoundException(string message) : base(message) {}

        public LicenseNotFoundException(string message, Exception inner) : base(message, inner) {}

        public LicenseNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) {}
    }
}