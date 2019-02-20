using System;
using System.Collections.Generic;
using System.Linq;
namespace Xigadee
{
    /// <summary>
    /// These methods can be used to create a channel.
    /// </summary>
    public static partial class CorePipelineExtensionsCore
    {
        /// <summary>
        /// Use this pipeline command to add a channel to a Microservice.
        /// </summary>
        /// <typeparam name="P"></typeparam>
        /// <param name="pipeline"></param>
        /// <param name="creatorId"></param>
        /// <param name="description"></param>
        /// <param name="partitions"></param>
        /// <param name="bLoggerStatus"></param>
        /// <param name="resourceProfiles"></param>
        /// <param name="internalOnly"></param>
        /// <param name="assign"></param>
        /// <param name="autosetPartition01"></param>
        /// <param name="isAutocreated">A boolean property that specifies whether the channel was created automatically by the communications container.</param>
        /// <returns>The original pipeline.</returns>
        public static IPipelineChannelIncoming<P> AddChannelIncoming<P>(this P pipeline
            , Func<IEnvironmentConfiguration, string> creatorId
            , string description = null
            , IEnumerable<ListenerPartitionConfig> partitions = null
            , bool? bLoggerStatus = null
            , IEnumerable<ResourceProfile> resourceProfiles = null
            , bool internalOnly = false
            , Action<IPipelineChannelIncoming<P>, Channel> assign = null
            , bool autosetPartition01 = true
            , bool isAutocreated = false
            )
            where P : IPipeline
        {
            return pipeline.AddChannelIncoming(creatorId(pipeline.Configuration)
                , description
                , partitions
                , bLoggerStatus
                , resourceProfiles
                , internalOnly
                , assign
                , autosetPartition01
                , isAutocreated);
        }
        /// <summary>
        /// Use this pipeline command to add a channel to a Microservice.
        /// </summary>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="channelId">The channel id.</param>
        /// <param name="description">A description of what the channel is used for.</param>
        /// <param name="partitions">The supported partition levels for the channel. If null, this will be set to Priority 0 and 1.</param>
        /// <param name="boundaryLoggingEnabled">Use this flag to override the default boundary logging settings for the Microservice.</param>
        /// <param name="resourceProfiles">The resource profiles that should be used in the polling logic to reduce or stop incoming messages.</param>
        /// <param name="internalOnly">Set this flag to true if you don't wish to attach any external listeners to this channel, i.e. internal only.</param>
        /// <param name="assign"></param>
        /// <param name="autosetPartition01">This method automatically sets the default priority 0 and 1 partitions for the channel.</param>
        /// <param name="isAutocreated">A boolean property that specifies whether the channel was created automatically by the communications container.</param>
        /// <returns>The original pipeline.</returns>
        public static IPipelineChannelIncoming<P> AddChannelIncoming<P>(this P pipeline
            , string channelId
            , string description = null
            , IEnumerable<ListenerPartitionConfig> partitions = null
            , bool? boundaryLoggingEnabled = null
            , IEnumerable<ResourceProfile> resourceProfiles = null
            , bool internalOnly = false
            , Action<IPipelineChannelIncoming<P>, Channel> assign = null
            , bool autosetPartition01 = true
            , bool isAutocreated = false
            )
            where P: IPipeline
        {     
            var channel = pipeline.ToMicroservice().Communication.RegisterChannel(
                new Channel(channelId, ChannelDirection.Incoming, description, boundaryLoggingEnabled, internalOnly,isAutocreated: isAutocreated));

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
