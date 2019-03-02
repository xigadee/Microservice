using System;
namespace Xigadee
{
    /// <summary>
    /// This is the base exception class for the CSV enumerator.
    /// </summary>
    public class TransmissionException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the TransmissionException class.
        /// </summary>
        public TransmissionException() : base() { }
        /// <summary>
        /// Initializes a new instance of the TransmissionException class.
        /// </summary>
        /// <param name="message">The error message.</param>
        public TransmissionException(string message) : base(message) { }
        /// <summary>
        /// Initializes a new instance of the TransmissionException class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="ex">The base exception.</param>
        public TransmissionException(string message, Exception ex) : base(message, ex) { }
    }
}
