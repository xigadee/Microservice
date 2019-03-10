using System;
using System.Collections.Generic;
using System.Diagnostics;
namespace Xigadee
{
    /// <summary>
    /// This persistence class is used to hold entities in memory during the lifetime of the 
    /// Microservice and does not persist to any backing store.
    /// This class is used extensively by the Unit test projects. The class inherits from Json base
    /// and so employs the same logic as that used by the Azure Storage and DocumentDb persistence classes.
    /// </summary>
    /// <typeparam name="K">The key type.</typeparam>
    /// <typeparam name="E">The entity type.</typeparam>
    [DebuggerDisplay("{Container.Debug}")]
    public class PersistenceManagerHandlerMemory<K, E> : PersistenceManagerHandlerEntityContainerBase<K, E, PersistenceStatistics, PersistenceManagerHandlerMemory<K, E>.CommandPolicy, EntityContainerMemory<K,E>>
        where K : IEquatable<K>
    {
        #region Constructor
        /// <summary>
        /// This is the default constructor for the memory persistence manager. 
        /// This persistence manager is used to hold an in-memory JSON representation of the entity.
        /// It is primarily used for test purposes, but can be used in a production context.
        /// Please note that all data will be lost when the service is restarted.
        /// </summary>
        /// <param name="keyMaker">This function creates a key of type K from an entity of type E</param>
        /// <param name="keyDeserializer">The entity key deserializer.</param>
        /// <param name="entityName">The entity name to be used in the collection. By default this will be set through reflection.</param>
        /// <param name="versionPolicy">The version policy. This is needed if you wish to support optimistic locking for updates.</param>
        /// <param name="defaultTimeout">The default timeout. This is used for testing to simulate timeouts.</param>
        /// <param name="persistenceRetryPolicy">The retry policy. This is used for testing purposes.</param>
        /// <param name="resourceProfile">The resource profile.</param>
        /// <param name="cacheManager">The cache manager.</param>
        /// <param name="referenceMaker">The reference maker. This is used for entities that support read by reference.</param>
        /// <param name="referenceHashMaker">The reference hash maker. This is used for fast lookup.</param>
        /// <param name="keySerializer">The key serializer function.</param>
        /// <param name="policy">The policy.</param>
        /// <param name="prePopulate">The optional pre-population collection.</param>
        public PersistenceManagerHandlerMemory(Func<E, K> keyMaker
            , Func<string, K> keyDeserializer
            , string entityName = null
            , VersionPolicy<E> versionPolicy = null
            , TimeSpan? defaultTimeout = default(TimeSpan?)
            , PersistenceRetryPolicy persistenceRetryPolicy = null
            , ResourceProfile resourceProfile = null
            , ICacheManager<K, E> cacheManager = null
            , Func<E, IEnumerable<Tuple<string, string>>> referenceMaker = null
            , Func<Tuple<string, string>, string> referenceHashMaker = null
            , Func<K, string> keySerializer = null
            , CommandPolicy policy = null
            , IEnumerable<KeyValuePair<K, E>> prePopulate = null
            )
            : base(keyMaker
                  , keyDeserializer
                  , entityName
                  , versionPolicy
                  , defaultTimeout
                  , persistenceRetryPolicy
                  , resourceProfile
                  , cacheManager
                  , referenceMaker
                  , referenceHashMaker
                  , keySerializer
                  , policy
                  , prePopulate
                  )
        {
        }
        #endregion

        #region Class -> CommandPolicy
        /// <summary>
        /// This policy class is used to configure the test conditions that can be set up for this persistence agent.
        /// </summary>
        /// <seealso cref="Xigadee.PersistenceCommandPolicy" />
        public class CommandPolicy: PersistenceCommandPolicy
        {
        } 
        #endregion
    }
}
