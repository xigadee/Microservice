using System.Net;
using System.Net.Sockets;

namespace Xigadee
{
    public class UdpChannelSender : MessagingSenderBase<UdpClient, UdpReceiveResult, UdpClientHolder>
    {
        public UdpChannelSender(bool isMulticast, IPEndPoint endPoint)
        {
            IsMulticast = isMulticast;
            EndPoint = endPoint;
        }
        /// <summary>
        /// Gets a value indicating whether this instance is a multicast socket.
        /// </summary>
        bool IsMulticast { get; }
        /// <summary>
        /// Gets the end point for the UDP socket.
        /// </summary>
        IPEndPoint EndPoint { get; }

        protected override UdpClientHolder ClientCreate(SenderPartitionConfig partition)
        {
            var client = base.ClientCreate(partition);

            return client;
        }
    }
}
