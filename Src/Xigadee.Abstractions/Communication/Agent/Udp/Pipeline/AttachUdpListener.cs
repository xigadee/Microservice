using System;

namespace Xigadee
{
    /// <summary>
    /// These extensions are used to attach a UDP based listener and sender to a channel
    /// </summary>
    public static partial class CorePipelineExtensionsCore
    {
        /// <summary>
        /// Attaches the UDP listener to the incoming channel.
        /// </summary>
        /// <typeparam name="C">The pipeline type.</typeparam>
        /// <param name="cpipe">The pipeline.</param>
        /// <param name="udpDefault">The UDP endpoint configuration.</param>
        /// <param name="defaultDeserializerContentType">Default deserializer MIME Content-type, i.e application/json.</param>
        /// <param name="defaultDeserializerContentEncoding">Default deserializer MIME Content-encoding, i.e. GZIP.</param>
        /// <param name="requestAddress">This is the optional address fragment which specifies the incoming message destination. If this is not set then ("","") will be used. This does not include a channelId as this will be provided by the pipeline.</param>
        /// <param name="responseAddress">This is the optional return address destination to be set for the incoming messages.</param>
        /// <param name="requestAddressPriority">This is the default priority for the request message. The default is null. This will inherit from the channel priority.</param>
        /// <param name="responseAddressPriority">This is the priority for the response address. The default is 1.</param>
        /// <param name="deserialize">The deserialize action.</param>
        /// <param name="canDeserialize">The deserialize check function.</param>
        /// <param name="action">The optional action to be called when the listener is created.</param>
        /// <param name="udpExtended">The extended udp configuration.</param>
        /// <returns>Returns the pipeline.</returns>
        public static C AttachUdpListener<C>(this C cpipe
            , UdpConfig udpDefault = null
            , string defaultDeserializerContentType = null
            , string defaultDeserializerContentEncoding = null
            , ServiceMessageHeaderFragment requestAddress = null
            , ServiceMessageHeader responseAddress = null
            , int? requestAddressPriority = null
            , int responseAddressPriority = 1
            , Action<ServiceHandlerContext> deserialize = null
            , Func<ServiceHandlerContext, bool> canDeserialize = null
            , Action<IListener> action = null
            , (int priority, UdpConfig config)[] udpExtended = null
            )
            where C : IPipelineChannelIncoming<IPipeline>
        {
            IServiceHandlerSerialization serializer = null;
            var shIdColl = new ServiceHandlerCollectionContext();


            if (deserialize != null)
            {
                defaultDeserializerContentType = (defaultDeserializerContentType ?? $"udp_in/{cpipe.Channel.Id}").ToLowerInvariant();
                serializer = CreateDynamicSerializer(defaultDeserializerContentType, deserialize: deserialize, canDeserialize: canDeserialize);
                shIdColl.Serialization = serializer.Id;
            }

            return cpipe.AttachUdpListener(
                  udpDefault
                , shIdColl
                , requestAddress
                , responseAddress
                , requestAddressPriority
                , responseAddressPriority
                , serializer
                , action
                , udpExtended
                );
        }

        /// <summary>
        /// Attaches the UDP listener to the incoming channel.
        /// </summary>
        /// <typeparam name="C">The pipeline type.</typeparam>
        /// <param name="cpipe">The pipeline.</param>
        /// <param name="udpDefault">The UDP endpoint configuration.</param>
        /// <param name="shIdColl">Default service handler id collection.</param>
        /// <param name="requestAddress">This is the optional address fragment which specifies the incoming message destination. If this is not set then ("","") will be used. This does not include a channelId as this will be provided by the pipeline.</param>
        /// <param name="responseAddress">This is the optional return address destination to be set for the incoming messages.</param>
        /// <param name="requestAddressPriority">This is the default priority for the request message. The default is null. This will inherit from the channel priority.</param>
        /// <param name="responseAddressPriority">This is the priority for the response address. The default is 1.</param>
        /// <param name="serializer">This is an optional serializer that can be added with the specific mime type. Note:  the serializer mime type will be changed, so you should not share this serializer instance.</param>
        /// <param name="action">The optional action to be called when the listener is created.</param>
        /// <param name="udpExtended">The extended udp configuration.</param>
        /// <returns>Returns the pipeline.</returns>
        public static C AttachUdpListener<C>(this C cpipe
            , UdpConfig udpDefault = null
            , ServiceHandlerCollectionContext shIdColl = null
            , ServiceMessageHeaderFragment requestAddress = null
            , ServiceMessageHeader responseAddress = null
            , int? requestAddressPriority = null
            , int responseAddressPriority = 1
            , IServiceHandlerSerialization serializer = null
            , Action<IListener> action = null
            , (int priority, UdpConfig config)[] udpExtended = null
            )
            where C : IPipelineChannelIncoming<IPipeline>
        {
            if (shIdColl == null)
                shIdColl = new ServiceHandlerCollectionContext();

            if (serializer != null)
                cpipe.Pipeline.AddPayloadSerializer(serializer);

            shIdColl.Serialization = (shIdColl.Serialization?.Id ?? serializer?.Id ?? $"udp_in/{cpipe.Channel.Id}").ToLowerInvariant();

            var configs = new[] { (requestAddressPriority ?? 1, udpDefault) };

            var listener = new UdpCommunicationAgent(configs
                , CommunicationAgentCapabilities.Listener
                , shIdColl
                , requestAddress, responseAddress
                , requestAddressPriority, responseAddressPriority
                );

            cpipe.AttachListener(listener, action, true);

            return cpipe;
        }

    }
}
