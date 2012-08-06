using System;
using System.Runtime.Serialization;

namespace NSoft.NFramework.Licensing {
    /// <summary>
    /// 
    /// </summary>
    public class FloatingLicenseNotAvailableException : LicensingException {
        public FloatingLicenseNotAvailableException() {}

        public FloatingLicenseNotAvailableException(string message) : base(message) {}

        public FloatingLicenseNotAvailableException(string message, Exception inner) : base(message, inner) {}

        public FloatingLicenseNotAvailableException(SerializationInfo info, StreamingContext context) : base(info, context) {}
    }
}