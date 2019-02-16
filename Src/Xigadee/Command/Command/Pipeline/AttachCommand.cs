using System;

namespace Xigadee
{
    public static partial class CorePipelineExtensions
    {
        /// <summary>
        /// Attaches the command.
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="C"></typeparam>
        /// <param name="cpipe">The cpipe.</param>
        /// <param name="command">The command.</param>
        /// <param name="startupPriority">The startup priority.</param>
        /// <param name="assign">The assign.</param>
        /// <param name="channelResponse">The channel response.</param>
        /// <returns></returns>
        public static E AttachCommand<E,C>(this E cpipe
            , C command
            , int startupPriority = 100
            , Action<C> assign = null
            , IPipelineChannelOutgoing<IPipeline> channelResponse = null
            )
            where E: IPipelineChannelIncoming<IPipeline>
            where C : ICommand
        {
            cpipe.Pipeline.AddCommand(command, startupPriority, assign, cpipe, channelResponse);

            return cpipe;
        }
        /// <summary>
        /// Attaches the command.
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="P"></typeparam>
        /// <typeparam name="C"></typeparam>
        /// <param name="cpipe">The cpipe.</param>
        /// <param name="creator">The creator.</param>
        /// <param name="startupPriority">The startup priority.</param>
        /// <param name="assign">The assign.</param>
        /// <param name="channelResponse">The channel response.</param>
        /// <returns></returns>
        public static E AttachCommand<E, P, C>(this E cpipe
            , Func<IEnvironmentConfiguration, C> creator
            , int startupPriority = 100
            , Action<C> assign = null
            , IPipelineChannelOutgoing<IPipeline> channelResponse = null
            )
            where E : IPipelineChannelIncoming<IPipeline>
            //where P : IPipeline
            where C : ICommand
        {
            cpipe.Pipeline.AddCommand(creator, startupPriority, assign, cpipe, channelResponse);

            return cpipe;
        }
    }
}
