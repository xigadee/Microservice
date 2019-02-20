using System;
using System.Threading.Tasks;

namespace Xigadee
{
    public static partial class CorePipelineExtensionsCore
    {
        /// <summary>
        /// This extension adds the in-line command to the pipeline
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="commandFunction">The command function.</param>
        /// <param name="header">The destination fragment</param>
        /// <param name="referenceId">The optional command reference id</param>
        /// <param name="startupPriority">The command start-up priority.</param>
        /// <param name="channelIncoming">The incoming channel. This is optional if you pass channel information in the header.</param>
        /// <param name="autoCreateChannel">Set this to true if you want the incoming channel created if it does not exist. The default is true.</param>
        /// <returns>Returns the pipeline.</returns>
        public static P AddCommand<P>(this P pipeline
            , Func<CommandMethodRequestContext, Task> commandFunction
            , ServiceMessageHeaderFragment header
            , string referenceId = null
            , int startupPriority = 100
            , IPipelineChannelIncoming<P> channelIncoming = null
            , bool autoCreateChannel = true
            )
            where P : IPipeline
        {
            ServiceMessageHeader location;

            if (header is ServiceMessageHeader)
                location = (ServiceMessageHeader)header;
            else
            {
                if (channelIncoming == null)
                    throw new ChannelIncomingMissingException();
                location = (channelIncoming.Channel.Id, header);
            }

            var command = new CommandMethodInline(location, commandFunction, referenceId);

            pipeline.AddCommand(command, startupPriority, null, channelIncoming);

            return pipeline;
        }

        /// <summary>
        /// This extension adds the in-line command to the pipeline
        /// </summary>
        /// <typeparam name="E">The pipeline type.</typeparam>
        /// <param name="cpipe">The pipeline.</param>
        /// <param name="commandFunction">The command function.</param>
        /// <param name="header">The header.</param>
        /// <param name="referenceId">The reference identifier.</param>
        /// <param name="startupPriority">The start-up priority.</param>
        /// <returns>Returns the pipeline.</returns>
        public static E AttachCommand<E>(this E cpipe
            , Func<CommandMethodRequestContext, Task> commandFunction
            , ServiceMessageHeaderFragment header
            , string referenceId = null
            , int startupPriority = 100
            )
            where E : IPipelineChannelIncoming<IPipeline>
        {
            cpipe.ToPipeline().AddCommand(commandFunction, header, referenceId, startupPriority, cpipe);

            return cpipe;
        }

        /// <summary>
        /// This extension adds the in-line command to the pipeline
        /// </summary>
        /// <typeparam name="E">The pipeline type.</typeparam>
        /// <param name="cpipe">The pipeline.</param>
        /// <param name="commandFunction">The command function.</param>
        /// <param name="header">The header.</param>
        /// <param name="referenceId">The reference identifier.</param>
        /// <param name="startupPriority">The start-up priority.</param>
        /// <returns>Returns the pipeline.</returns>
        public static E AttachCommand<E>(this E cpipe
            , Func<CommandMethodRequestContext, Task> commandFunction
            , ServiceMessageHeader header
            , string referenceId = null
            , int startupPriority = 100
            )
            where E : IPipelineChannelIncoming<IPipeline>
        {
            cpipe.ToPipeline().AddCommand(commandFunction, header, referenceId, startupPriority, cpipe);

            return cpipe;
        }

        /// <summary>
        /// This extension adds the in-line command to the pipeline
        /// </summary>
        /// <typeparam name="E">The pipeline type.</typeparam>
        /// <param name="cpipe">The pipeline.</param>
        /// <param name="contract">The contract.</param>
        /// <param name="commandFunction">The command function.</param>
        /// <param name="referenceId">The reference identifier.</param>
        /// <param name="startupPriority">The start-up priority.</param>
        /// <param name="channelResponse">The channel response.</param>
        /// <returns>Returns the pipeline.</returns>
        /// <exception cref="InvalidMessageContractException"></exception>
        /// <exception cref="InvalidPipelineChannelContractException"></exception>
        public static E AttachCommand<E>(this E cpipe, Type contract
            , Func<CommandMethodRequestContext, Task> commandFunction
            , string referenceId = null
            , int startupPriority = 100
            , IPipelineChannelOutgoing<IPipeline> channelResponse = null
            )
            where E : IPipelineChannelIncoming<IPipeline>
        {
            string channelId, messageType, actionType;
            if (!ServiceMessageHelper.ExtractContractInfo(contract, out channelId, out messageType, out actionType))
                throw new InvalidMessageContractException(contract);

            if (channelId != cpipe.Channel.Id)
                throw new InvalidPipelineChannelContractException(contract, channelId, cpipe.Channel.Id);

            var command = new CommandMethodInline((channelId, messageType, actionType), commandFunction, referenceId);

            cpipe.Pipeline.AddCommand(command, startupPriority, null, cpipe, channelResponse);

            return cpipe;
        }
    }
}