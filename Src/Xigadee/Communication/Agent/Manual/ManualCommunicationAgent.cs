using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Xigadee
{
    public class ManualCommunicationAgent: CommunicationAgentBase
    {
        public override CommunicationAgentCapabilities Capabilities { get { return CommunicationAgentCapabilities.Bidirectional; } }

        protected override void ListenerClientsStart()
        {
            throw new NotImplementedException();
        }

        protected override void ListenerClientsStop()
        {
            throw new NotImplementedException();
        }

        protected override void ListenerClientValidate(IClientHolder client, List<MessageFilterWrapper> newList)
        {
            throw new NotImplementedException();
        }

        public override Task SenderTransmit(TransmissionPayload message)
        {
            throw new NotImplementedException();
        }

        protected override IClientHolder SenderClientResolve(int priority)
        {
            throw new NotImplementedException();
        }
    }
}
