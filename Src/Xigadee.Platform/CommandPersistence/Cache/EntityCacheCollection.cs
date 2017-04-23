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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Xigadee
{
    public class EntityCacheCollection<K,E>
        where K : IEquatable<K>
        where E : class
    {
        protected long mAdded = 0;
        protected long mRemoved = 0;

        protected readonly ConcurrentDictionary<K, EntityCacheHolder<K, E>> mEntities;

        public EntityCacheCollection()
        {

        }


        #region Remove(K key)
        /// <summary>
        /// This method removes an item from the cache collection.
        /// </summary>
        /// <param name="key">The item key.</param>
        protected virtual bool Remove(K key)
        {
            EntityCacheHolder<K, E> holder;
            if (mEntities.TryRemove(key, out holder))
            {
                holder.Cancel();
                Interlocked.Increment(ref mRemoved);
                return true;
            }

            return false;
        }
        #endregion
    }
}
