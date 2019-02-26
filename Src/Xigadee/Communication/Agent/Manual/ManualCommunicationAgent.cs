using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Xigadee
{
    public class ManualCommunicationAgent: CommunicationAgentBase
    {
        public ManualCommunicationAgent(ServiceHandlerIdCollection shIds = null) 
            : base(CommunicationAgentCapabilities.Bidirectional, shIds)
        {

        }

        public override IClientHolderV2 SenderCreate(SenderPartitionConfig p)
        {
            throw new NotImplementedException();
        }

        public override void SenderStop(IClientHolderV2 client)
        {
            throw new NotImplementedException();
        }

        protected override IClientHolderV2 ListenerClientCreate(ListenerPartitionConfig p)
        {
            throw new NotImplementedException();
        }

        protected override void ListenerClientStop(IClientHolderV2 client)
        {
            throw new NotImplementedException();
        }

        protected override void ListenerClientValidate(IClientHolderV2 client, List<MessageFilterWrapper> newList)
        {
            throw new NotImplementedException();
        }
    }
}
