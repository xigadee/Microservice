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
        /// This extension method attaches a memory persistence command to the incoming pipeline.
        /// </summary>
        /// <typeparam name="C">The incoming channel type.</typeparam>
        /// <typeparam name="K">The equatable key type.</typeparam>
        /// <typeparam name="E">The entity type.</typeparam>
        /// <param name="cpipe">The incoming channel pipeline.</param>
        /// <param name="keyMaker">This function creates a key of type K from an entity of type E</param>
        /// <param name="keyDeserializer">The entity key deserializer.</param>
        /// <param name="startupPriority">The command start-up priority.</param>
        /// <param name="entityName">The entity name to be used in the collection. By default this will be set through reflection.</param>
        /// <param name="versionPolicy">The version policy. This is needed if you wish to support optimistic locking for updates.</param>
        /// <param name="defaultTimeout">The default timeout. This is used for testing to simulate timeouts.</param>
        /// <param name="persistenceRetryPolicy">The retry policy. This is used for testing purposes.</param>
        /// <param name="resourceProfile">The resource profile.</param>
        /// <param name="cacheManager">The cache manager.</param>
        /// <param name="referenceMaker">The reference maker. This is used for entities that support read by reference.</param>
        /// <param name="referenceHashMaker">The reference hash maker. This is used for fast lookup.</param>
        /// <param name="keySerializer">The key serializer function.</param>
        /// <returns>The pipeline.</returns>
        public static C AttachPersistenceManagerHandlerMemory<C, K, E>(this C cpipe
            , Func<E, K> keyMaker
            , Func<string, K> keyDeserializer
            , int startupPriority = 100
            , string entityName = null
            , VersionPolicy<E> versionPolicy = null
            , TimeSpan? defaultTimeout = default(TimeSpan?)
            , PersistenceRetryPolicy persistenceRetryPolicy = null
            , ResourceProfile resourceProfile = null
            , ICacheManager<K, E> cacheManager = null
            , Func<E, IEnumerable<Tuple<string, string>>> referenceMaker = null
            , Func<Tuple<string, string>, string> referenceHashMaker = null
            , Func<K, string> keySerializer = null
            )
            where C : IPipelineChannelIncoming<IPipeline>
            where K : IEquatable<K>
        {
            PersistenceManagerHandlerMemory<K, E> pm = null;

            return cpipe.AttachPersistenceManagerHandlerMemory(keyMaker, keyDeserializer, out pm
                  , startupPriority
                  , entityName
                  , versionPolicy
                  , defaultTimeout
                  , persistenceRetryPolicy
                  , resourceProfile
                  , cacheManager
                  , referenceMaker
                  , referenceHashMaker
                  , keySerializer);
        }

        /// <summary>
        /// This extension method attaches a memory persistence command to the incoming pipeline.
        /// </summary>
        /// <typeparam name="C">The incoming channel type.</typeparam>
        /// <typeparam name="K">The equatable key type.</typeparam>
        /// <typeparam name="E">The entity type.</typeparam>
        /// <param name="cpipe">The incoming channel pipeline.</param>
        /// <param name="keyMaker">This function creates a key of type K from an entity of type E</param>
        /// <param name="keyDeserializer">The entity key deserializer.</param>
        /// <param name="pm">An output parameter for the persistence manager.</param>
        /// <param name="startupPriority">The command start-up priority.</param>
        /// <param name="entityName">The entity name to be used in the collection. By default this will be set through reflection.</param>
        /// <param name="versionPolicy">The version policy. This is needed if you wish to support optimistic locking for updates.</param>
        /// <param name="defaultTimeout">The default timeout. This is used for testing to simulate timeouts.</param>
        /// <param name="persistenceRetryPolicy">The retry policy. This is used for testing purposes.</param>
        /// <param name="resourceProfile">The resource profile.</param>
        /// <param name="cacheManager">The cache manager.</param>
        /// <param name="referenceMaker">The reference maker. This is used for entities that support read by reference.</param>
        /// <param name="referenceHashMaker">The reference hash maker. This is used for fast lookup.</param>
        /// <param name="keySerializer">The key serializer function.</param>
        /// <returns>The pipeline.</returns>
        public static C AttachPersistenceManagerHandlerMemory<C, K, E>(this C cpipe
            , Func<E, K> keyMaker
            , Func<string, K> keyDeserializer
            , out PersistenceManagerHandlerMemory<K,E> pm
            , int startupPriority = 100
            , string entityName = null
            , VersionPolicy<E> versionPolicy = null
            , TimeSpan? defaultTimeout = default(TimeSpan?)
            , PersistenceRetryPolicy persistenceRetryPolicy = null
            , ResourceProfile resourceProfile = null
            , ICacheManager<K, E> cacheManager = null
            , Func<E, IEnumerable<Tuple<string, string>>> referenceMaker = null
            , Func<Tuple<string, string>, string> referenceHashMaker = null
            , Func<K, string> keySerializer = null
            )
            where C : IPipelineChannelIncoming<IPipeline>
            where K : IEquatable<K>
        {
            cpipe.Pipeline.AddPersistenceManagerHandlerMemory(keyMaker, keyDeserializer, cpipe, out pm
                  , startupPriority
                  , entityName: entityName
                  , versionPolicy: versionPolicy
                  , defaultTimeout: defaultTimeout
                  , persistenceRetryPolicy: persistenceRetryPolicy
                  , resourceProfile: resourceProfile
                  , cacheManager: cacheManager
                  , referenceMaker: referenceMaker
                  , referenceHashMaker: referenceHashMaker
                  , keySerializer: keySerializer);

            return cpipe;
        }
    }
}
