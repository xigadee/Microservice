using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public class RedisCacheManager<K, E>: ICacheManager<K, E>
        where K : IEquatable<K>
    {
        private readonly bool mReadOnly;

        public RedisCacheManager(string connection
            , bool readOnly = true)
        {
            mReadOnly = readOnly;
        }

        public bool IsActive
        {
            get
            {
                return true;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return mReadOnly;
            }
        }

        public async Task Delete(K key)
        {
            if (IsReadOnly)
                return;

            throw new NotImplementedException();
        }

        public async Task<IResponseHolder<E>> Read(K key)
        {
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

        public async Task Write(K key, IResponseHolder result)
        {
            if (IsReadOnly)
                return;

            throw new NotImplementedException();
        }
    }
}
