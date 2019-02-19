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

namespace Xigadee
{
    public static partial class CorePipelineExtensions
    {
        /// <summary>
        /// This pipeline command creates an internal Persistence Message Initiator that does not queue requests.
        /// </summary>
        /// <typeparam name="C">The incoming channel type.</typeparam>
        /// <typeparam name="K">The key type.</typeparam>
        /// <typeparam name="E">The entity type.</typeparam>
        /// <param name="cpipe">The pipeline.</param>
        /// <param name="command">The command.</param>
        /// <param name="outgoingChannel">The outgoing channel.</param>
        /// <param name="startupPriority">The start-up priority.</param>
        /// <param name="cacheManager">The cache manager.</param>
        /// <param name="defaultRequestTimespan">The default request timespan.</param>
        /// <returns>The pass through for the channel.</returns>
        public static C AttachPersistenceInternalService<C,K,E>(this C cpipe
            , out PersistenceInternalClient<K,E> command
            , string outgoingChannel
            , int startupPriority = 90
            , ICacheManager<K,E> cacheManager = null
            , TimeSpan? defaultRequestTimespan = null
            )
            where C : IPipelineChannelIncoming<IPipeline>
            where K : IEquatable<K>
        { 
            command = new PersistenceInternalClient<K, E>(cacheManager: cacheManager, defaultRequestTimespan: defaultRequestTimespan)
            {
                  ResponseChannelId = cpipe.Channel.Id
                , ChannelId = outgoingChannel
            };

            cpipe.Pipeline.AddCommand(command, startupPriority);

            return cpipe;
        }
    }
}
