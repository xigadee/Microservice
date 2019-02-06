using System.Collections.Generic;
using System.Net;

namespace Xigadee
{
    /// <summary>
    /// This agent uses the Tcp communication to link Microservice applications.
    /// </summary>
    public class TcpCommunicationBridgeAgent : CommunicationBridgeAgent
    {
        List<TcpTlsChannelListener> mListeners = new List<TcpTlsChannelListener>();
        List<TcpTlsChannelSender> mSenders = new List<TcpTlsChannelSender>();
        IPEndPoint mEndPoint;

        public TcpCommunicationBridgeAgent(IPEndPoint EndPoint)
        {
            mEndPoint = EndPoint;
        }

        public override IListener GetListener()
        {
            var listener = new TcpTlsChannelListener();
            listener.EndPoint = mEndPoint;
            return listener;
        }

        public override ISender GetSender()
        {
            var sender = new TcpTlsChannelSender();
            sender.EndPoint = mEndPoint;
            return sender;
        }
    }
}
