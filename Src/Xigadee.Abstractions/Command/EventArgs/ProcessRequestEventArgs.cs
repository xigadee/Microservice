using System;

namespace Xigadee
{
    /// <summary>
    /// This class is used when processing an event.
    /// </summary>
    /// <seealso cref="Xigadee.CommandEventArgsBase" />
    public class ProcessRequestEventArgs: CommandEventArgsBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessRequestEventArgs"/> class.
        /// </summary>
        /// <param name="payload">The payload.</param>
        /// <param name="ex">The optional exception.</param>
        public ProcessRequestEventArgs(TransmissionPayload payload, Exception ex = null)
        {
            Payload = payload;
            Ex = ex;
        }
        /// <summary>
        /// Gets the payload.
        /// </summary>
        public TransmissionPayload Payload { get; }
        /// <summary>
        /// Gets or sets the exception.
        /// </summary>
        public Exception Ex { get; set;}
    }
}
