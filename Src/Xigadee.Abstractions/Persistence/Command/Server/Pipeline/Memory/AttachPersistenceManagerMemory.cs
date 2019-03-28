using System;
using System.Collections.Generic;
using System.Linq;

namespace Xigadee
{
    public static partial class CorePipelineExtensionsCore
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
        /// <param name="referenceMaker">The reference maker. This is used for entities that support read by reference.</param>
        /// <param name="propertiesMaker">The entity property maker. This is used by search.</param>
        /// <param name="keySerializer">The key serializer function.</param>
        /// <param name="prePopulate">The optional pre-population collection.</param>
        /// <param name="searches">The memory search algorithms.</param>
        /// <param name="searchIdDefault">The default search algorithm identifier.</param>
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
            , Func<E, IEnumerable<Tuple<string, string>>> referenceMaker = null
            , Func<E, IEnumerable<Tuple<string, string>>> propertiesMaker = null
            , Func<K, string> keySerializer = null
            , IEnumerable<KeyValuePair<K, E>> prePopulate = null
            , IEnumerable<RepositoryMemorySearch<K, E>> searches = null
            , string searchIdDefault = null
            )
            where C : IPipelineChannelIncoming<IPipeline>
            where K : IEquatable<K>
        {
            PersistenceServer<K, E> pm = null;

            return cpipe.AttachPersistenceManagerHandlerMemory(keyMaker, keyDeserializer
                  , out pm
                  , startupPriority
                  , entityName
                  , versionPolicy
                  , defaultTimeout
                  , persistenceRetryPolicy
                  , resourceProfile
                  , referenceMaker
                  , propertiesMaker
                  , keySerializer
                  , prePopulate
                  , searches
                  , searchIdDefault);
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
        /// <param name="referenceMaker">The reference maker. This is used for entities that support read by reference.</param>
        /// <param name="propertiesMaker">The entity property maker. This is used by search.</param>
        /// <param name="keySerializer">The key serializer function.</param>
        /// <param name="prePopulate">The optional pre-population collection.</param>
        /// <param name="searches">The memory search algorithms.</param>
        /// <param name="searchIdDefault">The default search algorithm identifier.</param>
        /// <returns>The pipeline.</returns>
        public static C AttachPersistenceManagerHandlerMemory<C, K, E>(this C cpipe
            , Func<E, K> keyMaker
            , Func<string, K> keyDeserializer
            , out PersistenceServer<K, E> pm
            , int startupPriority = 100
            , string entityName = null
            , VersionPolicy<E> versionPolicy = null
            , TimeSpan? defaultTimeout = default(TimeSpan?)
            , PersistenceRetryPolicy persistenceRetryPolicy = null
            , ResourceProfile resourceProfile = null
            , Func<E, IEnumerable<Tuple<string, string>>> referenceMaker = null
            , Func<E, IEnumerable<Tuple<string, string>>> propertiesMaker = null
            , Func<K, string> keySerializer = null
            , IEnumerable<KeyValuePair<K, E>> prePopulate = null
            , IEnumerable<RepositoryMemorySearch<K, E>> searches = null
            , string searchIdDefault = null
            )
            where C : IPipelineChannelIncoming<IPipeline>
            where K : IEquatable<K>
        {
            cpipe.Pipeline.AddPersistenceManagerMemory(keyMaker, keyDeserializer, cpipe
                  , out pm
                  , startupPriority
                  , entityName: entityName
                  , versionPolicy: versionPolicy
                  , defaultTimeout: defaultTimeout
                  , persistenceRetryPolicy: persistenceRetryPolicy
                  , resourceProfile: resourceProfile
                  , referenceMaker: referenceMaker
                  , propertiesMaker: propertiesMaker
                  , keySerializer: keySerializer
                  , prePopulate: prePopulate
                  , searches: searches
                  , searchIdDefault: searchIdDefault
                  );

            return cpipe;
        }
    }
}
