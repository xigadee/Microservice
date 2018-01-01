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
        /// <param name="isMulticast">Set to true if this is a multicast sender.</param>
        /// <param name="localEndPoint">The local end point.</param>
        /// <param name="contentType">The MIME Content Type which is used to identify the serializer.</param>
        /// <param name="contentEncoding">The optional content encoding for the binary blob.</param>
        public UdpChannelSender(bool isMulticast
            , IPEndPoint localEndPoint
            , string contentType
            , string contentEncoding = null
            , IPEndPoint multicastEndpoint = null
            )
        {
            IsMulticast = isMulticast;
            LocalEndPoint = localEndPoint;
            ContentType = contentType;
            ContentEncoding = contentEncoding;
            MulticastEndPoint = multicastEndpoint;
        }

        /// <summary>
        /// Gets the serialization mime Content-type.
        /// </summary>
        public string ContentType { get; }
        /// <summary>
        /// Gets the serialization content type encoding, i.e. GZIP
        /// </summary>
        public string ContentEncoding { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is a multicast socket.
        /// </summary>
        bool IsMulticast { get; }
        /// <summary>
        /// Gets the end point for the UDP socket.
        /// </summary>
        IPEndPoint LocalEndPoint { get; }

        IPEndPoint MulticastEndPoint { get; }
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

            client.ContentType = ContentType;
            client.ContentEncoding = ContentEncoding;
            client.LocalEndpoint = LocalEndPoint;

            client.Start = () =>
            {
                client.Client = client.ClientCreate();
                client.IsActive = true;
            };

            client.ClientClose = () =>
            {
                client.IsActive = false;

                if (IsMulticast)
                    client.Client.DropMulticastGroup(LocalEndPoint.Address);

                client.Client.Close();
            };


            client.ClientCreate = () =>
            {
                var c = new UdpClient();
                c.EnableBroadcast = true;

                if (IsMulticast)
                    c.JoinMulticastGroup(LocalEndPoint.Address);

                c.Connect(LocalEndPoint);

                return c;
            };

            client.MessagePack = (p) =>
            {
                var holder = p.Message.Blob;

                if (holder.ContentType == null)
                    holder.ContentType = ContentType;

                if (holder.ContentEncoding == null)
                    holder.ContentEncoding = ContentEncoding;

                if (!PayloadSerializer.TryPayloadSerialize(holder))
                {
                    throw new ArgumentException("Cannot serialize.");
                }

                return holder;
            };

            return client;
        }
    }
}
