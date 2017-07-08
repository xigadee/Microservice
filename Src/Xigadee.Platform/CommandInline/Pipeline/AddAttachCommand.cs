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

namespace Xigadee
{
    public static partial class CorePipelineExtensions
    {
        /// <summary>
        /// This extension adds the inline command to the pipeline
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="commandFunction">The command function.</param>
        /// <param name="header">The destination fragment</param>
        /// <param name="referenceId">The optional command reference id</param>
        /// <param name="startupPriority">The command startup priority.</param>
        /// <param name="channelIncoming">The incoming channel. This is optional if you pass channel information in the header.</param>
        /// <param name="autoCreateChannel">Set this to true if you want the incoiming channel created if it does not exist. The default is true.</param>
        /// <returns>Returns the pipeline.</returns>
        public static P AddCommand<P>(this P pipeline
            , Func<CommandInlineContext, Task> commandFunction
            , ServiceMessageHeaderFragment header
            , string referenceId = null
            , int startupPriority = 100
            , IPipelineChannelIncoming<P> channelIncoming = null
            , bool autoCreateChannel = true
            )
            where P : IPipeline
        {
            ServiceMessageHeader location;

            if (header is ServiceMessageHeader)
                location = (ServiceMessageHeader)header;
            else
            {
                if (channelIncoming == null)
                    throw new ChannelIncomingMissingException();
                location = (channelIncoming.Channel.Id, header);
            }

            var command = new CommandInline(location, commandFunction, referenceId);

            pipeline.AddCommand(command, startupPriority, null, channelIncoming);

            return pipeline;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <param name="cpipe"></param>
        /// <param name="commandFunction"></param>
        /// <param name="header"></param>
        /// <param name="referenceId"></param>
        /// <param name="startupPriority"></param>
        /// <returns></returns>
        public static E AttachCommand<E>(this E cpipe
            , Func<CommandInlineContext, Task> commandFunction
            , ServiceMessageHeaderFragment header
            , string referenceId = null
            , int startupPriority = 100
            )
            where E : IPipelineChannelIncoming<IPipeline>
        {
            cpipe.ToPipeline().AddCommand(commandFunction, header, referenceId, startupPriority, cpipe);

            return cpipe;
        }


        public static E AttachCommand<E>(this E cpipe
            , Func<CommandInlineContext, Task> commandFunction
            , ServiceMessageHeader header
            , string referenceId = null
            , int startupPriority = 100
            )
            where E : IPipelineChannelIncoming<IPipeline>
        {
            cpipe.ToPipeline().AddCommand(commandFunction, header, referenceId, startupPriority, cpipe);

            return cpipe;
        }


        public static E AttachCommand<E>(this E cpipe, Type contract
            , Func<CommandInlineContext, Task> commandFunction
            , string referenceId = null
            , int startupPriority = 100
            , IPipelineChannelOutgoing<IPipeline> channelResponse = null
            )
            where E : IPipelineChannelIncoming<IPipeline>
        {
            string channelId, messageType, actionType;
            if (!ServiceMessageHelper.ExtractContractInfo(contract, out channelId, out messageType, out actionType))
                throw new InvalidMessageContractException(contract);

            if (channelId != cpipe.Channel.Id)
                throw new InvalidPipelineChannelContractException(contract, channelId, cpipe.Channel.Id);

            var command = new CommandInline((channelId, messageType, actionType), commandFunction, referenceId);

            cpipe.Pipeline.AddCommand(command, startupPriority, null, cpipe, channelResponse);

            return cpipe;
        }
    }
}