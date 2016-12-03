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
using System.Web.Http;

namespace Xigadee
{
    public static partial class UnityWebApiExtensionMethods
    {
        public static ChannelPipelineIncoming AttachCommandUnity<I,C>(this ChannelPipelineIncoming cpipe
            , int startupPriority = 100
            , Action<C> assign = null
            , ChannelPipelineOutgoing channelResponse = null
            , ChannelPipelineIncoming channelMasterJobNegotiationIncoming = null
            , ChannelPipelineOutgoing channelMasterJobNegotiationOutgoing = null)
            where C : I,ICommand, new()
        {
            return cpipe.AttachCommandUnity<I,C>(new C(), startupPriority, assign, channelResponse, channelMasterJobNegotiationIncoming, channelMasterJobNegotiationOutgoing);
        }

        public static ChannelPipelineIncoming AttachCommandUnity<I, C>(this ChannelPipelineIncoming cpipe
            , Func<IEnvironmentConfiguration, C> creator
            , int startupPriority = 100
            , Action<C> assign = null
            , ChannelPipelineOutgoing channelResponse = null
            , ChannelPipelineIncoming channelMasterJobNegotiationIncoming = null
            , ChannelPipelineOutgoing channelMasterJobNegotiationOutgoing = null
            )
            where C : I, ICommand
        {
            return cpipe.AttachCommandUnity<I, C>(creator(cpipe.Pipeline.Configuration), startupPriority, assign, channelResponse, channelMasterJobNegotiationIncoming, channelMasterJobNegotiationOutgoing);
        }

        public static ChannelPipelineIncoming AttachCommandUnity<I,C>(this ChannelPipelineIncoming cpipe
            , C command
            , int startupPriority = 100
            , Action<C> assign = null
            , ChannelPipelineOutgoing channelResponse = null
            , ChannelPipelineIncoming channelMasterJobNegotiationIncoming = null
            , ChannelPipelineOutgoing channelMasterJobNegotiationOutgoing = null
            )
            where C : I,ICommand
        {
            var pipeline = cpipe.Pipeline as UnityWebApiMicroservicePipeline;

            if (pipeline == null)
                throw new UnityWebApiMicroservicePipelineInvalidException();

            pipeline.AddCommandUnity<I,C>(command, startupPriority, assign, cpipe, channelResponse, channelMasterJobNegotiationIncoming, channelMasterJobNegotiationOutgoing);

            return cpipe;
        }

    }
}
