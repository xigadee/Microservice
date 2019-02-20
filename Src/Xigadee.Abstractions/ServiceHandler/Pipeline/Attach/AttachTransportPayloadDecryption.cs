namespace Xigadee
{
    public static partial class CorePipelineExtensionsCore
    {
        /// <summary>
        /// This extension method can be used to assign a registered encryption handler to the channel to ensure
        /// that the message payload in encrypted during transmission.
        /// </summary>
        /// <typeparam name="C">The pipeline channel extension type.</typeparam>
        /// <param name="cpipe">The pipeline.</param>
        /// <param name="handler">The encryption id.</param>
        /// <returns>Returns the pipeline.</returns>
        public static C AttachTransportPayloadDecryption<C>(this C cpipe, EncryptionHandlerId handler)
            where C : IPipelineChannelIncoming<IPipeline>
        {
            Channel channel = cpipe.ToChannel(ChannelDirection.Incoming);

            if (!cpipe.Pipeline.Service.ServiceHandlers.Encryption.Contains(handler.Id))
                throw new EncryptionHandlerNotResolvedException(channel.Id, handler.Id);

            if (channel.Encryption != null)
                throw new ChannelEncryptionHandlerAlreadySetException(cpipe.Channel.Id);

            channel.Encryption = handler;

            return cpipe;
        }
    }
}
