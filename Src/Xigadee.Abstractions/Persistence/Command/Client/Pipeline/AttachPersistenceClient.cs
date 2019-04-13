using System;

namespace Xigadee
{
    public static partial class CorePipelineExtensionsCore
    {
        /// <summary>
        /// This pipeline command creates a Persistence Message Initiator.
        /// </summary>
        /// <typeparam name="C">The incoming channel type.</typeparam>
        /// <typeparam name="K">The key type.</typeparam>
        /// <typeparam name="E">The entity type.</typeparam>
        /// <param name="cpipe">The incoming channel.</param>
        /// <param name="command">The output command.</param>
        /// <param name="responseChannel">The channel to send the response message on. If this is not set, a default response channel will be created.</param>
        /// <param name="startupPriority">The command start up priority. The default is 90.</param>
        /// <param name="cacheManager">The cache manager to attach to the persistence agent</param>
        /// <param name="defaultRequestTimespan">The default time out time. If not set, this defaults to 30s as set in the command initiator policy.</param>
        /// <param name="routing">The route map will attempt to first route the message internally and then try an external channel.</param>
        /// <param name="adjustPolicy">This method allows for the policy to be adjusted.</param>
        /// <returns>The pass through for the channel.</returns>
        public static C AttachPersistenceClient<C,K,E>(this C cpipe
            , out PersistenceClient<K,E> command
            , string responseChannel = null
            , int startupPriority = 90
            , ICacheManager<K,E> cacheManager = null
            , TimeSpan? defaultRequestTimespan = null
            , ProcessOptions routing = ProcessOptions.RouteExternal | ProcessOptions.RouteInternal
            , Action<PersistenceClientPolicy> adjustPolicy = null
            )
            where C : IPipelineChannelIncoming<IPipeline>
            where K : IEquatable<K>
        {        
            command = new PersistenceClient<K, E>(cacheManager, defaultRequestTimespan);

            adjustPolicy?.Invoke(command.Policy);

            bool channelInternalOnly = responseChannel == null;
            if (channelInternalOnly)
                responseChannel = $"PersistenceClient{command.ComponentId.ToString("N").ToUpperInvariant()}";

            if (!cpipe.ToMicroservice().Communication.HasChannel(responseChannel, ChannelDirection.Incoming))
            {
                var outPipe = cpipe.ToPipeline().AddChannelIncoming(responseChannel, isAutocreated:true, internalOnly: channelInternalOnly, description:$"Persistence Client Response: {typeof(E).Name}");
                command.ResponseChannelId = outPipe.Channel.Id;
            }
            else
                //If the response channel is not set, dynamically create a response channel.
                command.ResponseChannelId = responseChannel;

            //Check that the response channel exists
            if (!cpipe.ToMicroservice().Communication.HasChannel(command.ResponseChannelId, ChannelDirection.Incoming))
                throw new ChannelDoesNotExistException(command.ResponseChannelId, ChannelDirection.Incoming, cpipe.ToMicroservice().Id.Name);

            command.ChannelId = cpipe.Channel.Id;
            command.RoutingDefault = routing;

            cpipe.Pipeline.AddCommand(command, startupPriority);

            return cpipe;
        }

        /// <summary>
        /// This method sets the routing parameters for the client.
        /// </summary>
        /// <typeparam name="C"></typeparam>
        /// <param name="cpipe">The incoming channel.</param>
        /// <param name="command">The output command.</param>
        /// <param name="responseChannel">The channel to send the response message on. If this is not set, a default response channel will be created.</param>
        /// <param name="startupPriority">The command start up priority. The default is 90.</param>
        /// <param name="routing">The route map will attempt to first route the message internally and then try an external channel.</param>
        public static void SetPersistenceClientChannel<C>(this C cpipe, IPersistenceClientCommand command
            , string responseChannel = null
            , int startupPriority = 90
            , ProcessOptions routing = ProcessOptions.RouteExternal | ProcessOptions.RouteInternal
        )
            where C : IPipelineChannelIncoming<IPipeline>
        {
            bool channelInternalOnly = responseChannel == null;
            if (channelInternalOnly)
                responseChannel = $"PersistenceClient{command.ComponentId.ToString("N").ToUpperInvariant()}";

            if (!cpipe.ToMicroservice().Communication.HasChannel(responseChannel, ChannelDirection.Incoming))
            {
                var outPipe = cpipe.ToPipeline().AddChannelIncoming(responseChannel, isAutocreated: true, internalOnly: channelInternalOnly, description: $"Persistence Client Response: {command.EntityType}");
                command.ResponseChannelId = outPipe.Channel.Id;
            }
            else
                //If the response channel is not set, dynamically create a response channel.
                command.ResponseChannelId = responseChannel;

            //Check that the response channel exists
            if (!cpipe.ToMicroservice().Communication.HasChannel(command.ResponseChannelId, ChannelDirection.Incoming))
                throw new ChannelDoesNotExistException(command.ResponseChannelId, ChannelDirection.Incoming, cpipe.ToMicroservice().Id.Name);

            command.ChannelId = cpipe.Channel.Id;
            command.RoutingDefault = routing;

            cpipe.Pipeline.AddCommand(command, startupPriority);
        }

        /// <summary>
        /// This pipeline command creates a Persistence Message Initiator.
        /// </summary>
        /// <typeparam name="C">The incoming channel type.</typeparam>
        /// <typeparam name="K">The key type.</typeparam>
        /// <typeparam name="E">The entity type.</typeparam>
        /// <param name="cpipe">The incoming channel.</param>
        /// <param name="responseChannel">The channel to send the response message on. If this channel does not exist, and exception will be thrown.</param>
        /// <param name="command">The output command.</param>
        /// <param name="cacheManager">The cache manager to attach to the persistence agent</param>
        /// <param name="startupPriority"></param>
        /// <param name="defaultRequestTimespan">The default time out time. If not set, this defaults to 30s as set in the command initiator policy.</param>
        /// <param name="verifyChannel">This boolean value by default is true. It ensure the channel names passed exists. Set this to false if you do not want this check to happen.</param>
        /// <param name="adjustPolicy">This method allows for the policy to be adjusted.</param>
        /// <returns>The pass through for the channel.</returns>
        public static C AttachPersistenceClient<C, K, E>(this C cpipe
            , string responseChannel
            , out PersistenceClient<K, E> command
            , int startupPriority = 90
            , ICacheManager<K, E> cacheManager = null
            , TimeSpan? defaultRequestTimespan = null
            , bool verifyChannel = true
            , Action<PersistenceClientPolicy> adjustPolicy = null
            )
            where C : IPipelineChannelOutgoing<IPipeline>
            where K : IEquatable<K>
        {
            command = new PersistenceClient<K, E>(cacheManager, defaultRequestTimespan);

            adjustPolicy?.Invoke(command.Policy);

            if (responseChannel == null)
                throw new ArgumentNullException("responseChannel", $"{nameof(AttachPersistenceClient)}: responseChannel cannot be null.");

            //Check that the response channel exists
            if (verifyChannel && !cpipe.ToMicroservice().Communication.HasChannel(responseChannel, ChannelDirection.Incoming))
                throw new ChannelDoesNotExistException(responseChannel, ChannelDirection.Incoming, cpipe.ToMicroservice().Id.Name);

            //If the response channel is not set, dynamically create a response channel.
            command.ResponseChannelId = responseChannel;

            command.ChannelId = cpipe.Channel.Id;
            command.RoutingDefault = ProcessOptions.RouteExternal;

            cpipe.Pipeline.AddCommand(command, startupPriority);

            return cpipe;
        }
    }
}
