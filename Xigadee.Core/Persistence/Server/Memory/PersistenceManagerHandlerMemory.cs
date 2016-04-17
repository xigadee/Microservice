#region using
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
#endregion
namespace Xigadee
{
    #region PersistenceManagerHandlerMemory<K, E>
    /// <summary>
    /// This persistence class is used to hold entities in memory during the lifetime of the 
    /// Microservice and does not persist to any backing store.
    /// This class is used extensively by the Unit test projects. The class inherits from Json base
    /// and so employs the same logic as that used by the Azure Storage and DocumentDb persistence classes.
    /// </summary>
    /// <typeparam name="K">The key type.</typeparam>
    /// <typeparam name="E">The entity type.</typeparam>
    public class PersistenceManagerHandlerMemory<K, E> : PersistenceManagerHandlerMemory<K, E, PersistenceStatistics>
        where K : IEquatable<K>
    {
        #region Constructor

        /// <summary>
        /// This is the document db persistence agent.
        /// </summary>
        /// <param name="keyMaker">This function creates a key of type K from an entity of type E</param>
        /// <param name="keyDeserializer"></param>
        /// <param name="entityName">The entity name to be used in the collection. By default this will be set through reflection.</param>
        /// <param name="versionPolicy"></param>
        /// <param name="defaultTimeout">This is the default timeout period to be used when connecting to documentDb.</param>
        /// <param name="persistenceRetryPolicy"></param>
        /// <param name="resourceProfile"></param>
        /// <param name="cacheManager"></param>
        /// <param name="referenceMaker"></param>
        /// <param name="referenceHashMaker"></param>
        /// <param name="keySerializer"></param>
        public PersistenceManagerHandlerMemory(Func<E, K> keyMaker
            , Func<string, K> keyDeserializer
            , string entityName = null
            , VersionPolicy<E> versionPolicy = null
            , TimeSpan? defaultTimeout = null
            , PersistenceRetryPolicy persistenceRetryPolicy = null
            , ResourceProfile resourceProfile = null
            , ICacheManager<K, E> cacheManager = null
            , Func<E, IEnumerable<Tuple<string, string>>> referenceMaker = null
            , Func<Tuple<string, string>, string> referenceHashMaker = null
            , Func<K, string> keySerializer = null
            )
            : base(keyMaker, keyDeserializer
                  , entityName: entityName
                  , versionPolicy: versionPolicy
                  , defaultTimeout: defaultTimeout
                  , persistenceRetryPolicy: persistenceRetryPolicy
                  , resourceProfile: resourceProfile
                  , cacheManager: cacheManager
                  , referenceMaker: referenceMaker
                  , referenceHashMaker : referenceHashMaker
                  , keySerializer: keySerializer
                  )
        {
        }
        #endregion
    }
    #endregion

