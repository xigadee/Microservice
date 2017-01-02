#region Copyright
// Copyright Hitachi Consulting
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xigadee;

namespace Xigadee
{
    public static partial class CorePipelineExtensions
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
            cpipe.Channel.RedirectAdd(rewriteRule);

            return cpipe;
        }

        /// <summary>
        /// This method attaches a rewrite rule to the channel pipeline.
        /// </summary>
        /// <typeparam name="P">The channel pipeline type.</typeparam>
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
        /// <typeparam name="P">The channel pipeline type.</typeparam>
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
        /// <typeparam name="P">The channel pipeline type.</typeparam>
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
        /// <typeparam name="P">The channel pipeline type.</typeparam>
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
