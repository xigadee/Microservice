using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Xigadee
{
    /// <summary>
    /// This listener is used to receive UDP packets and to convert the packets in to entities that can be processed by the Xigadee framework.
    /// </summary>
    public class UdpChannelListener : MessagingListenerBase<UdpClient, UdpReceiveResult, UdpClientHolder>
    {
        public UdpChannelListener(bool isMulticast, IPEndPoint endPoint, Func<UdpReceiveResult, object> convert = null)
        {
            IsMulticast = isMulticast;
            EndPoint = endPoint;
            ConvertIncoming = convert;
        }

        Func<UdpReceiveResult, object> ConvertIncoming { get; }
        /// <summary>
        /// Gets a value indicating whether this instance is a multicast socket.
        /// </summary>
        bool IsMulticast { get; }
        /// <summary>
        /// Gets the end point for the UDP socket.
        /// </summary>
        IPEndPoint EndPoint { get; }

        protected override UdpClientHolder ClientCreate(ListenerPartitionConfig partition)
        {
            var client = base.ClientCreate(partition);

            client.ClientCreate = () =>
            {
                var c = new UdpClient(EndPoint);

                if (IsMulticast)
                    c.JoinMulticastGroup(EndPoint.Address);

                return c;
            };

            client.MessageUnpack = (u) =>
            {
                var sMessage = new ServiceMessage();

                string text = Encoding.UTF8.GetString(u.Buffer);


                return sMessage;
            };

            return client;
        }

    }
}
