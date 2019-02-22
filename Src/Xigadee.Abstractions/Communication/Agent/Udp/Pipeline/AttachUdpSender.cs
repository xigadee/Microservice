using System;

namespace Xigadee
{
    /// <summary>
    /// These extensions are used to attach a UDP based listener and sender to a channel
    /// </summary>
    public static partial class CorePipelineExtensionsCore
    {
        /// <summary>
        /// Attaches the Udp sender to the outgoing channel.
        /// </summary>
        /// <typeparam name="C">The pipeline type.</typeparam>
        /// <param name="cpipe">The pipeline.</param>
        /// <param name="udpDefault">The UDP endpoint configuration.</param>
        /// <param name="shIdColl">Default service handler id collection.</param>
        /// <param name="serializer">This is an optional serializer that can be added with the specific mime type. Note:  the serializer mime type will be changed, so you should not share this serializer instance.</param>
        /// <param name="action">The optional action to be called when the sender is created.</param>
        /// <param name="maxUdpMessagePayloadSize">This is the max UDP message payload size. The default is 508 bytes. If you set this to null, the sender will not check the size before transmitting.</param>
        /// <returns>Returns the pipeline.</returns>
        public static C AttachUdpSender<C>(this C cpipe
            , UdpConfig udpDefault = null
            , ServiceHandlerIdCollection shIdColl = null
            , IServiceHandlerSerialization serializer = null
            , Action<ISender> action = null
            , int? maxUdpMessagePayloadSize = UdpConfig.PacketMaxSize
            , params (int priority, UdpConfig config)[] udpExtended
            )
            where C : IPipelineChannelOutgoing<IPipeline>
        {
            shIdColl = shIdColl ?? new ServiceHandlerIdCollection();

            shIdColl.Serializer = (
                shIdColl.Serializer?.Id
                ?? serializer?.Id
                ?? $"udp_out/{cpipe.Channel.Id}"
                ).ToLowerInvariant();

            if (serializer != null)
                cpipe.Pipeline.AddPayloadSerializer(serializer);

            var sender = new UdpCommunicationAgent(udpDefault
                , CommunicationAgentCapabilities.Sender
                , shIdColl
                , maxUdpMessagePayloadSize: maxUdpMessagePayloadSize);

            cpipe.AttachSender(sender, action, true);

            return cpipe;
        }

        /// <summary>
        /// Attaches the Udp sender to the outgoing channel.
        /// </summary>
        /// <typeparam name="C">The pipeline type.</typeparam>
        /// <param name="cpipe">The pipeline.</param>
        /// <param name="udpDefault">The UDP endpoint configuration.</param>
        /// <param name="shIdColl">Default service handler id collection.</param>
        /// <param name="serialize">The serialize action.</param>
        /// <param name="canSerialize">The optional serialize check function.</param>
        /// <param name="action">The optional action to be called when the sender is created.</param>
        /// <param name="maxUdpMessagePayloadSize">This is the max UDP message payload size. The default is 508 bytes. If you set this to null, the sender will not check the size before transmitting.</param>
        /// <returns>Returns the pipeline.</returns>
        public static C AttachUdpSender<C>(this C cpipe
            , UdpConfig udpDefault = null
            , ServiceHandlerIdCollection shIdColl = null
            , Action<ServiceHandlerContext> serialize = null
            , Func<ServiceHandlerContext, bool> canSerialize = null
            , Action<ISender> action = null
            , int? maxUdpMessagePayloadSize = UdpConfig.PacketMaxSize
            , params (int priority, UdpConfig config)[] udpExtended
            )
            where C : IPipelineChannelOutgoing<IPipeline>
        {

            IServiceHandlerSerialization serializer = null;
            shIdColl = shIdColl ?? new ServiceHandlerIdCollection();

            shIdColl.Serializer = (
                shIdColl.Serializer?.Id?? $"udp_out/{cpipe.Channel.Id}"
                ).ToLowerInvariant();

            if (serialize != null)
            {
                serializer = CreateDynamicSerializer(shIdColl.Serializer.Id, serialize: serialize, canSerialize: canSerialize);
                shIdColl.Serializer = serializer.Id;
            }

            return cpipe.AttachUdpSender(
                  udpDefault
                , shIdColl
                , serializer
                , action
                , maxUdpMessagePayloadSize
                , udpExtended);
        }

    }
}
