using System;
using System.Threading.Tasks;

namespace Xigadee
{
    public abstract class CacheManagerBase<K, E>: ICacheManager<K, E> 
        where K : IEquatable<K>
    {
        private readonly bool mReadOnly;

        protected EntityTransformHolder<K, E> mTransform = null;

        protected CacheManagerBase(bool readOnly = true)
        {
            mReadOnly = readOnly;
        }

        public abstract bool IsActive { get; }

        public virtual bool IsReadOnly => mReadOnly;

        public abstract Task<IResponseHolder<E>> Read(EntityTransformHolder<K, E> transform, Tuple<string, string> reference);

        public abstract Task<IResponseHolder<E>> Read(EntityTransformHolder<K, E> transform, K key);

        public abstract Task<IResponseHolder<Tuple<K, string>>> VersionRead(EntityTransformHolder<K, E> transform, Tuple<string, string> reference);

        public abstract Task<IResponseHolder<Tuple<K, string>>> VersionRead(EntityTransformHolder<K, E> transform, K key);

        public abstract Task<bool> Write(EntityTransformHolder<K, E> transform, E entity, TimeSpan? expiry = null);

        public abstract Task<bool> WriteReference(EntityTransformHolder<K, E> transform, Tuple<string, string> reference, K key, string version, TimeSpan? expiry = null);

        public abstract Task<bool> WriteVersion(EntityTransformHolder<K, E> transform, K key, string version, TimeSpan? expiry = null);

        public abstract Task<bool> Delete(EntityTransformHolder<K, E> transform, K key);

        public abstract Task<bool> Delete(EntityTransformHolder<K, E> transform, Tuple<string, string> reference);

        public Task<bool> Write(E entity, TimeSpan? expiry = null)
        {
            return Write(mTransform, entity, expiry);
        }

        public Task<bool> WriteReference(Tuple<string, string> reference, K key, string version,TimeSpan? expiry = null)
        {
            return WriteReference(mTransform, reference, key, version, expiry);
        }

        public Task<bool> WriteVersion(K key, string version, TimeSpan? expiry = null)
        {
            return WriteVersion(mTransform, key, version, expiry);

        }

        public Task<bool> Delete(K key)
        {
            return Delete(mTransform, key);
        }

        public Task<bool> Delete(Tuple<string, string> reference)
        {
            return Delete(mTransform, reference);
        }

        public Task<IResponseHolder<E>> Read(K key)
        {
            return Read(mTransform, key);
        }

        public Task<IResponseHolder<E>> Read(Tuple<string, string> reference)
        {
            return Read(mTransform, reference);
        }

        public Task<IResponseHolder<Tuple<K, string>>> VersionRead(K key)
        {
            return VersionRead(mTransform, key);
        }

        public Task<IResponseHolder<Tuple<K, string>>> VersionRead(Tuple<string, string> reference)
        {
            return VersionRead(mTransform, reference);
        }
    }
}
