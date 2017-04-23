using System;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// The null cache manager is used to provide an object reference that does not cache.
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="E"></typeparam>
    public class NullCacheManager<K, E>: CacheManagerBase<K, E> where K : IEquatable<K>
    {
        public override bool IsActive => false;

        public override bool IsReadOnly => true;

        public override Task<bool> Delete(EntityTransformHolder<K, E> transform, Tuple<string, string> reference)
        {
            return Task.FromResult(false);
        }
        public override Task<bool> Delete(EntityTransformHolder<K, E> transform, K key)
        {
            return Task.FromResult(false);
        }

        public override Task<IResponseHolder<E>> Read(EntityTransformHolder<K, E> transform, Tuple<string, string> reference)
        {
            return Task.FromResult(new PersistenceResponseHolder<E> { IsSuccess = false, StatusCode = 404 } as IResponseHolder<E>);
        }

        public override Task<IResponseHolder<E>> Read(EntityTransformHolder<K, E> transform, K key)
        {
            return Task.FromResult(new PersistenceResponseHolder<E> { IsSuccess = false, StatusCode = 404 } as IResponseHolder<E>);
        }

        public override Task<IResponseHolder<Tuple<K, string>>> VersionRead(EntityTransformHolder<K, E> transform, Tuple<string, string> reference)
        {
            return Task.FromResult(new PersistenceResponseHolder<Tuple<K, string>> { IsSuccess = false, StatusCode = 404 } as IResponseHolder<Tuple<K, string>>);
        }

        public override Task<IResponseHolder<Tuple<K, string>>> VersionRead(EntityTransformHolder<K, E> transform, K key)
        {
            return Task.FromResult(new PersistenceResponseHolder<E> { IsSuccess = false, StatusCode = 404 } as IResponseHolder<Tuple<K, string>>);
        }

        public override Task<bool> Write(EntityTransformHolder<K, E> transform, E entity, TimeSpan? expiry = null)
        {
            return Task.FromResult(false);
        }

        public override Task<bool> WriteReference(EntityTransformHolder<K, E> transform, Tuple<string, string> reference, K key, string version, TimeSpan? expiry = null)
        {
            return Task.FromResult(false);
        }

        public override Task<bool> WriteVersion(EntityTransformHolder<K, E> transform, K key, string version, TimeSpan? expiry = null)
        {
            return Task.FromResult(false);
        }
    }
}
