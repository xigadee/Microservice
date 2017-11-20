using System;
using System.Net;
using System.Collections.Generic;
using System.Text;

namespace Xigadee
{
    public abstract class UdpBridgeAgentBase: CommunicationBridgeAgent
    {
        public UdpBridgeAgentBase(IPAddress address, int port):base(FabricMode.NotSet)
        {

        }
    }
}
