using System;

namespace Xigadee
{
    public interface ICommunicationBridge
    {
        event EventHandler<CommunicationBridgeAgentEventArgs> OnException;
        event EventHandler<CommunicationBridgeAgentEventArgs> OnReceive;
        event EventHandler<CommunicationBridgeAgentEventArgs> OnTransmit;

        CommunicationBridgeMode Mode { get; }

        bool PayloadsAllSignalled { get; }

        bool PayloadHistoryEnabled { get; }

        IListener GetListener();

        ISender GetSender();
    }
}