using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public static class ChannelCommunicationSenderExtensionMethods
    {
        public static ChannelPipelineOutgoing AddSender(this ChannelPipelineOutgoing cpipe
            , ISender sender
            , bool setFromChannelProperties = true)
        {
            if (cpipe.Channel.InternalOnly)
                throw new ChannelInternalOnlyException(cpipe.Channel.Id);

            if (setFromChannelProperties && sender.ChannelId != cpipe.Channel.Id)
                throw new ChannelIdMismatchException(cpipe.Channel.Id, sender.ChannelId);

            if (setFromChannelProperties)
            {
                sender.BoundaryLogger = cpipe.Channel.BoundaryLogger;
                sender.PriorityPartitions = cpipe.Channel.Partitions.Cast<SenderPartitionConfig>().ToList();
            }

            cpipe.Pipeline.Service.RegisterSender(sender);

            return cpipe;
        }

        public static ChannelPipelineOutgoing AddSender<S>(this ChannelPipelineOutgoing cpipe
            , Func<IEnvironmentConfiguration, S> creator
            , Action<S> action = null
            , bool setFromChannelProperties = true)
            where S : ISender
        {
            var sender = creator(cpipe.Pipeline.Configuration);

            action?.Invoke(sender);

            cpipe.AddSender(sender, setFromChannelProperties);

            return cpipe;
        }
    }
}
