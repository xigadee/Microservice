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
        /// <param name="requestAddress">This is the optional address fragment which specifies the incoming message destination. If this is not set then ("","") will be used. This does not include a channelId as this will be provided by the pipeline.</param>
        /// <param name="responseAddress">This is the optional return address destination to be set for the incoming messages.</param>
        /// <param name="requestAddressPriority">This is the default priority for the request message. The default is 1.</param>
        /// <param name="responseAddressPriority">This is the priority for the response address. The default is 1.</param>
        public UdpChannelListener(bool isMulticast, IPEndPoint endPoint
            , string contentType, string contentEncoding = null
            , ServiceMessageHeaderFragment requestAddress = null
            , ServiceMessageHeader responseAddress = null
            , int? requestAddressPriority = null
            , int responseAddressPriority = 1
            )
        {
            IsMulticast = isMulticast;
            EndPoint = endPoint;
            ContentType = contentType;
            ContentEncoding = contentEncoding;
            RequestAddress = requestAddress ?? ("","");
            RequestAddressPriority = requestAddressPriority;
            ResponseAddress = responseAddress;
            ResponseAddressPriority = responseAddressPriority;
        }

        /// <summary>
        /// This is the optional address fragment which specifies the incoming message destination. If this is not set then ("","") will be used.
        /// </summary>
        public ServiceMessageHeaderFragment RequestAddress { get; }
        /// <summary>
        /// Gets the address return priority.
        /// </summary>
        public int? RequestAddressPriority { get; }
        /// <summary>
        /// This is the optional return address destination to be set for the incoming messages.
        /// </summary>
        public ServiceMessageHeader ResponseAddress { get; }
        /// <summary>
        /// Gets the address return priority.
        /// </summary>
        public int ResponseAddressPriority { get; }

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

            client.ClientClose = () => client.Client.Close();

            client.MessageUnpack = (holder) =>
            {
                if (!PayloadSerializer.TryPayloadDeserialize(holder))
                {
                    holder.SetObject(new UdpBinaryContext { Blob = holder.Blob, Endpoint = (IPEndPoint)holder.Metadata });
                }

                var sMessage = new ServiceMessage((client.MappingChannelId ?? client.ChannelId, RequestAddress), ResponseAddress);

                if (ResponseAddress != null)
                    sMessage.ResponseChannelPriority = ResponseAddressPriority;

                sMessage.ChannelPriority = RequestAddressPriority ?? client.Priority;
                sMessage.Blob = holder;

                return sMessage;
            };

            return client;
        }

    }
}
