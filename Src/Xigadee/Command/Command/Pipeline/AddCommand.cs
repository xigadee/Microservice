using System;

namespace Xigadee
{
    public static partial class CorePipelineExtensions
    {
        /// <summary>
        /// This extension method is used to add a command to the Microservice.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <typeparam name="C">The command type.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="startupPriority">The optional start up priority. The default is 100.</param>
        /// <param name="assign">The command assignment action.</param>
        /// <param name="channelIncoming">The optional request channel.</param>
        /// <param name="channelResponse">The optional response channel.</param>
        /// <returns>Returns the pipeline.</returns>
        public static P AddCommand<P,C>(this P pipeline
            , int startupPriority = 100
            , Action<C> assign = null
            , IPipelineChannelIncoming<P> channelIncoming = null
            , IPipelineChannelOutgoing<P> channelResponse = null
            )
            where P: IPipeline
            where C : ICommand, new()
        {
            return pipeline.AddCommand(new C(), startupPriority, assign, channelIncoming, channelResponse);
        }

        /// <summary>
        /// This extension method is used to add a command to the Microservice.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <typeparam name="C">The command type.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="creator">The command creator function.</param>
        /// <param name="startupPriority">The optional start up priority. The default is 100.</param>
        /// <param name="assign">The command assignment action.</param>
        /// <param name="channelIncoming">The optional request channel.</param>
        /// <param name="channelResponse">The optional response channel.</param>
        /// <returns>Returns the pipeline.</returns>
        public static P AddCommand<P,C>(this P pipeline
            , Func<IEnvironmentConfiguration, C> creator
            , int startupPriority = 100
            , Action<C> assign = null
            , IPipelineChannelIncoming<P> channelIncoming = null
            , IPipelineChannelOutgoing<P> channelResponse = null
            )
            where P : IPipeline
            where C : ICommand
        {
            var command = creator(pipeline.Configuration);

            return pipeline.AddCommand(command, startupPriority, assign, channelIncoming, channelResponse);
        }

        /// <summary>
        /// This extension method is used to add a command to the Microservice.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <typeparam name="C">The command type.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="command">The command to add.</param>
        /// <param name="startupPriority">The optional start up priority. The default is 100.</param>
        /// <param name="assign">The command assignment action.</param>
        /// <param name="channelIncoming">The optional request channel.</param>
        /// <param name="channelResponse">The optional response channel.</param>
        /// <returns>Returns the pipeline.</returns>
        public static P AddCommand<P,C>(this P pipeline
            , C command
            , int startupPriority = 100
            , Action<C> assign = null
            , IPipelineChannelIncoming<P> channelIncoming = null
            , IPipelineChannelOutgoing<P> channelResponse = null
            )
            where P : IPipeline
            where C : ICommand
        {
            command.StartupPriority = startupPriority;

            if (channelIncoming != null && command.ChannelIdAutoSet)
                command.ChannelId = channelIncoming.Channel.Id;

            if (channelResponse != null && command.ResponseChannelIdAutoSet)
                command.ResponseChannelId = channelResponse.Channel.Id;

            assign?.Invoke(command);
            pipeline.Service.Commands.Register(command);
            return pipeline;
        }
    }
}