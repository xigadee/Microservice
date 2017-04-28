#region Copyright
// Copyright Hitachi Consulting
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public static partial class CorePipelineExtensions
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
        /// <param name="startupPriority"></param>
        /// <param name="cacheManager">The cache manager to attach to the persistence agent</param>
        /// <param name="defaultRequestTimespan">The default time out time. If not set, this defaults to 30s as set in the command initiator policy.</param>
        /// <param name="routing">The route map will attempt to first route the message internally and then try an external channel.</param>
        /// <returns>The passthrough for the channel.</returns>
        public static C AttachPersistenceClient<C,K,E>(this C cpipe
            , out PersistenceClient<K,E> command
            , string responseChannel = null
            , int startupPriority = 90
            , ICacheManager<K,E> cacheManager = null
            , TimeSpan? defaultRequestTimespan = null
            , ProcessOptions routing = ProcessOptions.RouteExternal | ProcessOptions.RouteInternal
            )
            where C : IPipelineChannelIncoming<IPipeline>
            where K : IEquatable<K>
        {
            var ms = cpipe.ToMicroservice();
            
            command = new PersistenceClient<K, E>(cacheManager, defaultRequestTimespan);

            if (responseChannel == null || !cpipe.ToMicroservice().Communication.HasChannel(responseChannel, ChannelDirection.Incoming))
            {
                var outPipe = cpipe.ToPipeline().AddChannelIncoming($"PersistenceClient{command.ComponentId.ToString("N").ToUpperInvariant()}");
                command.ResponseChannelId = outPipe.Channel.Id;
            }
            else
                //If the response channel is not set, dynamically create a response channel.
                command.ResponseChannelId = responseChannel;

            //Check that the response channel exists
            if (!cpipe.ToMicroservice().Communication.HasChannel(command.ResponseChannelId, ChannelDirection.Incoming))
                throw new ChannelDoesNotExistException(command.ResponseChannelId, ChannelDirection.Incoming, ms.Id.Name);

            command.ChannelId = cpipe.Channel.Id;
            command.RoutingDefault = routing;

            cpipe.Pipeline.AddCommand(command, startupPriority);

            return cpipe;
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
        /// <returns>The passthrough for the channel.</returns>
        public static C AttachPersistenceClient<C, K, E>(this C cpipe
            , string responseChannel
            , out PersistenceClient<K, E> command
            , int startupPriority = 90
            , ICacheManager<K, E> cacheManager = null
            , TimeSpan? defaultRequestTimespan = null
            , bool verifyChannel = true
            )
            where C : IPipelineChannelOutgoing<IPipeline>
            where K : IEquatable<K>
        {
            var ms = cpipe.ToMicroservice();

            command = new PersistenceClient<K, E>(cacheManager, defaultRequestTimespan);

            if (responseChannel == null)
                throw new ArgumentNullException("responseChannel", $"{nameof(AttachPersistenceClient)}: responseChannel cannot be null.");

            //Check that the response channel exists
            if (verifyChannel && !cpipe.ToMicroservice().Communication.HasChannel(responseChannel, ChannelDirection.Incoming))
                throw new ChannelDoesNotExistException(responseChannel, ChannelDirection.Incoming, ms.Id.Name);

            //If the response channel is not set, dynamically create a response channel.
            command.ResponseChannelId = responseChannel;

            command.ChannelId = cpipe.Channel.Id;
            command.RoutingDefault = ProcessOptions.RouteExternal;

            cpipe.Pipeline.AddCommand(command, startupPriority);

            return cpipe;
        }
    }
}
