using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Xigadee
{
    public class UdpChannelListener : MessagingListenerBase<UdpClient, UdpReceiveResult, UdpClientHolder>
    {
        public UdpChannelListener(bool isMulticast, IPEndPoint endPoint)
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

        protected override void ClientStop(UdpClientHolder client)
        {
            base.ClientStop(client);
        }
    }
}
