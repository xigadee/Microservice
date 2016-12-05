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
using Microsoft.Practices.Unity;

namespace Xigadee
{
    public static partial class UnityWebApiExtensionMethods
    {
        public static IPipelineWebApiUnity AddCommandUnity<I,C>(this IPipelineWebApiUnity pipeline
            , int startupPriority = 100
            , Action<C> assign = null
            , IPipelineChannelIncoming channelIncoming = null
            , IPipelineChannelOutgoing channelResponse = null
            , IPipelineChannelIncoming channelMasterJobNegotiationIncoming = null
            , IPipelineChannelOutgoing channelMasterJobNegotiationOutgoing = null
            )
            where C : I, ICommand, new()
        {
            return pipeline.AddCommandUnity<I, C>(new C()
                , startupPriority, assign, channelIncoming, channelResponse, channelMasterJobNegotiationIncoming, channelMasterJobNegotiationOutgoing);
        }

        public static IPipelineWebApiUnity AddCommandUnity<I, C>(this IPipelineWebApiUnity pipeline
            , Func<IEnvironmentConfiguration, C> creator
            , int startupPriority = 100
            , Action<C> assign = null
            , IPipelineChannelIncoming channelIncoming = null
            , IPipelineChannelOutgoing channelResponse = null
            , IPipelineChannelIncoming channelMasterJobNegotiationIncoming = null
            , IPipelineChannelOutgoing channelMasterJobNegotiationOutgoing = null
            )
            where C : I, ICommand
        {
            return pipeline.AddCommandUnity<I, C>(creator(pipeline.Configuration)
                , startupPriority, assign, channelIncoming, channelResponse, channelMasterJobNegotiationIncoming, channelMasterJobNegotiationOutgoing);
        }

        public static IPipelineWebApiUnity AddCommandUnity<I,C>(this IPipelineWebApiUnity pipeline
            , C command
            , int startupPriority = 100
            , Action<C> assign = null
            , IPipelineChannelIncoming channelIncoming = null
            , IPipelineChannelOutgoing channelResponse = null
            , IPipelineChannelIncoming channelMasterJobNegotiationIncoming = null
            , IPipelineChannelOutgoing channelMasterJobNegotiationOutgoing = null
            )
            where C : I, ICommand
        {
            pipeline.AddCommand(command, startupPriority, assign, channelIncoming, channelResponse, channelMasterJobNegotiationIncoming, channelMasterJobNegotiationOutgoing);

            pipeline.Unity.RegisterInstance<I>(command);

            return pipeline;
        }

    }
}
