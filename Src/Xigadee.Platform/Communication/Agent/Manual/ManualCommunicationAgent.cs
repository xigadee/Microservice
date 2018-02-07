using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Xigadee
{
    public class ManualCommunicationAgent: CommunicationAgentBase<StatusBase>
    {
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