    /// <summary>
    /// This persistence class is used to hold entities in memory during the lifetime of the 
    /// Microservice and does not persist to any backing store.
    /// This class is used extensively by the Unit test projects. The class inherits from Json base
    /// and so employs the same logic as that used by the Azure Storage and DocumentDb persistence classes.
    /// </summary>
    /// <typeparam name="K">The key type.</typeparam>
    /// <typeparam name="E">The entity type.</typeparam>
    /// <typeparam name="S">An extended statistics class.</typeparam>
    public abstract class PersistenceManagerHandlerMemory<K, E, S> : PersistenceManagerHandlerJsonBase<K, E, S, PersistenceCommandPolicy>
        where K : IEquatable<K>
        where S : PersistenceStatistics, new()
    {
        #region Declarations
        /// <summary>
        /// This container holds the entities.
        /// </summary>
        protected ConcurrentDictionary<K, JsonHolder<K>> mContainer;
        /// <summary>
        /// This container holds the key references.
        /// </summary>
        protected ConcurrentDictionary<string, K> mContainerReference;
        /// <summary>
        /// This lock is used when modifying references.
        /// </summary>
        protected ReaderWriterLockSlim mReferenceModifyLock;
        #endregion
        #region Constructor
        protected PersistenceManagerHandlerMemory(Func<E, K> keyMaker
            , Func<string, K> keyDeserializer
            , string entityName = null
            , VersionPolicy<E> versionPolicy = null
            , TimeSpan? defaultTimeout = default(TimeSpan?)
            , PersistenceRetryPolicy persistenceRetryPolicy = null
            , ResourceProfile resourceProfile = null
            , ICacheManager<K, E> cacheManager = null
            , Func<E, IEnumerable<Tuple<string, string>>> referenceMaker = null
            , Func<Tuple<string, string>, string> referenceHashMaker = null
            , Func<K, string> keySerializer = null)
            : base(keyMaker, keyDeserializer, entityName, versionPolicy, defaultTimeout, persistenceRetryPolicy, resourceProfile, cacheManager, referenceMaker, referenceHashMaker, keySerializer)
        {
        }
        #endregion

        #region Start/Stop
        protected override void StartInternal()
        {
            base.StartInternal();

            mContainer = new ConcurrentDictionary<K, JsonHolder<K>>();
            mContainerReference = new ConcurrentDictionary<string, K>();
            mReferenceModifyLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
        }

        protected override void StopInternal()
        {
            mReferenceModifyLock.Dispose();
            mReferenceModifyLock = null;
            mContainerReference.Clear();
            mContainer.Clear();
            mContainerReference = null;
            mContainer = null;

            base.StopInternal();
        }
        #endregion

        /// <summary>
        /// This method gets a key for a given reference.
        /// </summary>
        /// <param name="reference">The reference tuple.</param>
        /// <param name="key">The out key.</param>
        /// <returns>Returns true if the reference is found and the key is set.</returns>
        protected virtual bool ReferenceGet(Tuple<string, string> reference, out K key)
        {
            key = default(K);

            return false;
        }

        protected virtual void ReferenceSet(K key, List<Tuple<string, string>> references)
        {

        }

        protected virtual void ReferencesRemove(K key)
        {

        }


        protected override async Task<IResponseHolder<E>> InternalCreate(K key
            , PersistenceRequestHolder<K, E> holder)
        {
            try
            {
                mReferenceModifyLock.EnterWriteLock();

                E entity = holder.rq.Entity;
                var jsonHolder = mTransform.JsonMaker(entity);

                bool success = mContainer.TryAdd(key, jsonHolder);

                if (success)
                    return new PersistenceResponseHolder<E>(PersistenceResponse.Created201, jsonHolder.Json, mTransform.PersistenceEntitySerializer.Deserializer(jsonHolder.Json));
                else
                    return new PersistenceResponseHolder<E>(PersistenceResponse.Conflict409);
            }
            finally
            {
                mReferenceModifyLock.ExitWriteLock();
            }
        }

        protected override async Task<IResponseHolder<E>> InternalRead(K key
            , PersistenceRequestHolder<K, E> holder)
        {
            try
            {
                mReferenceModifyLock.EnterReadLock();

                JsonHolder<K> jsonHolder;
                bool success = mContainer.TryGetValue(key, out jsonHolder);

                if (success)
                    return new PersistenceResponseHolder<E>(PersistenceResponse.Ok200, jsonHolder.Json, mTransform.PersistenceEntitySerializer.Deserializer(jsonHolder.Json));
                else
                    return new PersistenceResponseHolder<E>(PersistenceResponse.NotFound404);
            }
            finally
            {
                mReferenceModifyLock.ExitReadLock();
            }
        }

        protected override async Task<IResponseHolder<E>> InternalReadByRef(Tuple<string, string> reference
            , PersistenceRequestHolder<K, E> holder)
        {
            try
            {
                mReferenceModifyLock.EnterReadLock();

                K key;
                if (!ReferenceGet(reference, out key))
                    return new PersistenceResponseHolder<E>(PersistenceResponse.NotFound404);

                return await InternalRead(key, holder);
            }
            finally
            {
                mReferenceModifyLock.ExitReadLock();
            }
        }

        protected override async Task<IResponseHolder<E>> InternalUpdate(K key, PersistenceRequestHolder<K, E> holder)
        {
            try
            {
                mReferenceModifyLock.EnterWriteLock();

                E entity = holder.rq.Entity;
                var jsonHolder = mTransform.JsonMaker(entity);

                bool success = false;//mContainer.TryUpdate(key, jsonHolder,

                if (success)
                    return new PersistenceResponseHolder<E>()
                    {
                        StatusCode = 201
                        , Content = jsonHolder.Json
                        , IsSuccess = true
                        , Entity = mTransform.PersistenceEntitySerializer.Deserializer(jsonHolder.Json)
                    };
                else
                    return new PersistenceResponseHolder<E>()
                    {
                        StatusCode = 412, IsSuccess = false, IsTimeout = false
                    };
            }
            finally
            {
                mReferenceModifyLock.EnterWriteLock();
            }
        }

        protected override async Task<IResponseHolder> InternalDelete(K key, PersistenceRequestHolder<K, Tuple<K, string>> holder)
        {
            try
            {
                mReferenceModifyLock.EnterWriteLock();

                JsonHolder<K> value;
                if (!mContainer.TryRemove(key, out value))
                    return new PersistenceResponseHolder(PersistenceResponse.NotFound404);

                ReferencesRemove(key);
                return new PersistenceResponseHolder(PersistenceResponse.Ok200);
            }
            finally
            {
                mReferenceModifyLock.EnterWriteLock();
            }
        }

        protected override async Task<IResponseHolder> InternalDeleteByRef(Tuple<string, string> reference
            , PersistenceRequestHolder<K, Tuple<K, string>> holder)
        {
            try
            {
                mReferenceModifyLock.EnterWriteLock();

                K key;
                if (!ReferenceGet(reference, out key))
                    return new PersistenceResponseHolder<E>() { StatusCode = 404, IsSuccess = false, IsTimeout = false };

                return await InternalDelete(key, holder);
            }
            finally
            {
                mReferenceModifyLock.ExitWriteLock();
            }
        }

        protected override async Task<IResponseHolder> InternalVersion(K key, PersistenceRequestHolder<K, Tuple<K, string>> holder)
        {
            try
            {
                mReferenceModifyLock.EnterReadLock();

                JsonHolder<K> jsonHolder;
                bool success = mContainer.TryGetValue(key, out jsonHolder);

                if (success)
                    return new PersistenceResponseHolder(PersistenceResponse.Ok200, jsonHolder.Json);
                else
                    return new PersistenceResponseHolder(PersistenceResponse.NotFound404);
            }
            finally
            {
                mReferenceModifyLock.ExitReadLock();
            }
        }

        protected override async Task<IResponseHolder> InternalVersionByRef(Tuple<string, string> reference, PersistenceRequestHolder<K, Tuple<K, string>> holder)
        {
            try
            {
                mReferenceModifyLock.EnterReadLock();

                K key;
                if (!ReferenceGet(reference, out key))
                    return new PersistenceResponseHolder(PersistenceResponse.NotFound404);

                return await InternalVersion(key, holder);
            }
            finally
            {
                mReferenceModifyLock.ExitReadLock();
            }
        }
    }
}
