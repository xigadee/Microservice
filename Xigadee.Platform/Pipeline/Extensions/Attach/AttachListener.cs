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
        /// <param name="cpipe"></param>
        /// <param name="listener"></param>
        /// <param name="setFromChannelProperties"></param>
        /// <returns></returns>
        public static IPipelineChannelIncoming AttachListener(this IPipelineChannelIncoming cpipe
            , IListener listener
            , bool setFromChannelProperties = true
            )
        {
            if (cpipe.Channel == null)
                throw new ArgumentNullException("The pipe channel is null.");

            if (cpipe.Channel.InternalOnly)
                throw new ChannelInternalOnlyException(cpipe.Channel.Id, cpipe.Channel.Direction);

            if (setFromChannelProperties)
            {
                listener.ChannelId = cpipe.Channel.Id;
                listener.PriorityPartitions = cpipe.Channel.Partitions.Cast<ListenerPartitionConfig>().ToList();
                listener.ResourceProfiles = cpipe.Channel.ResourceProfiles;
            }

            cpipe.Pipeline.Service.RegisterListener(listener);

            return cpipe;
        }

        public static IPipelineChannelIncoming AttachListener<S>(this IPipelineChannelIncoming cpipe
            , Func<IEnvironmentConfiguration, S> creator = null
            , Action<S> action = null
            , bool setFromChannelProperties = true
            )
            where S : IListener, new()
        {
            var listener = creator!=null?(creator(cpipe.Pipeline.Configuration)):new S();

            action?.Invoke(listener);

            cpipe.AttachListener(listener, setFromChannelProperties);

            return cpipe;
        }


    }
}
