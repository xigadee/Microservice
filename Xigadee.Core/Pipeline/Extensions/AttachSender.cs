using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public static partial class CorePipelineExtensions
    {
        public static MicroservicePipeline AttachSender(this MicroservicePipeline pipeline, ISender sender)
        {
            pipeline.Service.RegisterSender(sender);

            return pipeline;
        }

        public static MicroservicePipeline AttachSender<S>(this MicroservicePipeline pipeline, Func<IEnvironmentConfiguration, S> creator, Action<S> action = null)
            where S : ISender
        {
            var sender = creator(pipeline.Configuration);

            action?.Invoke(sender);

            pipeline.AttachSender(sender);

            return pipeline;
        }

        public static ChannelPipelineOutgoing AttachSender(this ChannelPipelineOutgoing cpipe
            , ISender sender
            , bool setFromChannelProperties = true)
        {
            if (cpipe.Channel.InternalOnly)
                throw new ChannelInternalOnlyException(cpipe.Channel.Id, cpipe.Channel.Direction);

            if (setFromChannelProperties && sender.ChannelId != cpipe.Channel.Id)
                throw new ChannelIdMismatchException(cpipe.Channel.Id, cpipe.Channel.Direction, sender.ChannelId);

            if (setFromChannelProperties)
            {
                sender.BoundaryLogger = cpipe.Channel.BoundaryLogger;
                sender.PriorityPartitions = cpipe.Channel.Partitions.Cast<SenderPartitionConfig>().ToList();
            }

            cpipe.Pipeline.Service.RegisterSender(sender);

            return cpipe;
        }

        public static ChannelPipelineOutgoing AttachSender<S>(this ChannelPipelineOutgoing cpipe
            , Func<IEnvironmentConfiguration, S> creator
            , Action<S> action = null
            , bool setFromChannelProperties = true)
            where S : ISender
        {
            var sender = creator(cpipe.Pipeline.Configuration);

            action?.Invoke(sender);

            cpipe.AttachSender(sender, setFromChannelProperties);

            return cpipe;
        }
    }
}
