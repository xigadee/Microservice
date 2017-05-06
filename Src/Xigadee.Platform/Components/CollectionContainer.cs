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
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Threading;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This is the base collection container.
    /// </summary>
    /// <typeparam name="I">The collection type.</typeparam>
    public abstract class CollectionContainerBase<I>: CollectionContainerBase<I,CollectionStatistics>
    {
        protected CollectionContainerBase(IEnumerable<I> items):base(items)
        {
        }
    }

    /// <summary>
    /// This is the base collection container.
    /// </summary>
    /// <typeparam name="I">The collection type.</typeparam>
    /// <typeparam name="S">The statistics type.</typeparam>
    public abstract class CollectionContainerBase<I,S>: ServiceBase<S>, IContainerService
        where S : StatusBase, ICollectionStatistics, new()
    {
        #region Events
        public event EventHandler<I> OnAdd;
        public event EventHandler<I> OnRemove; 
        #endregion
        #region Declarations
        private ConcurrentBag<I> mContainer;
        #endregion

        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="items">The items to preload in to the collection.</param>
        protected CollectionContainerBase(IEnumerable<I> items)
        {
            if (items == null)
                mContainer = new ConcurrentBag<I>();
            else
                mContainer = new ConcurrentBag<I>(items);
        } 
        #endregion

        protected ICollection ContainerInternal
        {
            get{ return mContainer; }
        }

        protected override void StatisticsRecalculate(S stats)
        {
            base.StatisticsRecalculate(stats);

            stats.ItemCount = Count;
        }

        public virtual void Clear()
        {
            Interlocked.Exchange(ref mContainer, new ConcurrentBag<I>());
        }

        public IEnumerable<I> Items
        {
            get
            {
                return mContainer;
            }
        }

        public int Count
        {
            get { return mContainer.Count; }
        }

        public IEnumerable<IService> Services
        {
            get 
            {
                return Items.Where((i) => i is IService).Cast<IService>();
            }
        }

        protected virtual void Add(I item)
        {
            mContainer.Add(item);
            if (OnAdd != null)
                try
                {
                    OnAdd(this, item);
                }
                catch (Exception)
                {
                }
        }

        protected virtual bool Remove(I item)
        {
            var list = mContainer.ToList();
            if (!list.Remove(item))
                return false;

            var newBag = new ConcurrentBag<I>(list);

            Interlocked.Exchange(ref mContainer, newBag);

            return true;
        }

        protected override void StartInternal()
        {
        }

        protected override void StopInternal()
        {
        }
    }
}
