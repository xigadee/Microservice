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

        protected readonly ConcurrentDictionary<K, EntityCacheHoldar<K, E>> mEntities;

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
            EntityCacheHoldar<K, E> holder;
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
