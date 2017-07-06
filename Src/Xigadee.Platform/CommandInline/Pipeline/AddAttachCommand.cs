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
        public static P AddCommand<P>(this P pipeline
            , Func<CommandInlineContext, Task> commandFunction
            , ServiceMessageHeader header
            , string referenceId = null
            , int startupPriority = 100
            , IPipelineChannelIncoming<P> channelIncoming = null
            )
            where P : IPipeline
        {
            var command = new CommandInline(header, commandFunction, referenceId);

            pipeline.AddCommand(command, startupPriority, null, channelIncoming);

            return pipeline;
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