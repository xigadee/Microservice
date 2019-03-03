using System;
using System.Collections.Concurrent;
namespace Xigadee
{
    public class ManualFabricAgentV2 : CommunicationFabricBridgeBase
    {
        public ManualFabricAgentV2(CommunicationFabricMode mode) : base(mode)
        {
        }

        public override IListener GetListener()
        {
            throw new NotImplementedException();
        }

        public override ISender GetSender()
        {
            throw new NotImplementedException();
        }
    }
}
