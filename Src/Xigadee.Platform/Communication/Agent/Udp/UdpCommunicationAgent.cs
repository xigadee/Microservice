using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public class UdpCommunicationAgent: CommunicationAgentBase
    {
        public override CommunicationAgentCapabilities Capabilities { get { return CommunicationAgentCapabilities.Bidirectional; } }

        public override Task SenderTransmit(TransmissionPayload message)
        {
            throw new NotImplementedException();
        }

        protected override void ListenerClientsStart()
        {
            throw new NotImplementedException();
        }

        protected override void ListenerClientsStop()
        {
            throw new NotImplementedException();
        }

        protected override void ListenerClientValidate(ClientHolder client, List<MessageFilterWrapper> newList)
        {
            throw new NotImplementedException();
        }

        protected override ClientHolder SenderClientResolve(int priority)
        {
            throw new NotImplementedException();
        }
    }
}
