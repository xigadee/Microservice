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
    public class MessageHandlerNotResolvedException : TransmissionPayloadException
    {
        /// <summary>
        /// Initializes a new instance of the DispatcherException class.
        /// </summary>
        public MessageHandlerNotResolvedException() : base() { }
        /// <summary>
        /// Initializes a new instance of the DispatcherException class.
        /// </summary>
        /// <param name="message">The error message.</param>
        public MessageHandlerNotResolvedException(TransmissionPayload payload) : this(payload, null) { }
        /// <summary>
        /// Initializes a new instance of the DispatcherException class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="ex">The base exception.</param>
        public MessageHandlerNotResolvedException(TransmissionPayload payload, Exception ex) : base(payload, ex) 
        {
            Header = payload.Message.ToKey();
        }

        public string Header { get; private set; }
    }
}
