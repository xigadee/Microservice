using System;

namespace Xigadee
{
    public static partial class CorePipelineExtensions
    {

        static string ValidateOrCreateOutgoingChannel(IPipeline pipeline, string outgoingChannelId, Guid componentId, bool create)
        {
            outgoingChannelId = string.IsNullOrEmpty(outgoingChannelId?.Trim()) ? $"CommandInitiator{componentId.ToString("N").ToUpperInvariant()}":outgoingChannelId;

            if (pipeline.ToMicroservice().Communication.HasChannel(outgoingChannelId, ChannelDirection.Incoming))
                return outgoingChannelId;

            if (!create)
                throw new ChannelDoesNotExistException(outgoingChannelId, ChannelDirection.Incoming, pipeline.ToMicroservice().Id.Name);

            var outPipe = pipeline.AddChannelIncoming(outgoingChannelId, internalOnly:true);
            
            return outgoingChannelId;
        }

        /// <summary>
        /// This method adds a command initiator to the Microservice.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="command">The command initiator output.</param>
        /// <param name="startupPriority">The start up priority. The default is 90.</param>
        /// <param name="defaultRequestTimespan">The default request timespan.</param>
        /// <param name="responseChannel">The incoming channel to attach the command initiator to.</param>
        /// <param name="createChannel">This will create the channel.</param>
        /// <returns>The pipeline.</returns>
        public static P AddCommandInitiator<P>(this P pipeline
            , out CommandInitiator command
            , int startupPriority = 90
            , TimeSpan? defaultRequestTimespan = null
            , string responseChannel = null
            , bool createChannel = true
            )
            where P:IPipeline
        {
            command = new CommandInitiator(defaultRequestTimespan);
            command.ResponseChannelId = ValidateOrCreateOutgoingChannel(pipeline, responseChannel, command.ComponentId, createChannel);
            return pipeline.AddCommand(command, startupPriority);
        }


        /// <summary>
        /// This method adds a command initiator to the Microservice.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="command">The command initiator output.</param>
        /// <param name="startupPriority">The start up priority. The default is 90.</param>
        /// <param name="defaultRequestTimespan">The default request timespan.</param>
        /// <param name="responseChannel">The incoming channel to attach the command initiator to.</param>
        /// <param name="createChannel">This property specifies that the method should create a read-only channel just for the command initiator if the responseChannel is not found.</param>
        /// <returns>The pipeline.</returns>
        public static P AddICommandInitiator<P>(this P pipeline
            , out ICommandInitiator command
            , int startupPriority = 90
            , TimeSpan? defaultRequestTimespan = null
            , string responseChannel = null
            , bool createChannel = true
            )
            where P : IPipeline
        {
            CommandInitiator interim;
            pipeline.AddCommandInitiator(out interim, startupPriority, defaultRequestTimespan, responseChannel, createChannel);
            command = interim;
            return pipeline;
        }
    }
}
