using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public static class ChannelCommunicationListenerExtensionMethods
    {

        public static ChannelPipelineIncoming AddListener(this ChannelPipelineIncoming cpipe
            , IListener listener
            , bool setFromChannelProperties = true
            )
        {
            if (cpipe.Channel.InternalOnly)
                throw new ChannelInternalOnlyException(cpipe.Channel.Id);

            if (setFromChannelProperties && listener.ChannelId != cpipe.Channel.Id)
                throw new ChannelIdMismatchException(cpipe.Channel.Id, listener.ChannelId);

            if (setFromChannelProperties)
            {
                listener.BoundaryLogger = cpipe.Channel.BoundaryLogger;
                listener.PriorityPartitions = cpipe.Channel.Partitions.Cast<ListenerPartitionConfig>().ToList();
                listener.ResourceProfiles = cpipe.Channel.ResourceProfiles;
            }

            cpipe.Pipeline.Service.RegisterListener(listener);

            return cpipe;
        }

        public static ChannelPipelineIncoming AddListener<S>(this ChannelPipelineIncoming cpipe
            , Func<IEnvironmentConfiguration, S> creator
            , Action<S> action = null
            , bool setFromChannelProperties = true
            )
            where S : IListener
        {
            var listener = creator(cpipe.Pipeline.Configuration);

            action?.Invoke(listener);

            cpipe.AddListener(listener, setFromChannelProperties);

            return cpipe;
        }
    }
}
