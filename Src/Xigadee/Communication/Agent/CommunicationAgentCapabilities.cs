using System;

namespace Xigadee
{
    /// <summary>
    /// This enumeration describes the capabilities of the communication agent.
    /// </summary>
    [Flags]
    public enum CommunicationAgentCapabilities
    {
        /// <summary>
        /// The agent supports a listener mode to receive messages.
        /// </summary>
        Listener = 1,
        /// <summary>
        /// The agent supports a sender mode to transmit messages.
        /// </summary>
        Sender = 2,
        /// <summary>
        /// The agent is bidirectional.
        /// </summary>
        Bidirectional = 3
    }
}
