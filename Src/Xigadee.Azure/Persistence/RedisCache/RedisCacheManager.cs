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
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Xigadee
{
    /// <summary>
    /// This is the RedisCache manager that can be inserted in to a standard persistence handler.
    /// </summary>
    /// <typeparam name="K">The key type.</typeparam>
    /// <typeparam name="E">The entity type.</typeparam>
    public class RedisCacheManager<K, E>: CacheManagerBase<K, E> where K : IEquatable<K>
    {
        #region Declarations
        private readonly string mConnection;

        private readonly Lazy<ConnectionMultiplexer> mLazyConnection;

        protected const string cnKeyVersion = "version";
        protected const string cnKeyEntity = "entity";
        protected const string cnKeyEntityId = "entityId";

        protected TimeSpan mEntityTtl;
        #endregion

        #region Constructor
        public RedisCacheManager(string connection, bool readOnly = true, EntityTransformHolder<K, E> transform = null, TimeSpan? entityTtl = null)
            :base(readOnly)
        {
            mConnection = connection;
            mTransform = transform ?? new EntityTransformHolder<K, E>(true);

            mEntityTtl = entityTtl??TimeSpan.FromDays(2);
            mLazyConnection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(mConnection));
        } 
        #endregion

        #region IsActive
        /// <summary>
        /// This property specifies that the cache can be used.
        /// </summary>
        public override bool IsActive => true;

        #endregion

        #region RedisKeyGet(EntityTransformHolder<K, E> transform, K key)
        /// <summary>
        /// This method creates the hash table key to hold the entity information in Redis cache.
        /// </summary>
        /// <param name="transform">The transform object.</param>
        /// <param name="key">The entity key.</param>
        /// <returns>Returns the redis key.</returns>
        protected virtual RedisKey RedisKeyGet(EntityTransformHolder<K, E> transform, K key)
        {
            return $"entity.{transform.EntityName}.{transform.KeySerializer(key)}";
        }
        #endregion
        #region RedisReferenceKeyGet(EntityTransformHolder<K, E> transform, string refType)

        /// <summary>
        /// This method creates the reference collection key for an entity reference.
        /// </summary>
        /// <param name="transform">The transform object.</param>
        /// <param name="referenceHash">The hash of the reference i.e. email.brian@hotmail.com / custoemerid.123456</param>
        /// <returns>Returns the appropriate redis key.</returns>
        protected virtual RedisKey RedisReferenceKeyGet(EntityTransformHolder<K, E> transform, string referenceHash)
        {
            //entityreference.{entitytype}.{keytype i.e., EMAIL, ID etc.}.{brian@hotmail.com}
            return $"entityreference.{transform.EntityName}.{referenceHash}";
        }
        #endregion
        #region RedisResolveReference(EntityTransformHolder<K, E> transform, Tuple<string, string> reference)
        /// <summary>
        /// This private method is used to resolve a lookup reference from the cache.
        /// </summary>
        /// <param name="transform">The transform.</param>
        /// <param name="reference">The tuple type/value pair.</param>
        /// <returns>Returns triple with the first boolean property indicating success followed by the key and the version.</returns>
        private async Task<Tuple<bool, K, string>> RedisResolveReference(EntityTransformHolder<K, E> transform, Tuple<string, string> reference)
        {
            if (transform.KeyDeserializer != null)
                try
                {
                    IDatabase rDb = mLazyConnection.Value.GetDatabase();
                    RedisKey hashkey = RedisReferenceKeyGet(transform, transform.ReferenceHashMaker(reference));

                    //Entity
                    RedisValue enitityId = await rDb.HashGetAsync(hashkey, cnKeyEntityId);
                    if (enitityId.HasValue)
                    {
                        K key = transform.KeyDeserializer(enitityId);
                        return new Tuple<bool, K, string>(true, key, await rDb.HashGetAsync(hashkey, cnKeyVersion));
                    }
                }
                catch (Exception)
                {
                    // Don't raise an exception here
                }

            return new Tuple<bool, K, string>(false, default(K), null);
        }
        #endregion

        #region Write(EntityTransformHolder<K, E> transform, E entity, TimeSpan? expiry = null)

        /// <summary>
        /// This method writes the entity to the redis cache.
        /// </summary>
        /// <param name="transform">The transform holder.</param>
        /// <param name="entity">The entity to write.</param>
        /// <param name="expiry"></param>
        /// <returns>Returns true if the write was successful.</returns>
        public override async Task<bool> Write(EntityTransformHolder<K, E> transform, E entity, TimeSpan? expiry = null)
        {
            if (transform == null)
                throw new ArgumentNullException(nameof(transform), "The EntityTransformHolder cannot be null.");

            try
            {
                K key = transform.KeyMaker(entity);
                string version = transform.Version.EntityVersionAsString(entity);

                IDatabase rDb = mLazyConnection.Value.GetDatabase();
                IBatch batch = rDb.CreateBatch();
                RedisKey hashkey = RedisKeyGet(transform, key);

                var tasks = new List<Task>
                {
                    //Entity
                    batch.HashSetAsync(hashkey, cnKeyEntity, transform.CacheEntitySerializer.Serializer(entity), when: When.Always),
                    //Version
                    batch.HashSetAsync(hashkey, cnKeyVersion, version, when: When.Always),
                    // Expiry
                    batch.KeyExpireAsync(hashkey, expiry ?? mEntityTtl)
                };

                //Get any associated references for the entity.
                var references = transform.ReferenceMaker(entity);
                references?.ForEach(r => tasks.AddRange(WriteReference(batch, transform, r, key, version)));

                batch.Execute();

                await Task.WhenAll(tasks);

                return true;
            }
            catch (Exception)
            {
                // Don't raise an exception here
            }

            return false;
        }

        /// <summary>
        /// This method writes out the references for the entity.
        /// </summary>
        /// <param name="transform">The entity transform.</param>
        /// <param name="reference">The reference.</param>
        /// <param name="key">The root key.</param>
        /// <param name="version">The entity version.</param>
        /// <param name="expiry">Optional expiry timespan for the key</param>
        /// <returns>Returns an async task.</returns>
        public override async Task<bool> WriteReference(EntityTransformHolder<K, E> transform, Tuple<string, string> reference, K key, string version, TimeSpan? expiry = null)
        {
            if (transform == null)
                throw new ArgumentNullException(nameof(transform), "The EntityTransformHolder cannot be null.");

            try
            {
                IDatabase rDb = mLazyConnection.Value.GetDatabase();
                IBatch batch = rDb.CreateBatch();
                var tasks = new List<Task>(WriteReference(batch, transform, reference, key, version, expiry));
                batch.Execute();

                await Task.WhenAll(tasks);

                return true;
            }
            catch (Exception)
            {
                // Don't raise an exception here
            }

            return false;
        }
        /// <summary>
        /// Writes the version out for the entity key
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="key"></param>
        /// <param name="version"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public override async Task<bool> WriteVersion(EntityTransformHolder<K, E> transform, K key, string version, TimeSpan? expiry = null)
        {
            if (transform == null)
                throw new ArgumentNullException(nameof(transform), "The EntityTransformHolder cannot be null.");

            try
            {
                IDatabase rDb = mLazyConnection.Value.GetDatabase();
                IBatch batch = rDb.CreateBatch();
                RedisKey hashkey = RedisKeyGet(transform, key);

                var tasks = new List<Task>
                {
                    //Version
                    batch.HashSetAsync(hashkey, cnKeyVersion, version, when: When.Always),
                    // Expiry
                    batch.KeyExpireAsync(hashkey, expiry ?? mEntityTtl)
                };
                batch.Execute();

                await Task.WhenAll(tasks);

                return true;
            }
            catch (Exception)
            {
                // Don't raise an exception here
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
        /// <param name="expiry">Expiry time of the key</param>
        /// <returns>Returns an async task.</returns>
        protected virtual List<Task> WriteReference(IBatch batch, EntityTransformHolder<K, E> transform, Tuple<string,string> reference, K key, string version, TimeSpan? expiry = null)
        {
            //entityreference.{entitytype}.{keytype i.e., EMAIL, ID etc.}
            RedisKey hashkey = RedisReferenceKeyGet(transform, transform.ReferenceHashMaker(reference));
            return new List<Task>(3)
            {
                batch.HashSetAsync(hashkey, cnKeyEntityId, transform.KeySerializer(key), when: When.Always),
                batch.HashSetAsync(hashkey, cnKeyVersion, version, when: When.Always),
                batch.KeyExpireAsync(hashkey, expiry ?? mEntityTtl)
            };
        }
        #endregion

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
                throw new ArgumentNullException(nameof(transform), "The EntityTransformHolder cannot be null.");
            try
            {
                IDatabase rDb = mLazyConnection.Value.GetDatabase();
                RedisKey hashkey = RedisKeyGet(transform, key);

                //Entity
                RedisValue result = await rDb.HashGetAsync(hashkey, cnKeyEntity);

                if (result.HasValue)
                {
                    var entity = transform.CacheEntitySerializer.Deserializer(result);
                    return new PersistenceResponseHolder<E> { StatusCode = 200, Content = result, IsSuccess = true, Entity = entity, Id = transform.KeySerializer(key), VersionId = transform.Version?.EntityVersionAsString(entity) };
                }

                return new PersistenceResponseHolder<E> { StatusCode = 404, IsSuccess = false };
            }
            catch (Exception)
            {
                return new PersistenceResponseHolder<E> { StatusCode = 500, IsSuccess = false };
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
                throw new ArgumentNullException(nameof(transform), "The EntityTransformHolder cannot be null.");

            try
            {
                var resolve = await RedisResolveReference(transform, reference);

                if (resolve.Item1)
                    return await Read(transform, resolve.Item2);

                return new PersistenceResponseHolder<E>() { StatusCode = 404, IsSuccess = false };
            }
            catch (Exception)
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
        /// <returns>Returns true if the entity was deleted from the cache.</returns>
        public override async Task<bool> Delete(EntityTransformHolder<K, E> transform, K key)
        {
            if (transform == null)
                throw new ArgumentNullException(nameof(transform), "The EntityTransformHolder cannot be null.");

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
            catch (Exception)
            {
                // Don't raise an exception here
            }

            return false;
        }
        #endregion
        #region Delete(EntityTransformHolder<K, E> transform, Tuple<string, string> reference)
        /// <summary>
        /// This method resolves the reference from the cache and then deletes the entity.
        /// </summary>
        /// <param name="transform">The transform.</param>
        /// <param name="reference">The key/value reference pair.</param>
        /// <returns>Returns true if the entity was deleted from the cache.</returns>
        public override async Task<bool> Delete(EntityTransformHolder<K, E> transform, Tuple<string, string> reference)
        {
            if (transform == null)
                throw new ArgumentNullException(nameof(transform), "The EntityTransformHolder cannot be null.");

            try
            {
                var resolve = await RedisResolveReference(transform, reference);

                if (resolve.Item1)
                    return await Delete(transform, resolve.Item2);
            }
            catch (Exception)
            {
                // Don't raise and exception here
            }

            return false;
        } 
        #endregion

        #region VersionRead(EntityTransformHolder<K, E> transform, K key)
        /// <summary>
        /// This method reads the version from the cache.
        /// </summary>
        /// <param name="transform">The entity transform.</param>
        /// <param name="key">The entity key.</param>
        /// <returns>Returns the async task.</returns>
        public override async Task<IResponseHolder<Tuple<K, string>>> VersionRead(EntityTransformHolder<K, E> transform, K key)
        {
            if (transform == null)
                throw new ArgumentNullException(nameof(transform), "The EntityTransformHolder cannot be null.");
            try
            {
                IDatabase rDb = mLazyConnection.Value.GetDatabase();
                RedisKey hashkey = RedisKeyGet(transform, key);

                //Entity
                RedisValue result = await rDb.HashGetAsync(hashkey, cnKeyVersion);

                if (result.HasValue)
                    return new PersistenceResponseHolder<Tuple<K, string>> { StatusCode = 200, IsSuccess = true, VersionId = result, Id = transform.KeySerializer(key), Entity = new Tuple<K, string>(key, result)};

                return new PersistenceResponseHolder<Tuple<K, string>> { StatusCode = 404, IsSuccess = false };
            }
            catch (Exception)
            {
                return new PersistenceResponseHolder<Tuple<K, string>> { StatusCode = 500, IsSuccess = false };
            }
        }
        #endregion
        #region VersionRead(EntityTransformHolder<K, E> transform, Tuple<string, string> reference)
        /// <summary>
        /// This method returns the reference key and version for the suple reftype/value pair.
        /// </summary>
        /// <param name="transform">The transform.</param>
        /// <param name="reference">The tuple reference.</param>
        /// <returns>Returns the response holder with a response code of 200 if successful</returns>
        public override async Task<IResponseHolder<Tuple<K, string>>> VersionRead(EntityTransformHolder<K, E> transform, Tuple<string, string> reference)
        {
            if (transform == null)
                throw new ArgumentNullException(nameof(transform), "The EntityTransformHolder cannot be null.");

            try
            {
                var resolve = await RedisResolveReference(transform, reference);

                if (resolve.Item1)
                    return new PersistenceResponseHolder<Tuple<K, string>> { StatusCode = 200, IsSuccess = true, Id = transform.KeySerializer(resolve.Item2), VersionId = resolve.Item3, Entity = new Tuple<K, string>(resolve.Item2, resolve.Item3 )};

                return new PersistenceResponseHolder<Tuple<K, string>> { StatusCode = 404, IsSuccess = false };
            }
            catch (Exception)
            {
                return new PersistenceResponseHolder<Tuple<K, string>> { StatusCode = 500, IsSuccess = false };
            }
        } 
        #endregion
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
        /// <param name="readOnly"></param>
        /// <param name="entityTtl"></param>
        /// <returns></returns>
        public static RedisCacheManager<K, E> Default<K, E>(string connection, bool readOnly = false, TimeSpan? entityTtl = null) where K : IEquatable<K>
        {
            return new RedisCacheManager<K, E>(connection, readOnly, entityTtl: entityTtl);
        }
    }
}
