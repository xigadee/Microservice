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
    public class TransmissionPayloadException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the DispatcherException class.
        /// </summary>
        public TransmissionPayloadException() : base() { }
        /// <summary>
        /// Initializes a new instance of the DispatcherException class.
        /// </summary>
        /// <param name="message">The error message.</param>
        public TransmissionPayloadException(TransmissionPayload payload) : base(payload.Message.OriginatorKey) 
        {
            Payload = payload;
        }
        /// <summary>
        /// Initializes a new instance of the DispatcherException class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="ex">The base exception.</param>
        public TransmissionPayloadException(TransmissionPayload payload, Exception ex) : base(payload.Message.OriginatorKey, ex) 
        {
            Payload = payload;
        }

        public TransmissionPayload Payload { get; private set; }
    }
}