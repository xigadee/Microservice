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
        public static IPipelineChannelIncoming AttachCommand<C>(this IPipelineChannelIncoming cpipe
            , int startupPriority = 100
            , Action<C> assign = null
            , IPipelineChannelOutgoing channelResponse = null
            , IPipelineChannelIncoming channelMasterJobNegotiationIncoming = null
            , IPipelineChannelOutgoing channelMasterJobNegotiationOutgoing = null)
            where C : ICommand, new()
        {
            return cpipe.AttachCommand(new C(), startupPriority, assign, channelResponse, channelMasterJobNegotiationIncoming, channelMasterJobNegotiationOutgoing);
        }

        public static IPipelineChannelIncoming AttachCommand<C>(this IPipelineChannelIncoming cpipe
            , C command
            , int startupPriority = 100
            , Action<C> assign = null
            , IPipelineChannelOutgoing channelResponse = null
            , IPipelineChannelIncoming channelMasterJobNegotiationIncoming = null
            , IPipelineChannelOutgoing channelMasterJobNegotiationOutgoing = null
            )
            where C : ICommand
        {
            cpipe.Pipeline.AddCommand(command, startupPriority, assign, cpipe, channelResponse, channelMasterJobNegotiationIncoming, channelMasterJobNegotiationOutgoing);

            return cpipe;
        }

        public static IPipelineChannelIncoming AttachCommand<C>(this IPipelineChannelIncoming cpipe
            , Func<IEnvironmentConfiguration, C> creator
            , int startupPriority = 100
            , Action<C> assign = null
            , IPipelineChannelOutgoing channelResponse = null
            , IPipelineChannelIncoming channelMasterJobNegotiationIncoming = null
            , IPipelineChannelOutgoing channelMasterJobNegotiationOutgoing = null
            )
            where C : ICommand
        {
            cpipe.Pipeline.AddCommand(creator, startupPriority, assign, cpipe, channelResponse, channelMasterJobNegotiationIncoming, channelMasterJobNegotiationOutgoing);

            return cpipe;
        }
    }
}
