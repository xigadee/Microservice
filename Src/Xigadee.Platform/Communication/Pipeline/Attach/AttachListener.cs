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
using System.Linq;

namespace Xigadee
{
    public static partial class CorePipelineExtensions
    {
        /// <summary>
        /// This extension method attaches a listener to an incoming pipeline.
        /// </summary>
        /// <param name="cpipe">The pipeline.</param>
        /// <param name="listener">The listener to attach.</param>
        /// <param name="action">The action that can be used for further configuration or assignment of the listener to an external variable.</param>
        /// <param name="setFromChannelProperties">The default value is true. This sets the listener properties from the channel default settings.</param>
        /// <returns>The pipeline.</returns>
        public static C AttachListener<C,S>(this C cpipe
            , S listener
            , Action<S> action = null
            , bool setFromChannelProperties = true
            )
            where C: IPipelineChannelIncoming<IPipeline>
            where S : IListener
        {
            Channel channel = cpipe.ToChannel(ChannelDirection.Incoming);

            if (channel.InternalOnly)
                throw new ChannelInternalOnlyException(channel.Id, channel.Direction);

            if (setFromChannelProperties)
            {
                if (channel.Partitions == null)
                    throw new ChannelPartitionConfigNotSetException(channel.Id);

                listener.ChannelId = channel.Id;
                listener.PriorityPartitions = channel.Partitions.Cast<ListenerPartitionConfig>().ToList();
                listener.BoundaryLoggingActive = channel.BoundaryLoggingActive;
                listener.ResourceProfiles = channel.ResourceProfiles;
            }

            action?.Invoke(listener);

            cpipe.Pipeline.Service.Communication.RegisterListener(listener);

            return cpipe;
        }

        /// <summary>
        /// This extension method attaches a listener to an incoming pipeline.
        /// </summary>
        /// <typeparam name="C"></typeparam>
        /// <typeparam name="S"></typeparam>
        /// <param name="cpipe">The pipeline.</param>
        /// <param name="creator">The listener creation function.</param>
        /// <param name="action">The pre-creation action that can be used for further configuration or assignment of the listener to an external variable.</param>
        /// <param name="setFromChannelProperties">The default value is true. This sets the listener properties from the channel default settings.</param>
        /// <returns>The pipeline.</returns>
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

            cpipe.AttachListener(listener, setFromChannelProperties:setFromChannelProperties);

            return cpipe;
        }
    }
}
