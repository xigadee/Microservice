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
        private static void AttachPriorityPartition<P>(IPipelineChannel pipeline, P config)
            where P : PartitionConfig
        {
            var channel = pipeline.Channel;

            if (channel.Partitions == null)
            {
                channel.Partitions = new List<P>(new[] { config });
                return;
            }

            var partitions = channel.Partitions as List<P>;

            if (partitions == null)
                throw new ChannelPartitionConfigCastException(pipeline.Channel.Id);

            if (partitions.Select((p) => p.Priority).Contains(config.Priority))
                throw new ChannelPartitionConfigExistsException(pipeline.Channel.Id, config.Priority);

            partitions.Add(config);
        }

        //Incoming
        public static C AttachPriorityPartition<C>(this C pipeline
            , ListenerPartitionConfig config)
            where C: IPipelineChannelIncoming<IPipeline>
        {
            AttachPriorityPartition<ListenerPartitionConfig>(pipeline, config);
            return pipeline;
        }

        public static C AttachPriorityPartition<C>(this C pipeline, params int[] init)
            where C : IPipelineChannel
        {
            if (pipeline is IPipelineChannelIncoming)
                ListenerPartitionConfig.Init(init).ForEach((p) => AttachPriorityPartition<ListenerPartitionConfig>(pipeline, p));
            else if (pipeline is IPipelineChannelOutgoing)
                SenderPartitionConfig.Init(init).ForEach((p) => AttachPriorityPartition<SenderPartitionConfig>(pipeline, p));
            else
                throw new ArgumentOutOfRangeException("AttachPriorityPartition unexpected partition type.");

            return pipeline;
        }


        public static C AttachPriorityPartition<C>(this C pipeline
            , Func<IEnvironmentConfiguration, Channel, ListenerPartitionConfig> creator)
            where C : IPipelineChannelIncoming<IPipeline>
        {
            var config = creator(pipeline.Pipeline.Configuration, pipeline.Channel);
            AttachPriorityPartition<ListenerPartitionConfig>(pipeline, config);
            return pipeline;
        }

        public static C AttachPriorityPartition<C>(this C pipeline
            , IEnumerable<ListenerPartitionConfig> config)
            where C : IPipelineChannelIncoming<IPipeline>
        {
            config?.ForEach((p) => pipeline.AttachPriorityPartition(p));

            return pipeline;
        }

        //Outgoing
        public static C AttachPriorityPartition<C>(this C pipeline
            , SenderPartitionConfig config)
            where C : IPipelineChannelOutgoing<IPipeline>
        {
            AttachPriorityPartition<SenderPartitionConfig>(pipeline, config);

            return pipeline;
        }

        public static C AttachPriorityPartition<C>(this C pipeline
            , Func<IEnvironmentConfiguration, Channel, SenderPartitionConfig> creator)
            where C : IPipelineChannelOutgoing<IPipeline>
        {
            var config = creator(pipeline.Pipeline.Configuration, pipeline.Channel);
            AttachPriorityPartition<SenderPartitionConfig>(pipeline, config);
            return pipeline;
        }

        public static C AttachPriorityPartition<C>(this C pipeline
            , IEnumerable<SenderPartitionConfig> config)
            where C : IPipelineChannelOutgoing<IPipeline>
        {
            config?.ForEach((p) => pipeline.AttachPriorityPartition(p));
            return pipeline;
        }
    }
}
