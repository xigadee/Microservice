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
        protected PersistenceMessageHandlerRedisCache(string redisConnection, Func<E, K> keyMaker
            , string entityName = null
            , VersionPolicy<E> versionPolicy = null
            , TimeSpan? defaultTimeout = null
            , PersistenceRetryPolicy persistenceRetryPolicy = null
            , ResourceProfile resourceProfile = null
            , ICacheManager<K, E> cacheManager = null
            , Func<E, IEnumerable<Tuple<string, string>>> referenceMaker = null

            )
            : base(redisConnection, keyMaker, entityName, versionPolicy, defaultTimeout, persistenceRetryPolicy: persistenceRetryPolicy, resourceProfile: resourceProfile
                  , referenceMaker: referenceMaker)
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
        protected PersistenceMessageHandlerRedisCache(string redisConnection, Func<E, K> keyMaker
            , string entityName = null
            , VersionPolicy<E> versionPolicy = null
            , TimeSpan? defaultTimeout = null
            , PersistenceRetryPolicy persistenceRetryPolicy = null
            , ResourceProfile resourceProfile = null
            , Func<E, IEnumerable<Tuple<string, string>>> referenceMaker = null

            )
            : base(entityName, versionPolicy, defaultTimeout, persistenceRetryPolicy: persistenceRetryPolicy, resourceProfile: resourceProfile
                  , cacheManager: RedisCacheManager.Default<K,E>(redisConnection), keyMaker:keyMaker, referenceMaker:referenceMaker)
        {
        }
        #endregion

        protected async override Task<IResponseHolder<E>> InternalCreate(K key, PersistenceRepositoryHolder<K, E> rq, PersistenceRepositoryHolder<K, E> rs, TransmissionPayload prq, List<TransmissionPayload> prs)
        {
            
            return new PersistenceResponseHolder<E>() { StatusCode = 501, IsSuccess = false };
        }

        protected async override Task<IResponseHolder<E>> InternalUpdate(K key, PersistenceRepositoryHolder<K, E> rq, PersistenceRepositoryHolder<K, E> rs, TransmissionPayload prq, List<TransmissionPayload> prs)
        {
            return new PersistenceResponseHolder<E>() { StatusCode = 501, IsSuccess = false };
        }

        protected async override Task<IResponseHolder<E>> InternalRead(K key, PersistenceRepositoryHolder<K, E> rq, PersistenceRepositoryHolder<K, E> rs, TransmissionPayload prq, List<TransmissionPayload> prs)
        {
            return new PersistenceResponseHolder<E>() { StatusCode = 404, IsSuccess = false };
        }


        protected async override Task<IResponseHolder> InternalVersion(K key, PersistenceRepositoryHolder<K, Tuple<K, string>> rq, PersistenceRepositoryHolder<K, Tuple<K, string>> rs, TransmissionPayload prq, List<TransmissionPayload> prs)
        {
            return new PersistenceResponseHolder() { StatusCode = 404, IsSuccess = false};
        }

        protected async override Task<IResponseHolder> InternalDelete(K key, PersistenceRepositoryHolder<K, Tuple<K, string>> rq, PersistenceRepositoryHolder<K, Tuple<K, string>> rs, TransmissionPayload prq, List<TransmissionPayload> prs)
        {
            return new PersistenceResponseHolder() { StatusCode = 501, IsSuccess = false };
        }

    }
}
