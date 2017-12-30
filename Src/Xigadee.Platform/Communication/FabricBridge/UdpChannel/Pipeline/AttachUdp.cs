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
        /// <param name="preferedDeserializerMimeType">Type of the preferred deserializer MIME.</param>
        /// <param name="deserialize">The deserialize action.</param>
        /// <param name="canDeserialize">The deserialize check function.</param>
        /// <param name="action">The optional action to be called when the listener is created.</param>
        /// <returns>Returns the pipeline.</returns>
        public static C AttachUdpListener<C>(this C cpipe
            , IPEndPoint ep
            , string preferedDeserializerMimeType = null
            , Action<SerializationHolder> deserialize = null
            , Func<SerializationHolder, bool> canDeserialize = null
            , Action<UdpChannelListener> action = null
            )
            where C : IPipelineChannelIncoming<IPipeline>
        {
            var listener = new UdpChannelListener(false, ep, preferedDeserializerMimeType);

            if (deserialize != null)
                cpipe.Pipeline.AddPayloadSerializer(
                      preferedDeserializerMimeType ?? $"udp/{cpipe.Channel.Id}_{listener.ComponentId.ToString("N")}".ToLowerInvariant()
                    , deserialize: deserialize
                    , canDeserialize: canDeserialize);

            cpipe.AttachListener(listener, action, true);

            return cpipe;
        }

        /// <summary>
        /// Attaches the multicast UDP listener.
        /// </summary>
        /// <typeparam name="C">The pipeline type.</typeparam>
        /// <param name="cpipe">The pipeline.</param>
        /// <param name="ep">The UDP endpoint to listen on.</param>
        /// <param name="preferedDeserializerMimeType">Type of the preferred deserializer MIME.</param>
        /// <param name="deserialize">The deserialize action.</param>
        /// <param name="canDeserialize">The deserialize check function.</param>
        /// <param name="action">The optional action to be called when the listener is created.</param>
        /// <returns>Returns the pipeline.</returns>
        public static C AttachMulticastUdpListener<C>(this C cpipe
            , IPEndPoint ep
            , string preferedDeserializerMimeType
            , Action<SerializationHolder> deserialize = null
            , Func<SerializationHolder, bool> canDeserialize = null
            , Action<UdpChannelListener> action = null
            )
            where C : IPipelineChannelIncoming<IPipeline>
        {
            var listener = new UdpChannelListener(true, ep, preferedDeserializerMimeType);

            if (deserialize != null)
                cpipe.Pipeline.AddPayloadSerializer(preferedDeserializerMimeType, deserialize: deserialize, canDeserialize: canDeserialize);

            cpipe.AttachListener(listener, action, true);

            return cpipe;
        }


        /// <summary>
        /// Attaches the UDP listener.
        /// </summary>
        /// <typeparam name="C">The pipeline type.</typeparam>
        /// <param name="cpipe">The pipeline.</param>
        /// <param name="ep">The UDP endpoint to listen on.</param>
        /// <param name="preferedDeserializerMimeType">The preferred deserializer's MimeType.</param>
        /// <param name="serializer">This is an optional serializer that can be added with the specific mime type. Note:  the serializer mime type will be changed, so you should not share this serializer instance.</param>
        /// <param name="action">The optional action to be called when the listener is created.</param>
        /// <returns>Returns the pipeline.</returns>
        public static C AttachUdpListener<C>(this C cpipe
            , IPEndPoint ep
            , string preferedDeserializerMimeType
            , IPayloadSerializer serializer = null
            , Action<UdpChannelListener> action = null
            )
            where C : IPipelineChannelIncoming<IPipeline>
        {
            var listener = new UdpChannelListener(false, ep, preferedDeserializerMimeType);

            if (serializer != null)
                cpipe.Pipeline.AddPayloadSerializer(serializer, preferedDeserializerMimeType);

            cpipe.AttachListener(listener, action, true);

            return cpipe;
        }

        /// <summary>
        /// Attaches the multicast UDP listener.
        /// </summary>
        /// <typeparam name="C">The pipeline type.</typeparam>
        /// <param name="cpipe">The pipeline.</param>
        /// <param name="ep">The UDP endpoint to listen on.</param>
        /// <param name="preferedDeserializerMimeType">The preferred deserializer's MimeType.</param>
        /// <param name="serializer">This is an optional serializer that can be added with the specific mime type. Note:  the serializer mime type will be changed, so you should not share this serializer instance.</param>
        /// <param name="action">The optional action to be called when the listener is created.</param>
        /// <returns>Returns the pipeline.</returns>
        public static C AttachMulticastUdpListener<C>(this C cpipe
            , IPEndPoint ep
            , string preferedDeserializerMimeType
            , IPayloadSerializer serializer = null
            , Action<UdpChannelListener> action = null
            )
            where C : IPipelineChannelIncoming<IPipeline>
        {
            var listener = new UdpChannelListener(true, ep, preferedDeserializerMimeType);

            if (serializer != null)
                cpipe.Pipeline.AddPayloadSerializer(serializer, preferedDeserializerMimeType);

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
