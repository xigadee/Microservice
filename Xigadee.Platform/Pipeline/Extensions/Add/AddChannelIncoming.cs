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
        public static IPipelineChannelIncoming<P> AddChannelIncoming<P>(this P pipeline
            , Func<IEnvironmentConfiguration, string> creatorId
            , string description = null
            , IEnumerable<ListenerPartitionConfig> partitions = null
            , IBoundaryLogger bLogger = null
            , IEnumerable<ResourceProfile> resourceProfiles = null
            , bool internalOnly = false
            , Action<IPipelineChannelIncoming<P>, Channel> assign = null
            , bool autosetPartition01 = true
            )
            where P : IPipeline
        {
            return pipeline.AddChannelIncoming(creatorId(pipeline.Configuration)
                , description
                , partitions
                , bLogger
                , resourceProfiles
                , internalOnly
                , assign
                , autosetPartition01);
        }
        /// <summary>
        /// use this command to add a channel to a Microservice.
        /// </summary>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="channelId">The channel id.</param>
        /// <param name="description"></param>
        /// <param name="partitions"></param>
        /// <param name="bLogger"></param>
        /// <param name="resourceProfiles"></param>
        /// <param name="internalOnly"></param>
        /// <param name="assign"></param>
        /// <param name="autosetPartition01">This method automatically sets the default priority 0 and 1 partitions for the channel.</param>
        /// <returns>The original pipeline.</returns>
        public static IPipelineChannelIncoming<P> AddChannelIncoming<P>(this P pipeline
            , string channelId
            , string description = null
            , IEnumerable<ListenerPartitionConfig> partitions = null
            , IBoundaryLogger bLogger = null
            , IEnumerable<ResourceProfile> resourceProfiles = null
            , bool internalOnly = false
            , Action<IPipelineChannelIncoming<P>, Channel> assign = null
            , bool autosetPartition01 = true
            )
            where P: IPipeline
        {     
            var channel = pipeline.ToMicroservice().RegisterChannel(
                new Channel(channelId, ChannelDirection.Incoming, description, bLogger, internalOnly));

            if (partitions == null && autosetPartition01)
                partitions = ListenerPartitionConfig.Init(0,1);

            channel.Partitions = partitions?.ToList();

            if (resourceProfiles != null)
                channel.ResourceProfiles = resourceProfiles?.ToList();

            var cpipe = new ChannelPipelineIncoming<P>(pipeline, channel);

            assign?.Invoke(cpipe, cpipe.Channel);

            return cpipe;
        }

    }
}
