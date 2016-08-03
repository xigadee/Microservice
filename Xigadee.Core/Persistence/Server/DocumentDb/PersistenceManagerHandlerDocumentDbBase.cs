#region using
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
#endregion
namespace Xigadee
{
    public abstract class PersistenceManagerHandlerDocumentDbBase<K, E, S>: PersistenceManagerHandlerJsonBase<K, E, S, PersistenceCommandPolicy>
        where K : IEquatable<K>
        where S : PersistenceStatistics, new()
    {
        /// <summary>
        /// This is the connection to documentDb.
        /// </summary>
        protected readonly DocumentDbConnection mConnection;
        /// <summary>
        /// This is the default documentDb database name.
        /// </summary>
        protected readonly string mDatabaseName;
        /// <summary>
        /// This is the base documentDb collection naem
        /// </summary>
        protected readonly string mCollectionName;
        /// <summary>
        /// This sharding policy is used to create the sharded collection.
        /// </summary>
        protected ShardingPolicy<K> mShardingPolicy;

        #region Constructor
        /// <summary>
        /// This is the document db persistence agent.
        /// </summary>
        /// <param name="connection">The documentDb connection.</param>
        /// <param name="database">The is the databaseId name. If the Db does not exist it will be created.</param>
        /// <param name="keyMaker">This function creates a key of type K from an entity of type E</param>
        /// <param name="jsonMaker">This function can be used to override the default JSON creation functions.</param>
        /// <param name="databaseCollection">The is the collection name. If the collection does it exist it will be created. This will be used by the sharding policy to create multiple collections.</param>
        /// <param name="entityName">The entity name to be used in the collection. By default this will be set through reflection.</param>
        /// <param name="versionMaker">This function should be set to enforce optimistic locking.</param>
        /// <param name="defaultTimeout">This is the default timeout period to be used when connecting to documentDb.</param>
        /// <param name="shardingPolicy">This is sharding policy used to choose the appropriate collection from the key presented.</param>
        /// <param name="retryPolicy"></param>
        public PersistenceManagerHandlerDocumentDbBase(DocumentDbConnection connection
            , string database
            , Func<E, K> keyMaker
            , Func<string, K> keyDeserializer
            , string databaseCollection = null
            , ShardingPolicy<K> shardingPolicy = null
            , string entityName = null
            , VersionPolicy<E> versionPolicy = null
            , TimeSpan? defaultTimeout = null
            , PersistenceRetryPolicy persistenceRetryPolicy = null
            , ResourceProfile resourceProfile = null
            , ICacheManager<K, E> cacheManager = null
            , Func<E, IEnumerable<Tuple<string, string>>> referenceMaker = null
            , Func<K, string> keySerializer = null
            )
            : base(entityName: entityName
                  , versionPolicy: versionPolicy
                  , defaultTimeout: defaultTimeout
                  , persistenceRetryPolicy: persistenceRetryPolicy
                  , resourceProfile: resourceProfile
                  , cacheManager: cacheManager
                  , keyMaker: keyMaker
                  , referenceMaker: referenceMaker
                  , keySerializer: keySerializer
                  , keyDeserializer: keyDeserializer
                  )
        {
            mConnection = connection;
            mDatabaseName = database;
            mCollectionName = databaseCollection ?? typeof(E).Name;
            mShardingPolicy = shardingPolicy ?? new ShardingPolicy<K>(mCollectionName, (k) => 0, 1, (i) => mCollectionName);
        }
        #endregion
    }

}
