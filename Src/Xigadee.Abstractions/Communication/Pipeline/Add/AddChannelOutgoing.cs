using System;
using System.Collections.Generic;
using System.Linq;
namespace Xigadee
{
    /// <summary>
    /// These methods can be used to create a channel.
    /// </summary>
    public static partial class CorePipelineExtensions
    {
        public static IPipelineChannelOutgoing<P> AddChannelOutgoing<P>(this P pipeline
            , Func<IEnvironmentConfiguration, string> creatorId
            , string description = null
            , IEnumerable<SenderPartitionConfig> partitions = null
            , bool? bLoggerStatus = null
            , bool internalOnly = false
            , Action<IPipelineChannelOutgoing<P>, Channel> assign = null
            , bool autosetPartition01 = true
            )
            where P : IPipeline
        {
            return pipeline.AddChannelOutgoing(creatorId(pipeline.Configuration)
                , description
                , partitions
                , bLoggerStatus
                , internalOnly
                , assign
                , autosetPartition01);
        }

        public static IPipelineChannelOutgoing<P> AddChannelOutgoing<P>(this P pipeline
            , string channelId
            , string description = null
            , IEnumerable<SenderPartitionConfig> partitions = null
            , bool? boundaryLoggingEnabled = null
            , bool internalOnly = false
            , Action<IPipelineChannelOutgoing<P>, Channel> assign = null
            , bool autosetPartition01 = true
            )
            where P: IPipeline
        {
            var channel = pipeline.ToMicroservice().Communication.RegisterChannel(
                new Channel(channelId, ChannelDirection.Outgoing, description, boundaryLoggingEnabled, internalOnly));

            if (partitions == null && autosetPartition01)
                partitions = SenderPartitionConfig.Init(0,1);

            channel.Partitions = partitions?.ToList();

            var cpipe = new ChannelPipelineOutgoing<P>(pipeline, channel);

            assign?.Invoke(cpipe, cpipe.Channel);

            return cpipe;
        }
    }
}
