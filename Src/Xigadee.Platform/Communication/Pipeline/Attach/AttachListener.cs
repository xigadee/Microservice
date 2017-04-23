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
        public static Channel ChannelResolve(this IPipelineChannel cpipe, ChannelDirection direction, bool throwIfChannelIsNull = true)
        {
            Channel channel = null;

            if (cpipe is IPipelineChannelBroadcast)
                switch (direction)
                {
                    case ChannelDirection.Incoming:
                        channel = ((IPipelineChannelBroadcast)cpipe).ChannelListener;
                        break;
                    case ChannelDirection.Outgoing:
                        channel = ((IPipelineChannelBroadcast)cpipe).ChannelSender;
                        break;
                    default:
                        throw new NotSupportedException($"ChannelDirection {direction} not supported in {nameof(CorePipelineExtensions)}/{nameof(ChannelResolve)}");
                }
            else
                channel = cpipe.Channel;

            if (channel == null)
                throw new ArgumentNullException($"The pipe channel is null -> {direction}");

            return channel;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cpipe"></param>
        /// <param name="listener"></param>
        /// <param name="setFromChannelProperties"></param>
        /// <returns></returns>
        public static C AttachListener<C>(this C cpipe
            , IListener listener
            , bool setFromChannelProperties = true
            )
            where C: IPipelineChannelIncoming<IPipeline>
        {
            Channel channel = cpipe.ChannelResolve(ChannelDirection.Incoming);

            if (channel.InternalOnly)
                throw new ChannelInternalOnlyException(channel.Id, channel.Direction);

            if (setFromChannelProperties)
            {
                listener.ChannelId = channel.Id;
                listener.PriorityPartitions = channel.Partitions.Cast<ListenerPartitionConfig>().ToList();
                listener.BoundaryLoggingActive = channel.BoundaryLoggingActive;
                listener.ResourceProfiles = channel.ResourceProfiles;
            }

            cpipe.Pipeline.Service.Communication.RegisterListener(listener);

            return cpipe;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="C"></typeparam>
        /// <typeparam name="S"></typeparam>
        /// <param name="cpipe"></param>
        /// <param name="creator"></param>
        /// <param name="action"></param>
        /// <param name="setFromChannelProperties"></param>
        /// <returns></returns>
        public static C AttachListener<C,S>(this C cpipe
            , Func<IEnvironmentConfiguration, S> creator = null
            , Action<S> action = null
            , bool setFromChannelProperties = true
            )
            where C : IPipelineChannelIncoming<IPipeline>
            where S : IListener, new()
        {
            var listener = creator!=null?(creator(cpipe.Pipeline.Configuration)):new S();

            action?.Invoke(listener);

            cpipe.AttachListener(listener, setFromChannelProperties);

            return cpipe;
        }


    }
}
