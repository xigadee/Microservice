using System;

namespace Xigadee
{
    /// <summary>
    /// This is the base exception class for the DispatcherException
    /// </summary>
    public class SenderNotResolvedException : DispatcherException
    {
        /// <summary>
        /// Initializes a new instance of the DispatcherException class.
        /// </summary>
        public SenderNotResolvedException() : base() { }
        /// <summary>
        /// Initializes a new instance of the DispatcherException class.
        /// </summary>
        /// <param name="payload">The payload.</param>
        public SenderNotResolvedException(TransmissionPayload payload) : base(payload) { }
        /// <summary>
        /// Initializes a new instance of the DispatcherException class.
        /// </summary>
        /// <param name="payload">The payload.</param>
        /// <param name="ex">The exception.</param>
        public SenderNotResolvedException(TransmissionPayload payload, Exception ex) : base(payload, ex) { }


    }
}
