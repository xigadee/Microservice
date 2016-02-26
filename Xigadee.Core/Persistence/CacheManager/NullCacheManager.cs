using System;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// The null cache manager is used to provide an object reference that does not cache.
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="E"></typeparam>
    public class NullCacheManager<K, E>: ICacheManager<K, E>
        where K : IEquatable<K>
    {
        public bool IsActive
        {
            get
            {
                return false;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return true;
            }
        }

        public Task<bool> CanRead(K key)
        {
            throw new NotImplementedException();
        }

        public Task Delete(K key)
        {
            throw new NotImplementedException();
        }

        public Task<IResponseHolder> Read(K key)
        {
            throw new NotImplementedException();
        }

        public Task<IResponseHolder> Read(string refType, string refValue)
        {
            throw new NotImplementedException();
        }

        public Task<IResponseHolder> VersionRead(K key)
        {
            throw new NotImplementedException();
        }

        public Task<IResponseHolder> VersionRead(string refType, string refValue)
        {
            throw new NotImplementedException();
        }

        public Task VersionWrite(K key, IResponseHolder result)
        {
            throw new NotImplementedException();
        }

        public Task Write(K key, IResponseHolder result)
        {
            throw new NotImplementedException();
        }
    }
}
