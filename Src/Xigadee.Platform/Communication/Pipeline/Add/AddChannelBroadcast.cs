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
    /// <summary>
    /// These methods can be used to create a channel.
    /// </summary>
    public static partial class CorePipelineExtensions
    {
        public static IPipelineChannelBroadcast<P> AddChannelBroadcast<P>(this P pipeline
            , Func<IEnvironmentConfiguration, string> creatorId
            , string description = null
            , IEnumerable<ListenerPartitionConfig> partitionsListener = null
            , IEnumerable<SenderPartitionConfig> partitionsSender = null
            , bool? bLoggerStatus = null
            , IEnumerable<ResourceProfile> resourceProfiles = null
            , bool internalOnly = false
            , Action<IPipelineChannelBroadcast<P>> assign = null
            , bool autosetPartition2 = true
            )
            where P : IPipeline
        {
            return pipeline.AddChannelBroadcast(creatorId(pipeline.Configuration)
                , description
                , partitionsListener
                , partitionsSender
                , bLoggerStatus
                , resourceProfiles
                , internalOnly
                , assign
                , autosetPartition2);
        }
        /// <summary>
        /// use this command to add a channel to a Microservice.
        /// </summary>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="channelId">The channel id.</param>
        /// <param name="description"></param>
        /// <param name="partitions"></param>
        /// <param name="boundaryLoggingEnabled"></param>
        /// <param name="resourceProfiles"></param>
        /// <param name="internalOnly"></param>
        /// <param name="assign"></param>
        /// <param name="autosetPartition01">This method automatically sets the default priority 0 and 1 partitions for the channel.</param>
        /// <returns>The original pipeline.</returns>
        public static IPipelineChannelBroadcast<P> AddChannelBroadcast<P>(this P pipeline
            , string channelId
            , string description = null
            , IEnumerable<ListenerPartitionConfig> partitionsListener = null
            , IEnumerable<SenderPartitionConfig> partitionsSender = null
            , bool? boundaryLoggingEnabled = null
            , IEnumerable<ResourceProfile> resourceProfiles = null
            , bool internalOnly = false
            , Action<IPipelineChannelBroadcast<P>> assign = null
            , bool autosetPartition2 = true
            )
            where P : IPipeline
        {
            var channelListener = pipeline.ToMicroservice().Communication.RegisterChannel(
                new Channel(channelId, ChannelDirection.Incoming, description, boundaryLoggingEnabled, internalOnly));

            var channelSender = pipeline.ToMicroservice().Communication.RegisterChannel(
                new Channel(channelId, ChannelDirection.Outgoing, description, boundaryLoggingEnabled, internalOnly));

            if (partitionsListener == null && autosetPartition2)
                partitionsListener = ListenerPartitionConfig.Init(2);

            if (partitionsSender == null && autosetPartition2)
                partitionsSender = SenderPartitionConfig.Init(2);

            channelListener.Partitions = partitionsListener?.ToList();
            channelSender.Partitions = partitionsSender?.ToList();

            if (resourceProfiles != null)
                channelListener.ResourceProfiles = resourceProfiles?.ToList();

            var cpipe = new ChannelPipelineBroadcast<P>(pipeline, channelListener, channelSender);

            assign?.Invoke(cpipe);

            return cpipe;
        }
    }
}
