using System;

namespace Xigadee
{
    /// <summary>
    /// This is the base interface for a communication bridge
    /// </summary>
    public interface ICommunicationBridge
    {
        /// <summary>
        /// Occurs when an exception is raised.
        /// </summary>
        event EventHandler<CommunicationBridgeAgentEventArgs> OnException;
        /// <summary>
        /// Occurs when a message is received.
        /// </summary>
        event EventHandler<CommunicationBridgeAgentEventArgs> OnReceive;
        /// <summary>
        /// Occurs before a message is transmitted.
        /// </summary>
        event EventHandler<CommunicationBridgeAgentEventArgs> OnTransmit;

        /// <summary>
        /// Gets the mode, round robin or broadcast.
        /// </summary>
        FabricMode Mode { get; }
        /// <summary>
        /// Gets a value indicating whether payloads received has all been signalled complete.
        /// </summary>
        bool PayloadsAllSignalled { get; }
        /// <summary>
        /// Gets a value indicating whether the payload history is enabled. This will store all incoming and outgoing payloads.
        /// The  default value is false.
        /// </summary>
        bool PayloadHistoryEnabled { get; }
        /// <summary>
        /// Gets a listener agent for the bridge.
        /// </summary>
        /// <returns>Returns the listener agent.</returns>
        IListener GetListener();
        /// <summary>
        /// Gets a sender for the bridge.
        /// </summary>
        /// <returns>Returns the sender agent.</returns>
        ISender GetSender();
    }
}