using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public static partial class CorePipelineExtensions
    {

        public static ChannelPipelineIncoming AttachListener(this ChannelPipelineIncoming cpipe
            , IListener listener
            , bool setFromChannelProperties = true
            )
        {
            if (cpipe.Channel.InternalOnly)
                throw new ChannelInternalOnlyException(cpipe.Channel.Id, cpipe.Channel.Direction);

            if (setFromChannelProperties && listener.ChannelId != cpipe.Channel.Id)
                throw new ChannelIdMismatchException(cpipe.Channel.Id, cpipe.Channel.Direction, listener.ChannelId);

            if (setFromChannelProperties)
            {
                listener.BoundaryLogger = cpipe.Channel.BoundaryLogger;
                listener.PriorityPartitions = cpipe.Channel.Partitions.Cast<ListenerPartitionConfig>().ToList();
                listener.ResourceProfiles = cpipe.Channel.ResourceProfiles;
            }

            cpipe.Pipeline.Service.RegisterListener(listener);

            return cpipe;
        }

        public static ChannelPipelineIncoming AttachListener<S>(this ChannelPipelineIncoming cpipe
            , Func<IEnvironmentConfiguration, S> creator
            , Action<S> action = null
            , bool setFromChannelProperties = true
            )
            where S : IListener
        {
            var listener = creator(cpipe.Pipeline.Configuration);

            action?.Invoke(listener);

            cpipe.AttachListener(listener, setFromChannelProperties);

            return cpipe;
        }

        public static MicroservicePipeline AttachListener(this MicroservicePipeline pipeline, IListener listener)
        {
            pipeline.Service.RegisterListener(listener);

            return pipeline;
        }

        public static MicroservicePipeline AttachListener<S>(this MicroservicePipeline pipeline, Func<IEnvironmentConfiguration, S> creator, Action<S> action = null)
            where S : IListener
        {
            var listener = creator(pipeline.Configuration);

            action?.Invoke(listener);

            pipeline.AttachListener(listener);

            return pipeline;
        }
    }
}
