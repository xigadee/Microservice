using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public static class ChannelPriorityPartitionsExtensionMethods
    {
        private static void AddPriorityPartition<P>(ChannelPipelineBase pipeline, P config) where P : PartitionConfig
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
        public static ChannelPipelineIncoming AssignPriorityPartition(this ChannelPipelineIncoming pipeline, ListenerPartitionConfig config)
        {
            AddPriorityPartition<ListenerPartitionConfig>(pipeline, config);
            return pipeline;
        }

        public static ChannelPipelineIncoming AssignPriorityPartition(this ChannelPipelineIncoming pipeline, Func<IEnvironmentConfiguration, Channel, ListenerPartitionConfig> creator)
        {
            var config = creator(pipeline.Pipeline.Configuration, pipeline.Channel);
            AddPriorityPartition<ListenerPartitionConfig>(pipeline, config);
            return pipeline;
        }

        public static ChannelPipelineIncoming AssignPriorityPartition(this ChannelPipelineIncoming pipeline, IEnumerable<ListenerPartitionConfig> config)
        {
            config?.ForEach((p) => pipeline.AssignPriorityPartition(p));
            return pipeline;
        }

        //Outgoing
        public static ChannelPipelineOutgoing AssignPriorityPartition(this ChannelPipelineOutgoing pipeline, SenderPartitionConfig config)
        {
            AddPriorityPartition<SenderPartitionConfig>(pipeline, config);
            return pipeline;
        }

        public static ChannelPipelineOutgoing AssignPriorityPartition(this ChannelPipelineOutgoing pipeline, Func<IEnvironmentConfiguration, Channel, SenderPartitionConfig> creator)
        {
            var config = creator(pipeline.Pipeline.Configuration, pipeline.Channel);
            AddPriorityPartition<SenderPartitionConfig>(pipeline, config);
            return pipeline;
        }

        public static ChannelPipelineOutgoing AssignPriorityPartition(this ChannelPipelineOutgoing pipeline, IEnumerable<SenderPartitionConfig> config)
        {
            config?.ForEach((p) => pipeline.AssignPriorityPartition(p));
            return pipeline;
        }
    }
}
