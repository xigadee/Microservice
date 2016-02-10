#region using
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This is the base exception class for the DispatcherException
    /// </summary>
    public class SenderNotResolvedException : TransmissionPayloadException
    {
        /// <summary>
        /// Initializes a new instance of the DispatcherException class.
        /// </summary>
        public SenderNotResolvedException() : base() { }
        /// <summary>
        /// Initializes a new instance of the DispatcherException class.
        /// </summary>
        /// <param name="message">The error message.</param>
        public SenderNotResolvedException(TransmissionPayload payload) : base(payload) { }
        /// <summary>
        /// Initializes a new instance of the DispatcherException class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="ex">The base exception.</param>
        public SenderNotResolvedException(TransmissionPayload payload, Exception ex) : base(payload, ex) { }


    }
}
