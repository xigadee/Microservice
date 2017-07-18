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

namespace Xigadee
{
    public static partial class CorePipelineExtensions
    {
        private static void AttachPriorityPartition<P>(IPipelineChannel pipeline, P config)
            where P : PartitionConfig
        {
            if (pipeline is IPipelineChannelBroadcast)
                throw new NotSupportedException("AttachPriorityPartition is not supported for broadcast channels.");

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

        /// <summary>
        /// This extension method can be used to attach a priority partition to a listener or sender channel using just the priority id.
        /// </summary>
        /// <typeparam name="C">The pipeline type.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="partitionIDs">The collection of ids</param>
        /// <returns>The pipeline.</returns>
        public static C AttachPriorityPartition<C>(this C pipeline, params int[] partitionIDs)
            where C : IPipelineChannel
        {
            if (pipeline is IPipelineChannelIncoming)
                partitionIDs.ForEach((p) => AttachPriorityPartition<ListenerPartitionConfig>(pipeline, p));
            else if (pipeline is IPipelineChannelOutgoing)
                partitionIDs.ForEach((p) => AttachPriorityPartition<SenderPartitionConfig>(pipeline, p));
            else
                throw new ArgumentOutOfRangeException("AttachPriorityPartition unexpected partition type.");

            return pipeline;
        }

        //--> Incoming

        /// <summary>
        /// This extension method can be used to attach a priority partition to a listening channel using a constructor function.
        /// </summary>
        /// <typeparam name="C">The pipeline type.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="creator"></param>
        /// <returns>The pipeline.</returns>
        public static C AttachPriorityPartition<C>(this C pipeline
            , Func<IEnvironmentConfiguration, Channel, ListenerPartitionConfig> creator)
            where C : IPipelineChannelIncoming<IPipeline>
        {
            var partition = creator(pipeline.Pipeline.Configuration, pipeline.Channel);
            AttachPriorityPartition<ListenerPartitionConfig>(pipeline, partition);
            return pipeline;
        }

        /// <summary>
        /// This extension method can be used to attach a priority partition to a listening channel using a collection of ListenerPartitionConfig objects.
        /// </summary>
        /// <typeparam name="C">The pipeline type.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="partitions"></param>
        /// <returns>The pipeline.</returns>
        public static C AttachPriorityPartition<C>(this C pipeline
            , params ListenerPartitionConfig[] partitions)
            where C : IPipelineChannelIncoming<IPipeline>
        {
            partitions?.ForEach((p) => AttachPriorityPartition<ListenerPartitionConfig>(pipeline, p));

            return pipeline;
        }

        //--> Outgoing

        /// <summary>
        /// This extension method can be used to attach a priority partition to a sender channel using a constructor function.
        /// </summary>
        /// <typeparam name="C">The pipeline type.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="creator">The creator function.</param>
        /// <returns>The pipeline.</returns>
        public static C AttachPriorityPartition<C>(this C pipeline
            , Func<IEnvironmentConfiguration, Channel, SenderPartitionConfig> creator)
            where C : IPipelineChannelOutgoing<IPipeline>
        {
            var partition = creator(pipeline.Pipeline.Configuration, pipeline.Channel);
            AttachPriorityPartition<SenderPartitionConfig>(pipeline, partition);
            return pipeline;
        }

        /// <summary>
        /// This extension method can be used to attach a priority partition to a listening channel using a collection of SenderPartitionConfig objects.
        /// </summary>
        /// <typeparam name="C">The pipeline type.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="partitions">The SenderPartitionConfig collection.</param>
        /// <returns>The pipeline.</returns>
        public static C AttachPriorityPartition<C>(this C pipeline
            , params SenderPartitionConfig[] partitions)
            where C : IPipelineChannelOutgoing<IPipeline>
        {
            partitions?.ForEach((p) => AttachPriorityPartition<SenderPartitionConfig>(pipeline, p));
            return pipeline;
        }
    }
}
