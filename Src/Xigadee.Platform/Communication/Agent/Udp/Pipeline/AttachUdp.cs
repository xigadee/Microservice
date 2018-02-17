using System;
using System.Net;

namespace Xigadee
{
    /// <summary>
    /// These extensions are used to attach a UDP based listener and sender to a channel
    /// </summary>
    public static partial class UdpCommunicationPipelineExtensions
    {
        /// <summary>
        /// Attaches the UDP listener to the incoming channel.
        /// </summary>
        /// <typeparam name="C">The pipeline type.</typeparam>
        /// <param name="cpipe">The pipeline.</param>
        /// <param name="udp">The UDP endpoint configuration.</param>
        /// <param name="defaultDeserializerContentType">Default deserializer MIME Content-type, i.e application/json.</param>
        /// <param name="defaultDeserializerContentEncoding">Default deserializer MIME Content-encoding, i.e. GZIP.</param>
        /// <param name="requestAddress">This is the optional address fragment which specifies the incoming message destination. If this is not set then ("","") will be used. This does not include a channelId as this will be provided by the pipeline.</param>
        /// <param name="responseAddress">This is the optional return address destination to be set for the incoming messages.</param>
        /// <param name="requestAddressPriority">This is the default priority for the request message. The default is null. This will inherit from the channel priority.</param>
        /// <param name="responseAddressPriority">This is the priority for the response address. The default is 1.</param>
        /// <param name="deserialize">The deserialize action.</param>
        /// <param name="canDeserialize">The deserialize check function.</param>
        /// <param name="action">The optional action to be called when the listener is created.</param>
        /// <returns>Returns the pipeline.</returns>
        public static C AttachUdpListener<C>(this C cpipe
            , UdpConfig udp
            , string defaultDeserializerContentType = null
            , string defaultDeserializerContentEncoding = null
            , ServiceMessageHeaderFragment requestAddress = null
            , ServiceMessageHeader responseAddress = null
            , int? requestAddressPriority = null
            , int responseAddressPriority = 1
            , Action<ServiceHandlerContext> deserialize = null
            , Func<ServiceHandlerContext, bool> canDeserialize = null
            , Action<IListener> action = null
            )
            where C : IPipelineChannelIncoming<IPipeline>
        {
            defaultDeserializerContentType = (defaultDeserializerContentType ?? $"udp_in/{cpipe.Channel.Id}").ToLowerInvariant();
            
            var listener = new UdpCommunicationAgent(udp
                , defaultDeserializerContentType, defaultDeserializerContentEncoding, null
                , requestAddress, responseAddress, requestAddressPriority, responseAddressPriority
                , CommunicationAgentCapabilities.Listener
                );

            if (deserialize != null)
                cpipe.Pipeline.AddPayloadSerializer(
                      defaultDeserializerContentType
                    , deserialize: deserialize
                    , canDeserialize: canDeserialize);

            cpipe.AttachListener(listener, action, true);

            return cpipe;
        }

        /// <summary>
        /// Attaches the UDP listener to the incoming channel.
        /// </summary>
        /// <typeparam name="C">The pipeline type.</typeparam>
        /// <param name="cpipe">The pipeline.</param>
        /// <param name="udp">The UDP endpoint configuration.</param>
        /// <param name="serializerId">Default serializer MIME Content-type id, i.e application/json.</param>
        /// <param name="compressionId">Default serializer MIME Content-encoding id, i.e. GZIP.</param>
        /// <param name="encryptionId">The encryption handler id.</param>
        /// <param name="requestAddress">This is the optional address fragment which specifies the incoming message destination. If this is not set then ("","") will be used. This does not include a channelId as this will be provided by the pipeline.</param>
        /// <param name="responseAddress">This is the optional return address destination to be set for the incoming messages.</param>
        /// <param name="requestAddressPriority">This is the default priority for the request message. The default is null. This will inherit from the channel priority.</param>
        /// <param name="responseAddressPriority">This is the priority for the response address. The default is 1.</param>
        /// <param name="serializer">This is an optional serializer that can be added with the specific mime type. Note:  the serializer mime type will be changed, so you should not share this serializer instance.</param>
        /// <param name="action">The optional action to be called when the listener is created.</param>
        /// <returns>Returns the pipeline.</returns>
        public static C AttachUdpListener<C>(this C cpipe
            , UdpConfig udp
            , SerializationHandlerId serializerId = null
            , CompressionHandlerId compressionId = null
            , EncryptionHandlerId encryptionId = null
            , ServiceMessageHeaderFragment requestAddress = null
            , ServiceMessageHeader responseAddress = null
            , int? requestAddressPriority = null
            , int responseAddressPriority = 1
            , IServiceHandlerSerialization serializer = null
            , Action<IListener> action = null
            )
            where C : IPipelineChannelIncoming<IPipeline>
        {
            serializerId = (serializerId?.Id ?? serializer?.Id ?? $"udp_in/{cpipe.Channel.Id}").ToLowerInvariant();

            var listener = new UdpCommunicationAgent(udp
                , serializerId, compressionId, encryptionId
                , requestAddress, responseAddress, requestAddressPriority, responseAddressPriority
                , CommunicationAgentCapabilities.Listener
                );

            if (serializer != null)
                cpipe.Pipeline.AddPayloadSerializer(serializer);

            cpipe.AttachListener(listener, action, true);

            return cpipe;
        }

