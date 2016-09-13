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

#region using
using System;
using System.Threading;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This class is used to hold a specific entity in the collection, and hold the associated
    /// metadata for the entity.
    /// </summary>
    /// <typeparam name="K">The entity key type.</typeparam>
    /// <typeparam name="E">The entity type.</typeparam>
    public class EntityCacheHolder<K, E> 
        where K : IEquatable<K>
        where E : class
    {
        #region Declarations
        private readonly ManualResetEvent mWait;

        public readonly K Key;

        private E mEntity = default(E);

        public readonly DateTime Expiry;

        private long mHitCount = 0;

        private int mWaiting = 0;

        private int? mResponseCode = null; 
        #endregion

        public EntityCacheHolder(K key, TimeSpan? span = null)
        {
            mWait = new ManualResetEvent(false);
            mWait.Reset();

            State = EntityCacheHolderState.Start;
            Key = key;
            Expiry = DateTime.UtcNow.Add(span ?? TimeSpan.FromDays(1));
        }

        public int Waiting { get { return mWaiting; } }

        public long HitCount
        {
            get
            {
                return mHitCount;
            }
        }

        public EntityCacheHolderState State { get; set; }

        public E Entity
        {
            get
            {
                return mEntity;
            }
        }

        public bool EntityExists
        {
            get
            {
                return mResponseCode == 200 && mEntity != null;
            }
        }

        public void Load(E entity, int responseCode)
        {
            mEntity = entity;
            mResponseCode = responseCode;
            State = EntityCacheHolderState.Completed;
            mWait.Set();
        }

        /// <summary>
        /// This is the debug string used for statistics.
        /// </summary>
        public string Debug
        {
            get
            {
                try
                {
                    return string.Format("{0}={1} @ {2} {3} Hits={4} Waiting={5} Expires={6}", Key.ToString(), mResponseCode, State, EntityExists, mHitCount, mWaiting, Expiry);
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }

        public void Cancel()
        {
            State = EntityCacheHolderState.Cancelled;
            mWait.Set();
        }

        public EntityCacheResult<E> ToCacheResult()
        {
            Interlocked.Increment(ref mHitCount);
            return new EntityCacheResult<E> { Success = State == EntityCacheHolderState.Completed, Entity = Entity, Exists = EntityExists };
        }

        public void Wait()
        {
            Interlocked.Increment(ref mWaiting);
            mWait.WaitOne(250);
            Interlocked.Decrement(ref mWaiting);
        }

        public void Release()
        {
            mWait.Set();
        }
    }
}
