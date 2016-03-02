using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Xigadee
{
    /// <summary>
    /// This is the root cache.
    /// </summary>
    public class RedisCacheManager
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

    public class RedisCacheManager<K, E>: RedisCacheManager, ICacheManager<K, E>
        where K : IEquatable<K>
    {
        private string mConnection;

        private Lazy<ConnectionMultiplexer> mLazyConnection;


        public RedisCacheManager(string connection, bool readOnly = true)
        {
            mConnection = connection;
            IsReadOnly = readOnly;

            mLazyConnection
            = new Lazy<ConnectionMultiplexer>(() =>
            {
                try
                {
                    return ConnectionMultiplexer.Connect(mConnection);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        );
        }

        #region IsActive
        /// <summary>
        /// This property specifies that the cache can be used.
        /// </summary>
        public bool IsActive
        {
            get
            {
                return true;
            }
        }
        #endregion
        #region IsReadOnly
        /// <summary>
        /// This property specifies whether the cache should support Delete, Write and VersionWrite.
        /// </summary>
        public bool IsReadOnly
        {
            get;private set;
        }
        #endregion

        public async Task<bool> Write(K key, string value)
        {
            try
            {
                IDatabase mRedisCacheDb = mLazyConnection.Value.GetDatabase();
                RedisKey hashkey = typeof(E).Name.ToLowerInvariant(); ;
                RedisValue redKey = key.ToString();
                RedisValue redData = value;
                
                return await mRedisCacheDb.HashSetAsync(hashkey, redKey, redData, when: When.Always);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task Write(K key, IResponseHolder result)
        {
            if (IsReadOnly || !result.IsSuccess || result.IsTimeout)
                return;

            await Write(key, result.Content);
        }

        public async Task<IResponseHolder<E>> Read(K key)
        {
            //IDatabase mRedisCacheDb = mLazyConnection.Value.GetDatabase();
            //RedisKey hashkey = typeof(E).Name.ToLowerInvariant(); ;
            //RedisValue redKey = key.ToString();

            //var redValue = await mRedisCacheDb.HashGetAsync(hashkey, redKey);

            //var result = new ResponseHolder<E>();

            //return result;

            throw new NotImplementedException();
        }

        public async Task<IResponseHolder<E>> Read(string refType, string refValue)
        {
            throw new NotImplementedException();
        }

        public async Task<IResponseHolder> VersionRead(K key)
        {
            throw new NotImplementedException();
        }

        public async Task<IResponseHolder> VersionRead(string refType, string refValue)
        {
            throw new NotImplementedException();
        }

        public async Task VersionWrite(K key, IResponseHolder result)
        {
            if (IsReadOnly)
                return;

            throw new NotImplementedException();
        }


        public async Task Delete(K key)
        {
            if (IsReadOnly)
                return;

            throw new NotImplementedException();
        }

    }
}
