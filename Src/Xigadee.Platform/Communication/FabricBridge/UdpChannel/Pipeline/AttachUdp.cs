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
            , Action<SerializationHolder> deserialize = null
            , Func<SerializationHolder, bool> canDeserialize = null
            , Action<UdpChannelListener> action = null
            )
            where C : IPipelineChannelIncoming<IPipeline>
        {
            defaultDeserializerContentType = (
                defaultDeserializerContentType 
                ?? $"udp_in/{cpipe.Channel.Id}"
                ).ToLowerInvariant();
            
            var listener = new UdpChannelListener(udp
                , defaultDeserializerContentType, defaultDeserializerContentEncoding
                , requestAddress, responseAddress, requestAddressPriority, responseAddressPriority
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
        /// <param name="defaultDeserializerContentType">Default deserializer MIME Content-type, i.e application/json.</param>
        /// <param name="defaultDeserializerContentEncoding">Default deserializer MIME Content-encoding, i.e. GZIP.</param>
        /// <param name="requestAddress">This is the optional address fragment which specifies the incoming message destination. If this is not set then ("","") will be used. This does not include a channelId as this will be provided by the pipeline.</param>
        /// <param name="responseAddress">This is the optional return address destination to be set for the incoming messages.</param>
        /// <param name="requestAddressPriority">This is the default priority for the request message. The default is null. This will inherit from the channel priority.</param>
        /// <param name="responseAddressPriority">This is the priority for the response address. The default is 1.</param>
        /// <param name="serializer">This is an optional serializer that can be added with the specific mime type. Note:  the serializer mime type will be changed, so you should not share this serializer instance.</param>
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
            , IPayloadSerializer serializer = null
            , Action<UdpChannelListener> action = null
            )
            where C : IPipelineChannelIncoming<IPipeline>
        {
            defaultDeserializerContentType = (
                defaultDeserializerContentType 
                ?? serializer?.ContentType 
                ?? $"udp_in/{cpipe.Channel.Id}"
                ).ToLowerInvariant();

            var listener = new UdpChannelListener(udp
                , defaultDeserializerContentType, defaultDeserializerContentEncoding
                , requestAddress, responseAddress, requestAddressPriority, responseAddressPriority
                );

            if (serializer != null)
                cpipe.Pipeline.AddPayloadSerializer(serializer, defaultDeserializerContentType);

            cpipe.AttachListener(listener, action, true);

            return cpipe;
        }

        /// <summary>
        /// Attaches the UDP sender to the outgoing channel.
        /// </summary>
        /// <typeparam name="C">The pipeline type.</typeparam>
        /// <param name="cpipe">The pipeline.</param>
        /// <param name="udp">The UDP endpoint configuration.</param>
        /// <param name="defaultSerializerContentType">Default serializer MIME Content-type, i.e application/json.</param>
        /// <param name="defaultSerializerContentTypeEncoding">Default serializer MIME Content-encoding, i.e. GZIP.</param>
        /// <param name="serializer">This is an optional serializer that can be added with the specific mime type. Note:  the serializer mime type will be changed, so you should not share this serializer instance.</param>
        /// <param name="action">The optional action to be called when the sender is created.</param>
        /// <returns>Returns the pipeline.</returns>
        public static C AttachUdpSender<C>(this C cpipe
            , UdpConfig udp
            , string defaultSerializerContentType = null
            , string defaultSerializerContentTypeEncoding = null
            , IPayloadSerializer serializer = null
            , Action<UdpChannelSender> action = null
            )
            where C : IPipelineChannelOutgoing<IPipeline>
        {
            defaultSerializerContentType = (
                defaultSerializerContentType
                ?? serializer?.ContentType
                ?? $"udp_out/{cpipe.Channel.Id}"
                ).ToLowerInvariant();

            var sender = new UdpChannelSender(udp
                , defaultSerializerContentType, defaultSerializerContentTypeEncoding);

            if (serializer != null)
                cpipe.Pipeline.AddPayloadSerializer(serializer, defaultSerializerContentType);

            cpipe.AttachSender(sender, action, true);

            return cpipe;
        }

        /// <summary>
        /// Attaches the UDP sender to the outgoing channel.
        /// </summary>
        /// <typeparam name="C">The pipeline type.</typeparam>
        /// <param name="cpipe">The pipeline.</param>
        /// <param name="udp">The UDP endpoint configuration.</param>
        /// <param name="defaultSerializerContentType">Default serializer MIME Content-type, i.e application/json.</param>
        /// <param name="defaultSerializerContentTypeEncoding">Default serializer MIME Content-encoding, i.e. GZIP.</param>
        /// <param name="serialize">The serialize action.</param>
        /// <param name="canSerialize">The optional serialize check function.</param>
        /// <param name="action">The optional action to be called when the sender is created.</param>
        /// <returns>Returns the pipeline.</returns>
        public static C AttachUdpSender<C>(this C cpipe
            , UdpConfig udp
            , string defaultSerializerContentType = null
            , string defaultSerializerContentTypeEncoding = null
            , Action<SerializationHolder> serialize = null
            , Func<SerializationHolder, bool> canSerialize = null
            , Action<UdpChannelSender> action = null
            )
            where C : IPipelineChannelOutgoing<IPipeline>
        {
            defaultSerializerContentType = (
                defaultSerializerContentType
                ?? $"udp_out/{cpipe.Channel.Id}"
                ).ToLowerInvariant();

            var sender = new UdpChannelSender(udp, defaultSerializerContentType, defaultSerializerContentTypeEncoding);

            if (serialize != null)
                cpipe.Pipeline.AddPayloadSerializer(defaultSerializerContentType
                    , serialize: serialize
                    , canSerialize: canSerialize);

            cpipe.AttachSender(sender, action, true);

            return cpipe;
        }

    }
}
