using System;
using System.Runtime.Serialization;

namespace NSoft.NFramework.Networks {
    /// <summary>
    /// The exception that is thrown when ftp operation error occurs.
    /// </summary>
    [Serializable]
    public class FtpException : NetworkException {
        ///<summary>
        /// Initializes a new instance of the NSoft.NFramework.Networks.Ftp.FtpException class 
        ///</summary>
        public FtpException() {}

        /// <summary>
        /// Initializes a new instance of the NSoft.NFramework.Networks.Ftp.FtpException class with a specified error message.
        /// </summary>
        /// <param name="message">error message</param>
        public FtpException(string message) : base(message) {}

        /// <summary>
        /// Initializes a new instance of the NSoft.NFramework.Networks.Ftp.FtpException class with a specified error message 
        /// and a reference to the inner exception that is the cause of this exception
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">
        /// The exception that is the cause of the current exception. 
        /// If the innerException parameter is not a null reference, 
        /// the current exception is raised in a catch block that handles the inner exception.
        /// </param>
        public FtpException(string message, Exception innerException) : base(message, innerException) {}

        /// <summary>
        /// Initializes a new instance of the NSoft.NFramework.Networks.Ftp.FtpException class with serialized data.
        /// </summary>
        /// <param name="info">The object that holds the serialized object data.</param>
        /// <param name="context">The contextual information about the source or destination.</param>
        public FtpException(SerializationInfo info, StreamingContext context) : base(info, context) {}
    }
}