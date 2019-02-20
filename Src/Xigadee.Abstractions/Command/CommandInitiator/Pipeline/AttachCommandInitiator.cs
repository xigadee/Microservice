using System;

namespace Xigadee
{
    public static partial class CorePipelineExtensionsCore
    {
        /// <summary>
        /// This method adds a command initiator to the Microservice incoming channel.
        /// </summary>
        /// <typeparam name="C">The pipeline type.</typeparam>
        /// <param name="cpipe">The incoming channel pipeline.</param>
        /// <param name="command">The command initiator output.</param>
        /// <param name="startupPriority">The start up priority. The default is 90.</param>
        /// <param name="defaultRequestTimespan">The default request timespan.</param>
        /// <returns>The pipeline.</returns>
        public static C AttachCommandInitiator<C>(this C cpipe
            , out CommandInitiator command
            , int startupPriority = 90
            , TimeSpan? defaultRequestTimespan = null
            )
            where C : IPipelineChannelIncoming<IPipeline>
        {
            cpipe.Pipeline.AddCommandInitiator(out command
                , startupPriority, defaultRequestTimespan, cpipe.Channel.Id, false);

            return cpipe;
        }

        /// <summary>
        /// This method adds a command initiator to the Microservice incoming channel.
        /// </summary>
        /// <typeparam name="C">The pipeline type.</typeparam>
        /// <param name="cpipe">The incoming channel pipeline.</param>
        /// <param name="command">The command initiator output.</param>
        /// <param name="startupPriority">The start up priority. The default is 90.</param>
        /// <param name="defaultRequestTimespan">The default request timespan.</param>
        /// <returns>The pipeline.</returns>
        public static C AttachICommandInitiator<C>(this C cpipe
            , out ICommandInitiator command
            , int startupPriority = 90
            , TimeSpan? defaultRequestTimespan = null
            )
            where C : IPipelineChannelIncoming<IPipeline>
        {
            cpipe.Pipeline.AddICommandInitiator(out command
                , startupPriority, defaultRequestTimespan, cpipe.Channel.Id, false);

            return cpipe;
        }
    }
}