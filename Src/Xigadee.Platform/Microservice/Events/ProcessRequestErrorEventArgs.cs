using System;

namespace Xigadee
{
    /// <summary>
    /// This class is used to signal a command processing exception.
    /// </summary>
    /// <seealso cref="Xigadee.MicroserviceEventArgs" />
    public class ProcessRequestErrorEventArgs: MicroserviceEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessRequestErrorEventArgs"/> class.
        /// </summary>
        /// <param name="payload">The payload.</param>
        /// <param name="ex">The exception.</param>
        public ProcessRequestErrorEventArgs(TransmissionPayload payload, Exception ex)
        {
            Payload = payload;
            Ex = ex;
        }
        /// <summary>
        /// The originating payload.
        /// </summary>
        public TransmissionPayload Payload { get; }
        /// <summary>
        /// The uncaught exception raised during command execution.
        /// </summary>
        public Exception Ex { get;  }
    }
}
