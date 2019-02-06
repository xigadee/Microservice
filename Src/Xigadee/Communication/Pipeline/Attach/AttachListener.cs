using System;
using System.Linq;

namespace Xigadee
{
    public static partial class CorePipelineExtensions
    {
        /// <summary>
        /// This extension method attaches a listener to an incoming pipeline.
        /// </summary>
        /// <typeparam name="C">The pipeline type.</typeparam>
        /// <typeparam name="L">The listener type.</typeparam>
        /// <param name="cpipe">The pipeline.</param>
        /// <param name="listener">The listener to attach.</param>
        /// <param name="action">The action that can be used for further configuration or assignment of the listener to an external variable.</param>
        /// <param name="setFromChannelProperties">The default value is true. This sets the listener properties from the channel default settings.</param>
        /// <returns>The pipeline.</returns>
        public static C AttachListener<C,L>(this C cpipe
            , L listener
            , Action<L> action = null
            , bool setFromChannelProperties = true
            )
            where C: IPipelineChannelIncoming<IPipeline>
            where L : IListener
        {
            Channel channel = cpipe.ToChannel(ChannelDirection.Incoming);

            if (channel.InternalOnly)
                throw new ChannelInternalOnlyException(channel.Id, channel.Direction);

            if (setFromChannelProperties)
            {
                if (channel.Partitions == null)
                    throw new ChannelPartitionConfigNotSetException(channel.Id);

                listener.ChannelId = channel.Id;
                listener.ListenerPriorityPartitions = channel.Partitions.Cast<ListenerPartitionConfig>().ToList();
                listener.BoundaryLoggingActive = channel.BoundaryLoggingActive;
                listener.ListenerResourceProfiles = channel.ResourceProfiles;
            }

            action?.Invoke(listener);

            cpipe.Pipeline.Service.Communication.RegisterListener(listener);

            return cpipe;
        }

        /// <summary>
        /// This extension method attaches a listener to an incoming pipeline.
        /// </summary>
        /// <typeparam name="C">The pipeline type.</typeparam>
        /// <typeparam name="L">The listener type.</typeparam>
        /// <param name="cpipe">The pipeline.</param>
        /// <param name="creator">The listener creation function.</param>
        /// <param name="action">The pre-creation action that can be used for further configuration or assignment of the listener to an external variable.</param>
        /// <param name="setFromChannelProperties">The default value is true. This sets the listener properties from the channel default settings.</param>
        /// <returns>The pipeline.</returns>
        public static C AttachListener<C,L>(this C cpipe
            , Func<IEnvironmentConfiguration, L> creator = null
            , Action<L> action = null
            , bool setFromChannelProperties = true
            )
            where C : IPipelineChannelIncoming<IPipeline>
            where L : IListener, new()
        {
            var listener = creator!=null?(creator(cpipe.Pipeline.Configuration)):new L();

            action?.Invoke(listener);

            cpipe.AttachListener(listener, setFromChannelProperties:setFromChannelProperties);

            return cpipe;
        }
    }
}
