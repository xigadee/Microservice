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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public abstract class CacheManagerBase<K, E>: ICacheManager<K, E> where K : IEquatable<K>
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
