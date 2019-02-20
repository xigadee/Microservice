using System;
namespace Xigadee
{
    public static partial class CorePipelineExtensionsCore
    {
        /// <summary>
        /// This method attaches a rewrite rule to the channel pipeline.
        /// </summary>
        /// <typeparam name="C">The channel pipeline type.</typeparam>
        /// <param name="cpipe">The incoming pipeline.</param>
        /// <param name="rewriteRule">The rewrite rule.</param>
        /// <returns>Returns the original pipeline.</returns>
        public static C AttachMessageRedirectRule<C>(this C cpipe, MessageRedirectRule rewriteRule)
            where C: IPipelineChannel<IPipeline>
        {
            if (cpipe is IPipelineChannelBroadcast)
            {
                cpipe.ToChannel(ChannelDirection.Incoming).RedirectAdd(rewriteRule);
                cpipe.ToChannel(ChannelDirection.Outgoing).RedirectAdd(rewriteRule);
            }
            else
                cpipe.Channel.RedirectAdd(rewriteRule);

            return cpipe;
        }

        /// <summary>
        /// This method attaches a rewrite rule to the channel pipeline.
        /// </summary>
        /// <typeparam name="C">The channel pipeline type.</typeparam>
        /// <param name="cpipe">The incoming pipeline.</param>
        /// <param name="canRedirect">The match function.</param>
        /// <param name="redirect">The redirect action.</param>
        /// <param name="canCache">Specifies whether the redirect hit can be cached.</param>
        /// <returns>Returns the original pipeline.</returns>
        public static C AttachMessageRedirectRule<C>(this C cpipe
            , Func<TransmissionPayload, bool> canRedirect
            , Action<TransmissionPayload> redirect
            , bool canCache = true)
            where C : IPipelineChannel<IPipeline>
        {
            return cpipe.AttachMessageRedirectRule(new MessageRedirectRule(canRedirect, redirect, canCache));
        }

        /// <summary>
        /// This method attaches a rewrite rule to the channel pipeline.
        /// </summary>
        /// <typeparam name="C">The channel pipeline type.</typeparam>
        /// <param name="cpipe">The incoming pipeline.</param>
        /// <param name="matchHeader">The match header.</param>
        /// <param name="redirect">The redirect action.</param>
        /// <param name="canCache">Specifies whether the redirect hit can be cached.</param>
        /// <returns>Returns the original pipeline.</returns>
        public static C AttachMessageRedirectRule<C>(this C cpipe
            , ServiceMessageHeader matchHeader
            , Action<TransmissionPayload> redirect
            , bool canCache = true)
            where C : IPipelineChannel<IPipeline>
        {
            return cpipe.AttachMessageRedirectRule(
                new MessageRedirectRule(
                    (p) => p.Message.ToServiceMessageHeader().Equals(matchHeader)
                    , redirect
                    , canCache
                ));
        }

        /// <summary>
        /// This method attaches a rewrite rule to the channel pipeline.
        /// </summary>
        /// <typeparam name="C">The channel pipeline type.</typeparam>
        /// <param name="cpipe">The incoming pipeline.</param>
        /// <param name="matchHeader">The match header.</param>
        /// <param name="changeHeader">The redirect action.</param>
        /// <param name="canCache">Specifies whether the redirect hit can be cached.</param>
        /// <returns>Returns the original pipeline.</returns>
        public static C AttachMessageRedirectRule<C>(this C cpipe
            , ServiceMessageHeader matchHeader
            , ServiceMessageHeader changeHeader
            , bool canCache = true)
            where C : IPipelineChannel<IPipeline>
        {
            Action<TransmissionPayload> updateHeader = (p) => 
            {
                p.Message.ChannelId = changeHeader.ChannelId;
                p.Message.MessageType = changeHeader.MessageType;
                p.Message.ActionType = changeHeader.ActionType;
            };

            cpipe.AttachMessageRedirectRule(matchHeader, updateHeader, canCache);

            return cpipe;
        }


        /// <summary>
        /// This method attaches a rewrite rule to the channel pipeline.
        /// </summary>
        /// <typeparam name="C">The channel pipeline type.</typeparam>
        /// <param name="cpipe">The incoming pipeline.</param>
        /// <param name="canRedirect">The match function.</param>
        /// <param name="changeHeader">The redirect header.</param>
        /// <param name="canCache">Specifies whether the redirect hit can be cached.</param>
        /// <returns>Returns the original pipeline.</returns>
        public static C AttachMessageRedirectRule<C>(this C cpipe
            , Func<TransmissionPayload, bool> canRedirect
            , ServiceMessageHeader changeHeader
            , bool canCache = true)
            where C : IPipelineChannel<IPipeline>
        {
            Action<TransmissionPayload> updateHeader = (p) =>
            {
                p.Message.ChannelId = changeHeader.ChannelId;
                p.Message.MessageType = changeHeader.MessageType;
                p.Message.ActionType = changeHeader.ActionType;
            };

            cpipe.AttachMessageRedirectRule(canRedirect, updateHeader, canCache);

            return cpipe;
        }

    }  
}
