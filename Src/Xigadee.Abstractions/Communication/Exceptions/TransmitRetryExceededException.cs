using System;
namespace Xigadee
{

    /// <summary>
    /// This is the base exception class for the CSV enumerator.
    /// </summary>
    public class RetryExceededTransmissionException : TransmissionException
    {
        /// <summary>
        /// Initializes a new instance of the RetryExceededTransmissionException class.
        /// </summary>
        public RetryExceededTransmissionException() : base() { }
        /// <summary>
        /// Initializes a new instance of the RetryExceededTransmissionException class.
        /// </summary>
        /// <param name="message">The error message.</param>
        public RetryExceededTransmissionException(string message) : base(message) { }
        /// <summary>
        /// Initializes a new instance of the RetryExceededTransmissionException class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="ex">The base exception.</param>
        public RetryExceededTransmissionException(string message, Exception ex) : base(message, ex) { }
    }
}
