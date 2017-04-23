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
        public static P AddCommand<P,C>(this P pipeline
            , int startupPriority = 100
            , Action<C> assign = null
            , IPipelineChannelIncoming<P> channelIncoming = null
            , IPipelineChannelOutgoing<P> channelResponse = null
            )
            where P: IPipeline
            where C : ICommand, new()
        {
            return pipeline.AddCommand(new C(), startupPriority, assign, channelIncoming, channelResponse);
        }

        public static P AddCommand<P,C>(this P pipeline
            , Func<IEnvironmentConfiguration, C> creator
            , int startupPriority = 100
            , Action<C> assign = null
            , IPipelineChannelIncoming<P> channelIncoming = null
            , IPipelineChannelOutgoing<P> channelResponse = null
            )
            where P : IPipeline
            where C : ICommand
        {
            var command = creator(pipeline.Configuration);

            return pipeline.AddCommand(command, startupPriority, assign, channelIncoming, channelResponse);
        }

        public static P AddCommand<P,C>(this P pipeline
            , C command
            , int startupPriority = 100
            , Action<C> assign = null
            , IPipelineChannelIncoming<P> channelIncoming = null
            , IPipelineChannelOutgoing<P> channelResponse = null
            )
            where P : IPipeline
            where C : ICommand
        {
            command.StartupPriority = startupPriority;

            if (channelIncoming != null && command.ChannelIdAutoSet)
                command.ChannelId = channelIncoming.Channel.Id;

            if (channelResponse != null && command.ResponseChannelIdAutoSet)
                command.ResponseChannelId = channelResponse.Channel.Id;

            assign?.Invoke(command);
            pipeline.Service.Commands.Register(command);
            return pipeline;
        }


    }
}
