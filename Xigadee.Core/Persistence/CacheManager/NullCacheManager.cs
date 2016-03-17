using System;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// The null cache manager is used to provide an object reference that does not cache.
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="E"></typeparam>
    public class NullCacheManager<K, E>: CacheManagerBase<K, E>
        where K : IEquatable<K>
    {
        public override bool IsActive
        {
            get
            {
                return false;
            }
        }

        public override bool IsReadOnly
        {
            get
            {
                return true;
            }
        }

        public override async Task<bool> Delete(EntityTransformHolder<K, E> transform, Tuple<string, string> reference)
        {
            return false;
        }

        public override async Task<bool> Delete(EntityTransformHolder<K, E> transform, K key)
        {
            return false;
        }

        public override async Task<IResponseHolder<E>> Read(EntityTransformHolder<K, E> transform, Tuple<string, string> reference)
        {
            return new PersistenceResponseHolder<E> { IsSuccess = false, StatusCode = 404 };
        }

        public override async Task<IResponseHolder<E>> Read(EntityTransformHolder<K, E> transform, K key)
        {
            return new PersistenceResponseHolder<E> { IsSuccess = false, StatusCode = 404 };
        }

        public override async Task<IResponseHolder> VersionRead(EntityTransformHolder<K, E> transform, Tuple<string, string> reference)
        {
            return new PersistenceResponseHolder { IsSuccess = false, StatusCode = 404 };
        }

        public override async Task<IResponseHolder> VersionRead(EntityTransformHolder<K, E> transform, K key)
        {
            return new PersistenceResponseHolder { IsSuccess = false, StatusCode = 404 };
        }

        public override async Task<bool> Write(EntityTransformHolder<K, E> transform, E entity, TimeSpan? expiry = null)
        {
            return false;
        }
    }
}
