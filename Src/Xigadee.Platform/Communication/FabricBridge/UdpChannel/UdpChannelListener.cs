using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Xigadee
{
    /// <summary>
    /// This listener is used to receive UDP packets and to convert the packets in to entities that can be processed by the Xigadee framework.
    /// </summary>
    public class UdpChannelListener : MessagingListenerBase<UdpClient, SerializationHolder, UdpClientHolder>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UdpChannelListener"/> class.
        /// </summary>
        /// <param name="isMulticast">Specifies whether this is a multicast connection.</param>
        /// <param name="endPoint">The IP end point to listen to.</param>
        /// <param name="contentType">The MIME Content Type which is used to identify the deserializer.</param>
        /// <param name="contentEncoding">The optional content encoding.</param>
        public UdpChannelListener(bool isMulticast, IPEndPoint endPoint, string contentType, string contentEncoding = null)
        {
            IsMulticast = isMulticast;
            EndPoint = endPoint;
            ContentType = contentType;
            ContentEncoding = contentEncoding;
        }

        /// <summary>
        /// Gets the Mime type used for deserialization.
        /// </summary>
        public string ContentType { get; }
        /// <summary>
        /// Gets the optional content encoding for the deserializer.
        /// </summary>
        public string ContentEncoding { get; }

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

            client.ContentType = ContentType;
            client.ContentEncoding = ContentEncoding;

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

                //string text = Encoding.UTF8.GetString(u.Buffer);


                return sMessage;
            };

            return client;
        }

    }
}
