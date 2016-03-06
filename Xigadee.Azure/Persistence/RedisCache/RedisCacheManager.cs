using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Xigadee
{
    public class RedisCacheManager<K, E>: CacheManagerBase<K, E>
        where K : IEquatable<K>
    {
        #region Declarations
        private string mConnection;

        private Lazy<ConnectionMultiplexer> mLazyConnection;

        protected const string cnKeyVersion = "version";
        protected const string cnKeyEntity = "entity";
        #endregion

        #region Constructor
        public RedisCacheManager(string connection, bool readOnly = true, EntityTransformHolder<K, E> transform = null):base(readOnly)
        {
            mConnection = connection;
            mTransform = transform;

            mLazyConnection = new Lazy<ConnectionMultiplexer>(() =>
             {
                 try
                 {
                     return ConnectionMultiplexer.Connect(mConnection);
                 }
                 catch (Exception ex)
                 {
                     throw;
                 }
             });
        } 
        #endregion

        #region IsActive
        /// <summary>
        /// This property specifies that the cache can be used.
        /// </summary>
        public override bool IsActive
        {
            get
            {
                return true;
            }
        }
        #endregion

        protected virtual RedisKey RedisKeyGet(EntityTransformHolder<K, E> transform, K key)
        {
            return $"entity.{transform.EntityName}.{transform.KeySerializer(key)}";
        }

        protected virtual RedisKey RedisReferenceKeyGet(EntityTransformHolder<K, E> transform, string refType)
        {
            //entityreference.{entitytype}.{keytype i.e., EMAIL, ID etc.}
            return $"entityreference.{transform.EntityName}.{refType.ToLowerInvariant()}";
        }

        #region Read(EntityTransformHolder<K, E> transform, K key)
        /// <summary>
        /// Reads the entity from the cache, if it is present.
        /// </summary>
        /// <param name="transform">The transform.</param>
        /// <param name="key">The entity key.</param>
        /// <returns>Returns the response holder.</returns>
        public override async Task<IResponseHolder<E>> Read(EntityTransformHolder<K, E> transform, K key)
        {
            if (transform == null)
                throw new ArgumentNullException("The EntityTransformHolder cannot be null.");
            try
            {
                IDatabase rDb = mLazyConnection.Value.GetDatabase();
                RedisKey hashkey = RedisKeyGet(transform, key);

                //Entity
                RedisValue result = await rDb.HashGetAsync(hashkey, cnKeyEntity);

                if (result.HasValue)
                    return new PersistenceResponseHolder<E>() { StatusCode = 200, Content = result, IsSuccess = true, Entity = transform.EntityDeserializer(result) };
                else
                    return new PersistenceResponseHolder<E>() { StatusCode = 404, IsSuccess = false };
            }
            catch (Exception ex)
            {
                return new PersistenceResponseHolder<E>() { StatusCode = 500, IsSuccess = false };
            }
        }
        #endregion
        #region Read(EntityTransformHolder<K, E> transform, Tuple<string, string> reference)
        /// <summary>
        /// Reads the entity from the cache using the entity reference.
        /// </summary>
        /// <param name="transform">The transform.</param>
        /// <param name="reference">The key value pair.</param>
        /// <returns>Returns the response holder.</returns>
        public override async Task<IResponseHolder<E>> Read(EntityTransformHolder<K, E> transform, Tuple<string, string> reference)
        {
            if (transform == null)
                throw new ArgumentNullException("The EntityTransformHolder cannot be null.");
            try
            {
                IDatabase rDb = mLazyConnection.Value.GetDatabase();
                RedisKey hashkey = RedisReferenceKeyGet(transform, reference.Item1);

                //Entity
                RedisValue result = await rDb.HashGetAsync(hashkey, reference.Item2.ToLowerInvariant());

                if (!result.HasValue)
                    return new PersistenceResponseHolder<E>() { StatusCode = 404, IsSuccess = false };

                string[] items = result.ToString().Split('|');

                K key = transform.KeyDeserializer(items[1]);

                return await Read(transform, key);
            }
            catch (Exception ex)
            {
                return new PersistenceResponseHolder<E>() { StatusCode = 500, IsSuccess = false };
            }
        }
        #endregion

        #region Delete(EntityTransformHolder<K, E> transform, K key)
        /// <summary>
        /// This method deletes from the cache.
        /// </summary>
        /// <param name="transform">The transform.</param>
        /// <param name="key">The entity key.</param>
        /// <returns>Returns the response holder.</returns>
        public override async Task<bool> Delete(EntityTransformHolder<K, E> transform, K key)
        {
            if (transform == null)
                throw new ArgumentNullException("The EntityTransformHolder cannot be null.");
            if (IsReadOnly)
                return false;

            try
            {
                IDatabase rDb = mLazyConnection.Value.GetDatabase();
                IBatch batch = rDb.CreateBatch();

                var tasks = new List<Task>();
                RedisKey hashkey = RedisKeyGet(transform, key);

                //Entity
                tasks.Add(batch.HashDeleteAsync(hashkey, cnKeyEntity));
                //Version
                tasks.Add(batch.HashDeleteAsync(hashkey, cnKeyVersion));

                batch.Execute();

                await Task.WhenAll(tasks);

                return true;
            }
            catch (Exception ex)
            {

            }

            return false;
        } 
        #endregion

        #region Write(EntityTransformHolder<K, E> transform, E entity)
        /// <summary>
        /// This method writes the entity to the redis cache.
        /// </summary>
        /// <param name="transform">The transform object.</param>
        /// <param name="entity">The entity to write.</param>
        /// <returns>Returns true if the write was successful.</returns>
        public override async Task<bool> Write(EntityTransformHolder<K, E> transform, E entity)
        {
            if (transform == null)
                throw new ArgumentNullException("The EntityTransformHolder cannot be null.");
            try
            {
                K key = transform.KeyMaker(entity);
                string version = transform.Version.EntityVersionAsString(entity);

                IDatabase rDb = mLazyConnection.Value.GetDatabase();
                IBatch batch = rDb.CreateBatch();

                var tasks = new List<Task>();
                RedisKey hashkey = RedisKeyGet(transform, key);

                //Entity
                tasks.Add(batch.HashSetAsync(hashkey, cnKeyEntity, transform.EntitySerializer(entity), when: When.Always));
                //Version
                tasks.Add(batch.HashSetAsync(hashkey, cnKeyVersion, version, when: When.Always));

                var references = transform.ReferenceMaker(entity);
                if (references != null)
                    references.ForEach((r) => tasks.Add(WriteReference(batch, transform, r, key, version)));

                batch.Execute();

                await Task.WhenAll(tasks);

                return true;
            }
            catch (Exception ex)
            {

            }

            return false;
        } 

        /// <summary>
        /// This method writes out the references for the entity.
        /// </summary>
        /// <param name="batch">The redis batch.</param>
        /// <param name="transform">The entity transform.</param>
        /// <param name="reference">The reference.</param>
        /// <param name="key">The root key.</param>
        /// <param name="version">The entity version.</param>
        /// <returns>Returns an async task.</returns>
        protected virtual Task WriteReference(IBatch batch, EntityTransformHolder<K, E> transform, Tuple<string,string> reference, K key, string version)
        {
            //entityreference.{entitytype}.{keytype i.e., EMAIL, ID etc.}
            RedisKey hashkey = RedisReferenceKeyGet(transform, reference.Item1);

            string combined = $"{version}|{transform.KeySerializer(key)}";
            //Entity
            return batch.HashSetAsync(hashkey, reference.Item2.ToLowerInvariant(), combined, when: When.Always);
        }
        #endregion

        /// <summary>
        /// This method reads the version from the cache.
        /// </summary>
        /// <param name="transform">The entity transform.</param>
        /// <param name="key">The entity key.</param>
        /// <returns>Returns the async task.</returns>
        public override async Task<IResponseHolder> VersionRead(EntityTransformHolder<K, E> transform, K key)
        {
            if (transform == null)
                throw new ArgumentNullException("The EntityTransformHolder cannot be null.");
            try
            {
                IDatabase rDb = mLazyConnection.Value.GetDatabase();
                RedisKey hashkey = RedisKeyGet(transform, key);

                //Entity
                RedisValue result = await rDb.HashGetAsync(hashkey, cnKeyVersion);

                if (result.HasValue)
                    return new PersistenceResponseHolder<E>() { StatusCode = 200, Content = result, IsSuccess = true, VersionId = result };
                else
                    return new PersistenceResponseHolder<E>() { StatusCode = 404, IsSuccess = false };
            }
            catch (Exception ex)
            {
                return new PersistenceResponseHolder<E>() { StatusCode = 500, IsSuccess = false };
            }
        }

        public override async Task<IResponseHolder> VersionRead(EntityTransformHolder<K, E> transform, Tuple<string, string> reference)
        {
            if (transform == null)
                throw new ArgumentNullException("The EntityTransformHolder cannot be null.");

            try
            {
                IDatabase rDb = mLazyConnection.Value.GetDatabase();
                RedisKey hashkey = RedisReferenceKeyGet(transform, reference.Item1);

                //Entity
                RedisValue result = await rDb.HashGetAsync(hashkey, cnKeyEntity);

                if (result.HasValue)
                    return new PersistenceResponseHolder<E>() { StatusCode = 200, Content = result, IsSuccess = true, Entity = transform.EntityDeserializer(result) };
                else
                    return new PersistenceResponseHolder<E>() { StatusCode = 404, IsSuccess = false };
            }
            catch (Exception ex)
            {
                return new PersistenceResponseHolder<E>() { StatusCode = 500, IsSuccess = false };
            }
        }

    }

    /// <summary>
    /// This is the root cache.
    /// </summary>
    public class RedisCacheHelper
    {
        /// <summary>
        /// This is the static helper.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static RedisCacheManager<K, E> Default<K, E>(string connection)
            where K : IEquatable<K>
        {
            return new RedisCacheManager<K, E>(connection);
        }
    }
}
