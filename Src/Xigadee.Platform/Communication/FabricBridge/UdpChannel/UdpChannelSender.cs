using System;
using System.Net;
using System.Net.Sockets;

namespace Xigadee
{
    /// <summary>
    /// This sender is used to convert object to their binary format and transmit them as UDP packets.
    /// </summary>
    public class UdpChannelSender : MessagingSenderBase<UdpClient, SerializationHolder, UdpClientHolder>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UdpChannelSender"/> class.
        /// </summary>
        /// <param name="isMulticast">if set to <c>true</c> [is multicast].</param>
        /// <param name="endPoint">The end point.</param>
        /// <param name="mimeContentType">The serialization mime type.</param>
        public UdpChannelSender(bool isMulticast, IPEndPoint endPoint, string mimeContentType)
        {
            IsMulticast = isMulticast;
            EndPoint = endPoint;
            MimeContentType = mimeContentType;
        }
        /// <summary>
        /// Gets the serialization mime type.
        /// </summary>
        public string MimeContentType { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is a multicast socket.
        /// </summary>
        bool IsMulticast { get; }
        /// <summary>
        /// Gets the end point for the UDP socket.
        /// </summary>
        IPEndPoint EndPoint { get; }
        /// <summary>
        /// This is the default client create logic.
        /// </summary>
        /// <param name="partition"></param>
        /// <returns>
        /// Returns the client.
        /// </returns>
        protected override UdpClientHolder ClientCreate(SenderPartitionConfig partition)
        {
            var client = base.ClientCreate(partition);

            return client;
        }
    }
}
