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
        /// Attaches the UDP listener.
        /// </summary>
        /// <typeparam name="C">The pipeline type.</typeparam>
        /// <param name="cpipe">The pipeline.</param>
        /// <param name="ep">The UDP endpoint to listen on.</param>
        /// <param name="defaultDeserializerContentType">Default deserializer MIME Content-type, i.e application/json.</param>
        /// <param name="defaultDeserializerContentEncoding">Default deserializer MIME Content-encoding, i.e. GZIP.</param>
        /// <param name="addressSend">This is the optional address fragment which specifies the incoming message destination. If this is not set then ("","") will be used. This does not include a channelId as this will be provided by the pipeline.</param>
        /// <param name="addressReturn">This is the optional return address destination to be set for the incoming messages.</param>
        /// <param name="deserialize">The deserialize action.</param>
        /// <param name="canDeserialize">The deserialize check function.</param>
        /// <param name="action">The optional action to be called when the listener is created.</param>
        /// <param name="isMulticast">Specifies whether this connection is a multicast connection. The default is false.</param>
        /// <returns>Returns the pipeline.</returns>
        public static C AttachUdpListener<C>(this C cpipe
            , IPEndPoint ep
            , string defaultDeserializerContentType = null
            , string defaultDeserializerContentEncoding = null
            , ServiceMessageHeaderFragment addressSend = null
            , ServiceMessageHeader addressReturn = null
            , Action<SerializationHolder> deserialize = null
            , Func<SerializationHolder, bool> canDeserialize = null
            , Action<UdpChannelListener> action = null
            , bool isMulticast = false
            )
            where C : IPipelineChannelIncoming<IPipeline>
        {
            defaultDeserializerContentType = defaultDeserializerContentType 
                ?? $"udp_in/{cpipe.Channel.Id}"
                .ToLowerInvariant();
            
            var listener = new UdpChannelListener(isMulticast, ep
                , defaultDeserializerContentType, defaultDeserializerContentEncoding
                , addressSend, addressReturn
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
        /// Attaches the UDP listener.
        /// </summary>
        /// <typeparam name="C">The pipeline type.</typeparam>
        /// <param name="cpipe">The pipeline.</param>
        /// <param name="ep">The UDP endpoint to listen on.</param>
        /// <param name="defaultDeserializerContentType">Default deserializer MIME Content-type, i.e application/json.</param>
        /// <param name="defaultDeserializerContentEncoding">Default deserializer MIME Content-encoding, i.e. GZIP.</param>
        /// <param name="addressSend">This is the optional address fragment which specifies the incoming message destination. If this is not set then ("","") will be used. This does not include a channelId as this will be provided by the pipeline.</param>
        /// <param name="addressReturn">This is the optional return address destination to be set for the incoming messages.</param>
        /// <param name="serializer">This is an optional serializer that can be added with the specific mime type. Note:  the serializer mime type will be changed, so you should not share this serializer instance.</param>
        /// <param name="action">The optional action to be called when the listener is created.</param>
        /// <param name="isMulticast">Specifies whether this connection is a multicast connection. The default is false.</param>
        /// <returns>Returns the pipeline.</returns>
        public static C AttachUdpListener<C>(this C cpipe
            , IPEndPoint ep
            , string defaultDeserializerContentType = null
            , string defaultDeserializerContentEncoding = null
            , ServiceMessageHeaderFragment addressSend = null
            , ServiceMessageHeader addressReturn = null
            , IPayloadSerializer serializer = null
            , Action<UdpChannelListener> action = null
            , bool isMulticast = false
            )
            where C : IPipelineChannelIncoming<IPipeline>
        {
            defaultDeserializerContentType = defaultDeserializerContentType 
                ?? serializer?.ContentType 
                ?? $"udp_in/{cpipe.Channel.Id}"
                .ToLowerInvariant();

            var listener = new UdpChannelListener(isMulticast, ep
                , defaultDeserializerContentType, defaultDeserializerContentEncoding
                , addressSend, addressReturn
                );

            if (serializer != null)
                cpipe.Pipeline.AddPayloadSerializer(serializer, defaultDeserializerContentType);

            cpipe.AttachListener(listener, action, true);

            return cpipe;
        }

        /// <summary>
        /// Attaches the UDP sender.
        /// </summary>
        /// <typeparam name="C">The pipeline type.</typeparam>
        /// <param name="cpipe">The pipeline.</param>
        /// <param name="ep">The UDP endpoint to transmit on.</param>
        /// <param name="preferedSerializerMimeType">Type of the preferred serializer's MIME type.</param>
        /// <param name="serializer">This is an optional serializer that can be added with the specific mime type. Note:  the serializer mime type will be changed, so you should not share this serializer instance.</param>
        /// <param name="action">The optional action to be called when the sender is created.</param>
        /// <returns>Returns the pipeline.</returns>
        public static C AttachUdpSender<C>(this C cpipe
            , IPEndPoint ep
            , string preferedSerializerMimeType
            , IPayloadSerializer serializer = null
            , Action<UdpChannelSender> action = null
            )
            where C : IPipelineChannelOutgoing<IPipeline>
        {
            var sender = new UdpChannelSender(false, ep, preferedSerializerMimeType);

            if (serializer != null)
                cpipe.Pipeline.AddPayloadSerializer(serializer, preferedSerializerMimeType);

            cpipe.AttachSender(sender, action, true);

            return cpipe;
        }

        /// <summary>
        /// Attaches the multicast UDP sender.
        /// </summary>
        /// <typeparam name="C">The pipeline type.</typeparam>
        /// <param name="cpipe">The pipeline.</param>
        /// <param name="ep">The UDP endpoint to transmit on.</param>
        /// <param name="preferedSerializerMimeType">Type of the preferred serializer's MIME type.</param>
        /// <param name="serializer">This is an optional serializer that can be added with the specific mime type. Note:  the serializer mime type will be changed, so you should not share this serializer instance.</param>
        /// <param name="action">The optional action to be called when the sender is created.</param>
        /// <returns>Returns the pipeline.</returns>
        public static C AttachMulticastUdpSender<C>(this C cpipe
            , IPEndPoint ep
            , string preferedSerializerMimeType
            , IPayloadSerializer serializer = null
            , Action<UdpChannelSender> action = null
            )
            where C : IPipelineChannelOutgoing<IPipeline>
        {
            var sender = new UdpChannelSender(true, ep, preferedSerializerMimeType);

            if (serializer != null)
                cpipe.Pipeline.AddPayloadSerializer(serializer, preferedSerializerMimeType);

            cpipe.AttachSender(sender, action, true);

            return cpipe;
        }
    }
}
