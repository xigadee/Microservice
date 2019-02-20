using System;

namespace Xigadee
{
    public static partial class CorePipelineExtensionsCore
    {
        /// <summary>
        /// Assigns a master job to the broadcast channel.
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="C"></typeparam>
        /// <param name="cpipe">The pipeline.</param>
        /// <param name="command">The command.</param>
        /// <param name="assign">The assign.</param>
        /// <param name="channelType">Type of the channel.</param>
        /// <param name="autosetPartition2">Automatically sets the priority partition for master job negotiation to 2.</param>
        /// <returns></returns>
        public static E AssignMasterJob<E, C>(this E cpipe
            , C command
            , Action<C> assign = null
            , string channelType = null
            , bool autosetPartition2 = true
            )
            where E : IPipelineChannelBroadcast<IPipeline>
            where C : ICommand
        {
            command.MasterJobNegotiationChannelMessageType = channelType ?? command.GetType().Name.ToLowerInvariant();

            if (autosetPartition2)
                command.MasterJobNegotiationChannelPriority = 2;

            if (cpipe.ChannelListener != null && command.MasterJobNegotiationChannelIdAutoSet)
                command.MasterJobNegotiationChannelIdIncoming = cpipe.ChannelListener.Id;

            if (cpipe.ChannelSender != null && command.MasterJobNegotiationChannelIdAutoSet)
                command.MasterJobNegotiationChannelIdOutgoing = cpipe.ChannelSender.Id;

            return cpipe;
        }
        /// <summary>
        /// Assigns the master job.
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="P"></typeparam>
        /// <typeparam name="C"></typeparam>
        /// <param name="cpipe">The pipeline.</param>
        /// <param name="creator">The creator.</param>
        /// <param name="assign">The assign.</param>
        /// <param name="channelType">Type of the channel.</param>
        /// <returns></returns>
        public static E AssignMasterJob<E, P, C>(this E cpipe
            , Func<IEnvironmentConfiguration, C> creator
            , Action<C> assign = null
            , string channelType = null
            )
            where E : IPipelineChannelBroadcast<IPipeline>
            where C : ICommand
        {
            var command = creator(cpipe.ToConfiguration());

            cpipe.AssignMasterJob(command, assign, channelType);

            return cpipe;
        }
    }
}
