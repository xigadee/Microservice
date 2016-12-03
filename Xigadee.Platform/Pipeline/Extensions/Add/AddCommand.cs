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
        /// 
        /// </summary>
        /// <typeparam name="C"></typeparam>
        /// <param name="pipeline"></param>
        /// <param name="command"></param>
        /// <param name="assignment"></param>
        /// <param name="startupPriority"></param>
        /// <param name="channelIncoming"></param>
        /// <param name="channelResponse"></param>
        /// <param name="channelMasterJobNegotiationIncoming"></param>
        /// <param name="channelMasterJobNegotiationOutgoing"></param>
        /// <returns></returns>
        public static MicroservicePipeline AddCommand<C>(this MicroservicePipeline pipeline
            , C command
            , Action<C> assignment = null
            , int startupPriority = 100
            , ChannelPipelineIncoming channelIncoming = null
            , ChannelPipelineOutgoing channelResponse = null
            , ChannelPipelineIncoming channelMasterJobNegotiationIncoming = null
            , ChannelPipelineOutgoing channelMasterJobNegotiationOutgoing = null
            )
            where C: ICommand
        {
            command.StartupPriority = startupPriority;

            if (channelIncoming != null && command.ChannelIdAutoSet)
                command.ChannelId = channelIncoming.Channel.Id;

            if (channelResponse != null && command.ResponseChannelIdAutoSet)
                command.ResponseChannelId = channelResponse.Channel.Id;

            if (channelMasterJobNegotiationIncoming != null && command.MasterJobNegotiationChannelIdAutoSet)
                command.MasterJobNegotiationChannelIdIncoming = channelMasterJobNegotiationIncoming.Channel.Id;

            if (channelMasterJobNegotiationOutgoing != null && command.MasterJobNegotiationChannelIdAutoSet)
                command.MasterJobNegotiationChannelIdOutgoing = channelMasterJobNegotiationOutgoing.Channel.Id;

            assignment?.Invoke(command);
            pipeline.Service.RegisterCommand(command);
            return pipeline;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="C"></typeparam>
        /// <param name="pipeline"></param>
        /// <param name="creator"></param>
        /// <param name="assignment"></param>
        /// <param name="channelIncoming"></param>
        /// <param name="channelResponse"></param>
        /// <param name="channelMasterJobNegotiationIncoming"></param>
        /// <param name="channelMasterJobNegotiationOutgoing"></param>
        /// <param name="startupPriority"></param>
        /// <returns></returns>
        public static MicroservicePipeline AddCommand<C>(this MicroservicePipeline pipeline
            , Func<IEnvironmentConfiguration, C> creator
            , Action<C> assignment = null
            , ChannelPipelineIncoming channelIncoming = null
            , ChannelPipelineOutgoing channelResponse = null
            , ChannelPipelineIncoming channelMasterJobNegotiationIncoming = null
            , ChannelPipelineOutgoing channelMasterJobNegotiationOutgoing = null
            , int startupPriority = 100
            )
            where C: ICommand
        {
            var command = creator(pipeline.Configuration);

            return pipeline.AddCommand(command, assignment, startupPriority, channelIncoming, channelResponse, channelMasterJobNegotiationIncoming);
        }

    }
}
