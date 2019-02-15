using System;

namespace Xigadee
{
    /// <summary>
    /// This class is used to hold temporary payload and exception information.
    /// </summary>
    public class CommunicationAgentEventArgs: EventArgs
    {
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="Payload">The payload.</param>
        /// <param name="Ex">The optional exception.</param>
        public CommunicationAgentEventArgs(TransmissionPayload Payload, Exception Ex = null)
        {
            this.Payload = Payload;
            this.Ex = Ex;
        }
        /// <summary>
        /// The payload passing through the agent.
        /// </summary>
        public TransmissionPayload Payload { get; }
        /// <summary>
        /// The optional exception. 
        /// </summary>
        public Exception Ex { get; }
    }
}
