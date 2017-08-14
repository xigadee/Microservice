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
    #region CollectionContainerBase<I>
    /// <summary>
    /// This is the base collection container.
    /// </summary>
    /// <typeparam name="I">The collection type.</typeparam>
    public abstract class CollectionContainerBase<I>: CollectionContainerBase<I, CollectionStatistics>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionContainerBase{I}"/> class.
        /// </summary>
        /// <param name="items">The items to preload in to the collection.</param>
        protected CollectionContainerBase(IEnumerable<I> items) : base(items)
        {
        }
    } 
    #endregion

    /// <summary>
    /// This is the base collection container.
    /// </summary>
    /// <typeparam name="I">The collection type.</typeparam>
    /// <typeparam name="S">The statistics type.</typeparam>
    public abstract class CollectionContainerBase<I,S>: ServiceBase<S>, IContainerService
        where S : StatusBase, ICollectionStatistics, new()
    {
        #region Events        
        /// <summary>
        /// Occurs when [an item is added to the collection.
        /// </summary>
        public event EventHandler<I> OnAdd;
        /// <summary>
        /// Occurs when an item is removed from the collection.
        /// </summary>
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

        /// <summary>
        /// Gets the internal container.
        /// </summary>
        protected ICollection ContainerInternal
        {
            get{ return mContainer; }
        }

        /// <summary>
        /// Recalculates the statistics.
        /// </summary>
        /// <param name="stats">The stats.</param>
        protected override void StatisticsRecalculate(S stats)
        {
            base.StatisticsRecalculate(stats);

            stats.ItemCount = Count;
        }

        /// <summary>
        /// Clears the collection.
        /// </summary>
        public virtual void Clear()
        {
            Interlocked.Exchange(ref mContainer, new ConcurrentBag<I>());
        }
        /// <summary>
        /// Gets the items in the collection.
        /// </summary>
        public IEnumerable<I> Items
        {
            get
            {
                return mContainer;
            }
        }
        /// <summary>
        /// Gets the count for the collection.
        /// </summary>
        public int Count
        {
            get { return mContainer.Count; }
        }
        /// <summary>
        /// Gets any registered services in the collection.
        /// </summary>
        public IEnumerable<IService> Services
        {
            get 
            {
                return Items.Where((i) => i is IService).Cast<IService>();
            }
        }
        /// <summary>
        /// Adds the specified item to the collection.
        /// </summary>
        /// <param name="item">The item to add.</param>
        protected virtual void Add(I item)
        {
            mContainer.Add(item);
            try
            {
                OnAdd?.Invoke(this, item);
            }
            catch (Exception)
            {
            }
        }
        /// <summary>
        /// Removes the specified item from the collection.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <returns>Returns true if the item is removed successfully.</returns>
        protected virtual bool Remove(I item)
        {
            var list = mContainer.ToList();
            if (!list.Remove(item))
                return false;

            var newBag = new ConcurrentBag<I>(list);

            Interlocked.Exchange(ref mContainer, newBag);

            try
            {
                OnRemove?.Invoke(this, item);
            }
            catch (Exception)
            {
            }

            return true;
        }

        #region StartInternal/StopInternal
        /// <summary>
        /// This method starts the service. You should override this method for your own logic and implement your specific startup implementation.
        /// </summary>
        protected override void StartInternal()
        {
        }
        /// <summary>
        /// This method stops the service. You should override this method for your own logic.
        /// </summary>
        protected override void StopInternal()
        {
        } 
        #endregion
    }
}
