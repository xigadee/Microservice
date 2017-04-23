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
        /// <param name="cacheManager"></param>
        /// <param name="defaultRequestTimespan"></param>
        /// <param name="routing"></param>
        /// <returns>The passthrough for the channel.</returns>
        public static C AttachPersistenceMessageInitiator<C,K,E>(this C cpipe
            , out PersistenceMessageInitiator<K,E> command
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
            
            command = new PersistenceMessageInitiator<K, E>(cacheManager, defaultRequestTimespan);

            if (responseChannel == null)
            {
                var outPipe = cpipe.ToPipeline().AddChannelOutgoing($"PersistMI{command.ComponentId.ToString("N").ToUpperInvariant()}");
                command.ResponseChannelId = outPipe.Channel.Id;
            }
            else
                //If the response channel is not set, dynamically create a response channel.
                command.ResponseChannelId = responseChannel;

            command.ChannelId = cpipe.Channel.Id;
            command.RoutingDefault = routing;

            cpipe.Pipeline.AddCommand(command, startupPriority);

            return cpipe;
        }
    }
}
