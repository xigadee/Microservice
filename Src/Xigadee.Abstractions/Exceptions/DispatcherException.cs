using System;
namespace Xigadee
{
    /// <summary>
    /// This is the base exception class for the DispatcherException
    /// </summary>
    public class DispatcherException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the DispatcherException class.
        /// </summary>
        public DispatcherException() : base() { }
        /// <summary>
        /// Initializes a new instance of the DispatcherException class.
        /// </summary>
        /// <param name="payload">The error message.</param>
        public DispatcherException(TransmissionPayload payload) : base(payload.Message.OriginatorKey) 
        {
            Payload = payload;
        }
        /// <summary>
        /// Initializes a new instance of the DispatcherException class.
        /// </summary>
        /// <param name="payload">The error message.</param>
        /// <param name="ex">The base exception.</param>
        public DispatcherException(TransmissionPayload payload, Exception ex) : base(payload.Message.OriginatorKey, ex) 
        {
            Payload = payload;
        }
        /// <summary>
        /// This is the payload that has caused the exception
        /// </summary>
        public TransmissionPayload Payload { get; private set; }

        /// <summary>
        /// This property identifies whether the exception is transient and can be retried.
        /// The default is no.
        /// </summary>
        public virtual bool CanRetry { get { return false; } }
    }
}