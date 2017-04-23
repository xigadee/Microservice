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
        public static E AssignMasterJob<E, C>(this E cpipe
            , C command
            , Action<C> assign = null
            , string channelType = null
            )
            where E : IPipelineChannelBroadcast<IPipeline>
            where C : ICommand
        {

            command.MasterJobNegotiationChannelType = channelType ?? command.GetType().Name.ToLowerInvariant();

            if (cpipe.ChannelListener != null && command.MasterJobNegotiationChannelIdAutoSet)
                command.MasterJobNegotiationChannelIdIncoming = cpipe.ChannelListener.Id;

            if (cpipe.ChannelSender != null && command.MasterJobNegotiationChannelIdAutoSet)
                command.MasterJobNegotiationChannelIdOutgoing = cpipe.ChannelSender.Id;

            return cpipe;
        }

        public static E AssignMasterJob<E, P, C>(this E cpipe
            , Func<IEnvironmentConfiguration, C> creator
            , Action<C> assign = null
            , string channelType = null
            )
            where E : IPipelineChannelBroadcast<IPipeline>
            where C : ICommand
        {
            var command = creator(cpipe.ToConfiguration());

            cpipe.AssignMasterJob(command, assign, channelType);

            return cpipe;
        }
    }
}
