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

        protected CacheManagerBase(bool readOnly = true)
        {
            mReadOnly = readOnly;
        }

        public abstract bool IsActive { get; }

        public bool IsReadOnly
        {
            get
            {
                return mReadOnly;
            }
        }

        public abstract Task<IResponseHolder<E>> Read(K key);

        public abstract Task<IResponseHolder<E>> Read(string refType, string refValue);

        public abstract Task<IResponseHolder> VersionRead(K key);

        public abstract Task<IResponseHolder> VersionRead(string refType, string refValue);

        public abstract Task VersionWrite(K key, IResponseHolder result);

        public abstract Task Write(K key, IResponseHolder result);

        public abstract Task Delete(K key);

    }
}
