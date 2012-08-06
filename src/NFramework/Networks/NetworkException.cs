using System;
using System.Runtime.Serialization;

namespace NSoft.NFramework.Networks {
    /// <summary>
    /// The exception that is thrown when network operation error occurs.
    /// </summary>
    [Serializable]
    public class NetworkException : InvalidOperationException {
        ///<summary>
        /// Initializes a new instance of the NSoft.NFramework.Networks.NetworkException class 
        ///</summary>
        public NetworkException() {}

        /// <summary>
        /// Initializes a new instance of the NSoft.NFramework.Networks.NetworkException class with a specified error message.
        /// </summary>
        /// <param name="message">error message</param>
        public NetworkException(string message) : base(message) {}

        /// <summary>
        /// Initializes a new instance of the NSoft.NFramework.Networks.NetworkException class with a specified error message 
        /// and a reference to the inner exception that is the cause of this exception
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">
        /// The exception that is the cause of the current exception. 
        /// If the innerException parameter is not a null reference, 
        /// the current exception is raised in a catch block that handles the inner exception.
        /// </param>
        public NetworkException(string message, Exception innerException) : base(message, innerException) {}

        /// <summary>
        /// Initializes a new instance of the  NSoft.NFramework.Networks.NetworkException class with serialized data.
        /// </summary>
        /// <param name="info">The object that holds the serialized object data.</param>
        /// <param name="context">The contextual information about the source or destination.</param>
        public NetworkException(SerializationInfo info, StreamingContext context) : base(info, context) {}
    }
}