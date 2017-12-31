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
        /// <param name="addressSend">This is the optional address fragment which specifies the incoming message destination. If this is not set then ("","") will be used. This does not include a channelId as this will be provided by the pipeline.</param>
        /// <param name="addressReturn">This is the optional return address destination to be set for the incoming messages.</param>
        public UdpChannelListener(bool isMulticast, IPEndPoint endPoint
            , string contentType, string contentEncoding = null
            , ServiceMessageHeaderFragment addressSend = null, ServiceMessageHeader addressReturn = null
            )
        {
            IsMulticast = isMulticast;
            EndPoint = endPoint;
            ContentType = contentType;
            ContentEncoding = contentEncoding;
            AddressSend = addressSend ?? ("","");
            AddressReturn = addressReturn;
        }

        /// <summary>
        /// This is the optional address fragment which specifies the incoming message destination. If this is not set then ("","") will be used.
        /// </summary>
        public ServiceMessageHeaderFragment AddressSend { get; }
        /// <summary>
        /// This is the optional return address destination to be set for the incoming messages.
        /// </summary>
        public ServiceMessageHeader AddressReturn { get; }

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

        /// <summary>
        /// This override sets the default processing time to the client for incoming messages.
        /// </summary>
        /// <param name="partition">The current partition.</param>
        /// <returns>
        /// Returns the new client.
        /// </returns>
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

            client.MessageUnpack = (holder) =>
            {
                if (!PayloadSerializer.TryPayloadDeserialize(holder))
                {
                    holder.Object = new UdpBinaryContext { Blob = holder.Blob, Endpoint = (IPEndPoint)holder.Metadata };
                    holder.ObjectType = typeof(UdpBinaryContext);
                }

                var sMessage = new ServiceMessage((client.ChannelId, AddressSend), AddressReturn);
                sMessage.ChannelPriority = client.Priority;
                sMessage.Blob = holder;

                return sMessage;
            };

            return client;
        }

    }
}
