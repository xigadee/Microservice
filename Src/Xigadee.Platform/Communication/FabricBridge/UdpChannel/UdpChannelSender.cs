using System;
using System.Net;
using System.Net.Sockets;

namespace Xigadee
{
    /// <summary>
    /// This sender is used to convert object to their binary format and transmit them as UDP packets.
    /// </summary>
    public class UdpChannelSender : MessagingSenderBase<UdpHelper, ServiceHandlerContext, UdpClientHolder>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UdpChannelSender"/> class.
        /// </summary>
        /// <param name="udp">The UDP endpoint configuration.</param>
        /// <param name="contentType">The MIME Content Type which is used to identify the serializer.</param>
        /// <param name="contentEncoding">The optional content encoding for the binary blob.</param>
        /// <param name="maxUdpMessagePayloadSize">This is the max UDP message payload size. The default is 508 bytes. If you set this to null, the sender will not check the size before transmitting.</param>
        public UdpChannelSender(UdpConfig udp
            , string contentType
            , string contentEncoding = null
            , int? maxUdpMessagePayloadSize = UdpHelper.PacketMaxSize
            )
        {
            Config = udp;
            ContentType = contentType;
            ContentEncoding = contentEncoding;
            UdpMessageMaximumPayloadSize = maxUdpMessagePayloadSize;
        }
        /// <summary>
        /// Gets the maximum size of the UDP message payload.
        /// </summary>
        /// <value>
        /// The maximum size of the UDP message payload.
        /// </value>
        public int? UdpMessageMaximumPayloadSize { get; }
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
                var holder = p.Message.Holder;

                if (holder.ContentType == null)
                    holder.ContentType = ContentType;

                if (holder.ContentEncoding == null)
                    holder.ContentEncoding = ContentEncoding;

                if (!ServiceHandlers.TryPayloadSerialize(holder))
                {
                    throw new ArgumentException("Cannot serialize.");
                }

                //Do a bounds check for the binary payload size.
                if (UdpMessageMaximumPayloadSize.HasValue && holder.Blob.Length > UdpMessageMaximumPayloadSize)
                {
                    //throw new 
                }

                return holder;
            };

            return client;
        }
    }
}