        /// <summary>
        /// Attaches the UDP sender to the outgoing channel.
        /// </summary>
        /// <typeparam name="C">The pipeline type.</typeparam>
        /// <param name="cpipe">The pipeline.</param>
        /// <param name="udp">The UDP endpoint configuration.</param>
        /// <param name="serializerId">Default serializer MIME Content-type id, i.e application/json.</param>
        /// <param name="compressionId">Default serializer MIME Content-encoding id, i.e. GZIP.</param>
        /// <param name="encryptionId">The encryption handler id.</param>
        /// <param name="serializer">This is an optional serializer that can be added with the specific mime type. Note:  the serializer mime type will be changed, so you should not share this serializer instance.</param>
        /// <param name="action">The optional action to be called when the sender is created.</param>
        /// <param name="maxUdpMessagePayloadSize">This is the max UDP message payload size. The default is 508 bytes. If you set this to null, the sender will not check the size before transmitting.</param>
        /// <returns>Returns the pipeline.</returns>
        public static C AttachUdpSender<C>(this C cpipe
            , UdpConfig udp
            , SerializationHandlerId serializerId = null
            , CompressionHandlerId compressionId = null
            , EncryptionHandlerId encryptionId = null
            , IServiceHandlerSerialization serializer = null
            , Action<ISender> action = null
            , int? maxUdpMessagePayloadSize = UdpHelper.PacketMaxSize
            )
            where C : IPipelineChannelOutgoing<IPipeline>
        {
            serializerId = (
                serializerId?.Id
                ?? serializer?.Id
                ?? $"udp_out/{cpipe.Channel.Id}"
                ).ToLowerInvariant();

            var sender = new UdpCommunicationAgent(udp, serializerId, compressionId, encryptionId
                , capabilities: CommunicationAgentCapabilities.Sender
                , maxUdpMessagePayloadSize: maxUdpMessagePayloadSize);

            if (serializer != null)
                cpipe.Pipeline.AddPayloadSerializer(serializer);

            cpipe.AttachSender(sender, action, true);

            return cpipe;
        }

        /// <summary>
        /// Attaches the UDP sender to the outgoing channel.
        /// </summary>
        /// <typeparam name="C">The pipeline type.</typeparam>
        /// <param name="cpipe">The pipeline.</param>
        /// <param name="udp">The UDP endpoint configuration.</param>
        /// <param name="serializerId">Default serializer MIME Content-type id, i.e application/json.</param>
        /// <param name="compressionId">Default serializer MIME Content-encoding id, i.e. GZIP.</param>
        /// <param name="encryptionId">The encryption handler id.</param>
        /// <param name="serialize">The serialize action.</param>
        /// <param name="canSerialize">The optional serialize check function.</param>
        /// <param name="action">The optional action to be called when the sender is created.</param>
        /// <param name="maxUdpMessagePayloadSize">This is the max UDP message payload size. The default is 508 bytes. If you set this to null, the sender will not check the size before transmitting.</param>
        /// <returns>Returns the pipeline.</returns>
        public static C AttachUdpSender<C>(this C cpipe
            , UdpConfig udp
            , SerializationHandlerId serializerId = null
            , CompressionHandlerId compressionId = null
            , EncryptionHandlerId encryptionId = null
            , Action<ServiceHandlerContext> serialize = null
            , Func<ServiceHandlerContext, bool> canSerialize = null
            , Action<ISender> action = null
            , int? maxUdpMessagePayloadSize = UdpHelper.PacketMaxSize
            )
            where C : IPipelineChannelOutgoing<IPipeline>
        {
            serializerId = (
                serializerId?.Id?? $"udp_out/{cpipe.Channel.Id}"
                ).ToLowerInvariant();

            var sender = new UdpCommunicationAgent(udp, serializerId, compressionId, encryptionId
                , capabilities: CommunicationAgentCapabilities.Sender
                , maxUdpMessagePayloadSize: maxUdpMessagePayloadSize
                );

            if (serialize != null)
                cpipe.Pipeline.AddPayloadSerializer(serializerId
                    , serialize: serialize
                    , canSerialize: canSerialize);

            cpipe.AttachSender(sender, action, true);

            return cpipe;
        }

    }
}
