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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pipeline"></param>
        /// <param name="channelId"></param>
        /// <param name="description"></param>
        /// <param name="partitions"></param>
        /// <param name="bLogger"></param>
        /// <param name="internalOnly"></param>
        /// <param name="assign"></param>
        /// <param name="autosetPartition01"></param>
        /// <returns></returns>
        public static IPipelineChannelOutgoing AddChannelOutgoing(this IPipelineBase pipeline
            , string channelId
            , string description = null
            , IEnumerable<SenderPartitionConfig> partitions = null
            , IBoundaryLogger bLogger = null
            , bool internalOnly = false
            , Action<ChannelPipelineOutgoing, Channel> assign = null
            , bool autosetPartition01 = true
            )
        {
            var channel = pipeline.ToMicroservice().RegisterChannel(
                new Channel(channelId, ChannelDirection.Outgoing, description, bLogger, internalOnly));

            if (partitions == null && autosetPartition01)
                partitions = SenderPartitionConfig.Init(0,1);

            channel.Partitions = partitions?.ToList();

            var cpipe = new ChannelPipelineOutgoing(pipeline.ToPipeline(), channel);

            assign?.Invoke(cpipe, cpipe.Channel);

            return cpipe;
        }
    }
}
