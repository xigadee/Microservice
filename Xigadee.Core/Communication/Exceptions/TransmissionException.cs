#region using
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This is the base exception class for the CSV enumerator.
    /// </summary>
    public class TransmissionException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the XimuraException class.
        /// </summary>
        public TransmissionException() : base() { }
        /// <summary>
        /// Initializes a new instance of the XimuraException class.
        /// </summary>
        /// <param name="message">The error message.</param>
        public TransmissionException(string message) : base(message) { }
        /// <summary>
        /// Initializes a new instance of the XimuraException class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="ex">The base exception.</param>
        public TransmissionException(string message, Exception ex) : base(message, ex) { }
    }

    /// <summary>
    /// This is the base exception class for the CSV enumerator.
    /// </summary>
    public class RetryExceededTransmissionException : TransmissionException
    {
        /// <summary>
        /// Initializes a new instance of the XimuraException class.
        /// </summary>
        public RetryExceededTransmissionException() : base() { }
        /// <summary>
        /// Initializes a new instance of the XimuraException class.
        /// </summary>
        /// <param name="message">The error message.</param>
        public RetryExceededTransmissionException(string message) : base(message) { }
        /// <summary>
        /// Initializes a new instance of the XimuraException class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="ex">The base exception.</param>
        public RetryExceededTransmissionException(string message, Exception ex) : base(message, ex) { }
    }
}
