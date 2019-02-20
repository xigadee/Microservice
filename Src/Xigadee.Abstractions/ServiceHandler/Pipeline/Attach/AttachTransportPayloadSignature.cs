namespace Xigadee
{
    public static partial class CorePipelineExtensionsCore
    {
        /// <summary>
        /// This extension method can be used to assign a registered authentication handler to the channel to ensure
        /// that the message payload in signed during transmission.
        /// </summary>
        /// <typeparam name="C">The pipeline channel extension type.</typeparam>
        /// <param name="cpipe">The pipeline.</param>
        /// <param name="handler">The authentication id.</param>
        /// <returns>Returns the pipeline.</returns>
        public static C AttachTransportPayloadSignature<C>(this C cpipe, AuthenticationHandlerId handler)
            where C : IPipelineChannelOutgoing<IPipeline>
        {
            Channel channel = cpipe.ToChannel(ChannelDirection.Outgoing);

            if (!cpipe.Pipeline.Service.ServiceHandlers.Authentication.Contains(handler.Id))
                throw new AuthenticationHandlerNotResolvedException(channel.Id, handler.Id);

            if (channel.Authentication != null)
                throw new ChannelAuthenticationHandlerAlreadySetException(channel.Id);

            channel.Authentication = handler;

            return cpipe;
        }
    }
}
