using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public class PersistenceMessageHandlerRedisCache<K, E>: PersistenceMessageHandlerRedisCache<K, E, PersistenceStatistics>
        where K : IEquatable<K>
    {
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="entityName">The entity name, derived from E if left null.</param>
        /// <param name="versionPolicy">The optional version and locking policy.</param>
        /// <param name="defaultTimeout">The default timeout when making requests.</param>
        /// <param name="retryPolicy">The retry policy</param>
        protected PersistenceMessageHandlerRedisCache(Func<E, K> keyMaker
            , string entityName = null
            , VersionPolicy<E> versionPolicy = null
            , TimeSpan? defaultTimeout = null
            , PersistenceRetryPolicy persistenceRetryPolicy = null
            , ResourceProfile resourceProfile = null
            , ICacheManager<K, E> cacheManager = null
            , Func<E, IEnumerable<KeyValuePair<string, string>>> referenceMaker = null

            )
            : base(keyMaker, entityName, versionPolicy, defaultTimeout, persistenceRetryPolicy: persistenceRetryPolicy, resourceProfile: resourceProfile
                  , cacheManager: cacheManager, referenceMaker: referenceMaker)
        {
        }
        #endregion
    }

    public class PersistenceMessageHandlerRedisCache<K,E,S>: PersistenceManagerHandlerJsonBase<K, E>
        where K : IEquatable<K>
        where S : PersistenceStatistics, new()
    {
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="entityName">The entity name, derived from E if left null.</param>
        /// <param name="versionPolicy">The optional version and locking policy.</param>
        /// <param name="defaultTimeout">The default timeout when making requests.</param>
        /// <param name="retryPolicy">The retry policy</param>
        protected PersistenceMessageHandlerRedisCache(Func<E, K> keyMaker
            , string entityName = null
            , VersionPolicy<E> versionPolicy = null
            , TimeSpan? defaultTimeout = null
            , PersistenceRetryPolicy persistenceRetryPolicy = null
            , ResourceProfile resourceProfile = null
            , ICacheManager<K, E> cacheManager = null
            , Func<E, IEnumerable<KeyValuePair<string, string>>> referenceMaker = null

            )
            : base(entityName, versionPolicy, defaultTimeout, persistenceRetryPolicy: persistenceRetryPolicy, resourceProfile: resourceProfile
                  , cacheManager: cacheManager, keyMaker:keyMaker, referenceMaker:referenceMaker)
        {
        }
        #endregion

    }
}
