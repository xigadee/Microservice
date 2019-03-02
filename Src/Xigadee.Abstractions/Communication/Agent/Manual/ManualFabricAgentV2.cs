using System;
using System.Collections.Concurrent;
namespace Xigadee
{
    public class ManualFabricAgentV2 : CommunicationBridgeAgent
    {
        public ManualFabricAgentV2(FabricMode mode) : base(mode)
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
