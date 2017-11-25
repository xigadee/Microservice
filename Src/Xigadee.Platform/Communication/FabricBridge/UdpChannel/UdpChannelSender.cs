using System;
using System.Net;
using System.Net.Sockets;

namespace Xigadee
{
    /// <summary>
    /// This sender is used to convert object to their binary format and transmit them as UDP packets.
    /// </summary>
    public class UdpChannelSender : MessagingSenderBase<UdpClient, UdpContext, UdpClientHolder>
    {
        public UdpChannelSender(bool isMulticast, IPEndPoint endPoint, Action<UdpContext> convert = null)
        {
            IsMulticast = isMulticast;
            EndPoint = endPoint;
            ConvertOutgoing = convert;
        }

        /// <summary>
        /// This is the convert function used to format outgoing messages.
        /// </summary>
        Action<UdpContext> ConvertOutgoing { get; }
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
