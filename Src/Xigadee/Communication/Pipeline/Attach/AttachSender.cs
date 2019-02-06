using System;
using System.Linq;

namespace Xigadee
{
    public static partial class CorePipelineExtensions
    {
        /// <summary>
        /// This method attaches a sender to the outgoing channel.
        /// </summary>
        /// <typeparam name="C">The pipeline type.</typeparam>
        /// <typeparam name="S">The sender type.</typeparam>
        /// <param name="cpipe">The pipeline.</param>
        /// <param name="sender">The sender to attach.</param>
        /// <param name="action">The action that can be used for further configuration or assignment of the sender to an external variable.</param>
        /// <param name="setFromChannelProperties">The default value is true. This sets the sender properties from the channel.</param>
        /// <returns>Returns the pipeline</returns>
        public static C AttachSender<C,S>(this C cpipe
            , S sender
            , Action<S> action = null
            , bool setFromChannelProperties = true)
            where C: IPipelineChannelOutgoing<IPipeline>
            where S: ISender
        {
            Channel channel = cpipe.ToChannel(ChannelDirection.Outgoing);

            if (channel.InternalOnly)
                throw new ChannelInternalOnlyException(channel.Id, channel.Direction);

            if (setFromChannelProperties)
            {
                if (channel.Partitions == null)
                    throw new ChannelPartitionConfigNotSetException(channel.Id);

                sender.ChannelId = channel.Id;
                sender.SenderPriorityPartitions = channel.Partitions.Cast<SenderPartitionConfig>().ToList();
                sender.BoundaryLoggingActive = channel.BoundaryLoggingActive;
            }

            action?.Invoke(sender);

            cpipe.Pipeline.Service.Communication.RegisterSender(sender);

            return cpipe;
        }

        /// <summary>
        /// This method attaches a sender to the outgoing channel.
        /// </summary>
        /// <typeparam name="C">The pipeline type.</typeparam>
        /// <typeparam name="S">The sender type.</typeparam>
        /// <param name="cpipe">The pipeline.</param>
        /// <param name="creator">The sender creator function.</param>
        /// <param name="action">The post-creation action.</param>
        /// <param name="setFromChannelProperties">The default value is true. This sets the sender properties from the channel.</param>
        /// <returns>Returns the pipeline</returns>
        public static C AttachSender<C,S>(this C cpipe
            , Func<IEnvironmentConfiguration, S> creator = null
            , Action<S> action = null
            , bool setFromChannelProperties = true)
            where S : ISender, new()
            where C : IPipelineChannelOutgoing<IPipeline>
        {
            var sender = (creator==null)?new S():creator(cpipe.Pipeline.Configuration);

            action?.Invoke(sender);

            cpipe.AttachSender(sender, setFromChannelProperties:setFromChannelProperties);

            return cpipe;
        }
    }
}
