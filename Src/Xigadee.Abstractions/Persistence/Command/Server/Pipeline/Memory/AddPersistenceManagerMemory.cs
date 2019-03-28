using System;
using System.Collections.Generic;

namespace Xigadee
{
    public static partial class CorePipelineExtensionsCore
    {
        /// <summary>
        /// This extension method attaches a memory persistence command to the incoming pipeline.
        /// </summary>
        /// <typeparam name="P">The incoming channel type.</typeparam>
        /// <typeparam name="K">The equatable key type.</typeparam>
        /// <typeparam name="E">The entity type.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="keyMaker">This function creates a key of type K from an entity of type E</param>
        /// <param name="keyDeserializer">The entity key deserializer.</param>
        /// <param name="cpipe">The incoming channel to listen for requests.</param>
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
        public static P AddPersistenceManagerMemory<P, K, E>(this P pipeline
            , Func<E, K> keyMaker
            , Func<string, K> keyDeserializer
            , IPipelineChannelIncoming<P> cpipe
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
            , string searchIdDefault = null)
            where P : IPipeline
            where K : IEquatable<K>
        {
            PersistenceServer<K, E> pm = null;

            return pipeline.AddPersistenceManagerMemory(keyMaker, keyDeserializer, cpipe, out pm
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
                  , searchIdDefault: searchIdDefault);
        }

        /// <summary>
        /// This extension method attaches a memory persistence command to the incoming pipeline.
        /// </summary>
        /// <typeparam name="P">The incoming channel type.</typeparam>
        /// <typeparam name="K">The equatable key type.</typeparam>
        /// <typeparam name="E">The entity type.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="keyMaker">This function creates a key of type K from an entity of type E</param>
        /// <param name="keyDeserializer">The entity key deserializer.</param>
        /// <param name="cpipe">The incoming channel to listen for requests.</param>
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
        public static P AddPersistenceManagerMemory<P, K, E>(this P pipeline
            , Func<E, K> keyMaker
            , Func<string, K> keyDeserializer
            , IPipelineChannelIncoming<P> cpipe
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
            , string searchIdDefault = null)
            where P : IPipeline
            where K : IEquatable<K>
        {
            if (keyMaker == null)
                throw new ArgumentNullException("keyMaker", $"keyMaker cannot be null in {nameof(AddPersistenceManagerMemory)}");
            if (keyDeserializer == null)
                throw new ArgumentNullException("keyDeserializer", $"keyDeserializer cannot be null in {nameof(AddPersistenceManagerMemory)}");
            if (cpipe == null)
                throw new ArgumentNullException("cpipe", $"cpipe cannot be null in {nameof(AddPersistenceManagerMemory)}");

            var repo = new RepositoryMemory<K, E>(keyMaker
                , referenceMaker
                , propertiesMaker
                , versionPolicy: versionPolicy);

            searches?.ForEach(s => repo.SearchAdd(s, s.Id == searchIdDefault));

            pm = new PersistenceServer<K, E>(repo);

            pipeline.AddCommand(pm, startupPriority, channelIncoming: cpipe);

            return pipeline;
        }
    }
}
