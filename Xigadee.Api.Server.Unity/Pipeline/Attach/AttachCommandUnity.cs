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
        public static P AttachCommandUnity<P,I,C>(this P cpipe
            , int startupPriority = 100
            , Action<C> assign = null
            , IPipelineChannelOutgoing channelResponse = null
            , IPipelineChannelIncoming channelMasterJobNegotiationIncoming = null
            , IPipelineChannelOutgoing channelMasterJobNegotiationOutgoing = null)
            where P : IPipelineChannelIncoming
            where C : I,ICommand, new()
        {
            return cpipe.AttachCommandUnity<P,I,C>(new C(), startupPriority, assign, channelResponse, channelMasterJobNegotiationIncoming, channelMasterJobNegotiationOutgoing);
        }

        public static P AttachCommandUnity<P,I,C>(this P cpipe
            , Func<IEnvironmentConfiguration, C> creator
            , int startupPriority = 100
            , Action<C> assign = null
            , IPipelineChannelOutgoing channelResponse = null
            , IPipelineChannelIncoming channelMasterJobNegotiationIncoming = null
            , IPipelineChannelOutgoing channelMasterJobNegotiationOutgoing = null
            )
            where P : IPipelineChannelIncoming
            where C : I, ICommand
        {
            return cpipe.AttachCommandUnity<P, I, C>(creator(cpipe.Pipeline.Configuration), startupPriority, assign, channelResponse, channelMasterJobNegotiationIncoming, channelMasterJobNegotiationOutgoing);
        }

        public static P AttachCommandUnity<P,I,C>(this P cpipe
            , C command
            , int startupPriority = 100
            , Action<C> assign = null
            , IPipelineChannelOutgoing channelResponse = null
            , IPipelineChannelIncoming channelMasterJobNegotiationIncoming = null
            , IPipelineChannelOutgoing channelMasterJobNegotiationOutgoing = null
            )
            where P : IPipelineChannelIncoming
            where C : I,ICommand
        {
            var pipeline = cpipe.Pipeline as IPipelineWebApiUnity;

            if (pipeline == null)
                throw new UnityWebApiMicroservicePipelineInvalidException();

            pipeline.AddCommandUnity<IPipelineWebApiUnity, I,C>(command, startupPriority, assign, cpipe, channelResponse, channelMasterJobNegotiationIncoming, channelMasterJobNegotiationOutgoing);

            return cpipe;
        }

    }
}
