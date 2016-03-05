using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public virtual bool IsReadOnly
        {
            get
            {
                return mReadOnly;
            }
        }

        public abstract Task<IResponseHolder<E>> Read(EntityTransformHolder<K, E> transform, Tuple<string, string> reference);

        public abstract Task<IResponseHolder<E>> Read(EntityTransformHolder<K, E> transform, K key);

        public abstract Task<IResponseHolder> VersionRead(EntityTransformHolder<K, E> transform, Tuple<string, string> reference);

        public abstract Task<IResponseHolder> VersionRead(EntityTransformHolder<K, E> transform, K key);

        public abstract Task<bool> Write(EntityTransformHolder<K, E> transform, E entity);

        public abstract Task<bool> Delete(EntityTransformHolder<K, E> transform, K key);


        public Task<bool> Write(E entity)
        {
            return Write(mTransform, entity);
        }

        public Task<bool> Delete(K key)
        {
            return Delete(mTransform, key);
        }

        public Task<IResponseHolder<E>> Read(K key)
        {
            return Read(mTransform, key);
        }

        public Task<IResponseHolder<E>> Read(Tuple<string, string> reference)
        {
            return Read(mTransform, reference);
        }

        public Task<IResponseHolder> VersionRead(K key)
        {
            return VersionRead(mTransform, key);
        }

        public Task<IResponseHolder> VersionRead(Tuple<string, string> reference)
        {
            return VersionRead(mTransform, reference);
        }
    }
}
