using System;
using System.Net;
using System.Net.Sockets;

namespace Xigadee
{
    /// <summary>
    /// This sender is used to convert object to their binary format and transmit them as UDP packets.
    /// </summary>
    public class UdpChannelSender : MessagingSenderBase<UdpHelper, SerializationHolder, UdpClientHolder>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UdpChannelSender"/> class.
        /// </summary>
        /// <param name="udp">The UDP endpoint configuration.</param>
        /// <param name="contentType">The MIME Content Type which is used to identify the serializer.</param>
        /// <param name="contentEncoding">The optional content encoding for the binary blob.</param>
        public UdpChannelSender(UdpConfig udp
            , string contentType
            , string contentEncoding = null
            )
        {
            Config = udp;
            ContentType = contentType;
            ContentEncoding = contentEncoding;
        }

        /// <summary>
        /// Gets the UDP configuration.
        /// </summary>
        public UdpConfig Config { get; }

        /// <summary>
        /// Gets the serialization mime Content-type.
        /// </summary>
        public string ContentType { get; }
        /// <summary>
        /// Gets the serialization content type encoding, i.e. GZIP
        /// </summary>
        public string ContentEncoding { get; }

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

            client.Start = () =>
            {
                client.Client = client.ClientCreate();

                client.IsActive = true;
            };

            client.ClientCreate = () =>
            {
                var c = new UdpHelper(Config, UdpHelperMode.Sender);
                c.Start();
                return c;
            };

            client.ClientClose = () => client.Client.Stop();

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
