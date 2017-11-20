using System.Net;

namespace Xigadee
{
    public abstract class UdpBridgeAgentBase: CommunicationBridgeAgent
    {
        public UdpBridgeAgentBase(IPAddress address, int port):base(FabricMode.NotSet)
        {

        }
    }
}
