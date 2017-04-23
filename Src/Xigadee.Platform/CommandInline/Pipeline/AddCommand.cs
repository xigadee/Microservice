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
            , Func<TransmissionPayload, List<TransmissionPayload>, IPayloadSerializationContainer, Task> commandFunction
            , MessageFilterWrapper message
            , string referenceId = null
            , int startupPriority = 100
            , IPipelineChannelIncoming<P> channelIncoming = null
            , IPipelineChannelOutgoing<P> channelResponse = null
            )
            where P : IPipeline
        {
            var command = new CommandInline(message, commandFunction, referenceId);

            pipeline.AddCommand(command, startupPriority, null, channelIncoming, channelResponse);

            return pipeline;
        }

        public static P AddCommand<P>(this P pipeline
            , Func<TransmissionPayload, List<TransmissionPayload>, IPayloadSerializationContainer, Task> commandFunction
            , ServiceMessageHeader header
            , string referenceId = null
            , int startupPriority = 100
            , IPipelineChannelIncoming<P> channelIncoming = null
            , IPipelineChannelOutgoing<P> channelResponse = null
            )
            where P : IPipeline
        {
            var command = new CommandInline(header, commandFunction, referenceId);

            pipeline.AddCommand(command, startupPriority, null, channelIncoming, channelResponse);

            return pipeline;
        }

        public static P AddCommand<P>(this P pipeline
            , Func<TransmissionPayload, List<TransmissionPayload>, IPayloadSerializationContainer, Task> commandFunction
            , string channelId
            , string messageType = null
            , string actionType = null
            , string referenceId = null
            , int startupPriority = 100
            , IPipelineChannelIncoming<P> channelIncoming = null
            , IPipelineChannelOutgoing<P> channelResponse = null
            )
            where P : IPipeline
        {
            var command = new CommandInline(commandFunction, channelId, messageType, actionType, referenceId);

            pipeline.AddCommand(command, startupPriority, null, channelIncoming, channelResponse);

            return pipeline;
        }
    }
}