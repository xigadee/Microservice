using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public class PersistenceMessageHandlerRedisCache<K,E>: PersistenceManagerHandlerJsonBase<K,E, PersistenceStatistics>
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
        protected PersistenceMessageHandlerRedisCache(string redisConnection
            , Func<E, K> keyMaker
            , string entityName = null
            , VersionPolicy<E> versionPolicy = null
            , TimeSpan? defaultTimeout = null
            , PersistenceRetryPolicy persistenceRetryPolicy = null
            , ResourceProfile resourceProfile = null
            , Func<E, IEnumerable<Tuple<string, string>>> referenceMaker = null

            )
            : base( entityName: entityName
                  , versionPolicy: versionPolicy
                  , defaultTimeout: defaultTimeout
                  , persistenceRetryPolicy: persistenceRetryPolicy
                  , resourceProfile: resourceProfile
                  , cacheManager: RedisCacheHelper.Default<K,E>(redisConnection)
                  , keyMaker:keyMaker
                  , referenceMaker:referenceMaker)
        {
        }
        #endregion

        protected async override Task<IResponseHolder<E>> InternalCreate(PersistenceRepositoryHolder<K, E> rq, PersistenceRepositoryHolder<K, E> rs, TransmissionPayload prq, List<TransmissionPayload> prs)
        {          
            if (await mCacheManager.Write(mTransform, rq.Entity))
                return new PersistenceResponseHolder<E> { IsSuccess = true, StatusCode = 201, Entity = rq.Entity };

            return new PersistenceResponseHolder<E> { IsSuccess = false, StatusCode = 409 };
        }

        protected async override Task<IResponseHolder<E>> InternalUpdate(PersistenceRepositoryHolder<K, E> rq, PersistenceRepositoryHolder<K, E> rs, TransmissionPayload prq, List<TransmissionPayload> prs)
        {
            if (await mCacheManager.Write(mTransform, rq.Entity))
                return new PersistenceResponseHolder<E> { IsSuccess = true, StatusCode = 200, Entity = rq.Entity };

            return new PersistenceResponseHolder<E> { IsSuccess = false, StatusCode = 409 };
        }

        protected async override Task<IResponseHolder<E>> InternalRead(K key, PersistenceRepositoryHolder<K, E> rq, PersistenceRepositoryHolder<K, E> rs, TransmissionPayload prq, List<TransmissionPayload> prs)
        {
            return await mCacheManager.Read(mTransform, key);
        }

        protected async override Task<IResponseHolder<E>> InternalReadByRef(Tuple<string, string> reference, PersistenceRepositoryHolder<K, E> rq, PersistenceRepositoryHolder<K, E> rs, TransmissionPayload prq, List<TransmissionPayload> prs)
        {
            return await mCacheManager.Read(mTransform, reference);
        }

        protected async override Task<IResponseHolder> InternalVersion(K key, PersistenceRepositoryHolder<K, Tuple<K, string>> rq, PersistenceRepositoryHolder<K, Tuple<K, string>> rs, TransmissionPayload prq, List<TransmissionPayload> prs)
        {
            return await mCacheManager.VersionRead(mTransform, key);
        }

        protected async override Task<IResponseHolder> InternalDelete(K key, PersistenceRepositoryHolder<K, Tuple<K, string>> rq, PersistenceRepositoryHolder<K, Tuple<K, string>> rs, TransmissionPayload prq, List<TransmissionPayload> prs)
        {
            if (await mCacheManager.Delete(mTransform, key))
                return new PersistenceResponseHolder<E> { IsSuccess = true, StatusCode = 200 };

            return new PersistenceResponseHolder<E> { IsSuccess = false, StatusCode = 404 };
        }

    }

}
