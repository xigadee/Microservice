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
    /// 
    /// </summary>
    /// <typeparam name="I"></typeparam>
    public abstract class CollectionContainerBase<I>: CollectionContainerBase<I,CollectionStatistics>
    {
        protected CollectionContainerBase(IEnumerable<I> items):base(items)
        {
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="I"></typeparam>
    /// <typeparam name="S">The statistics.</typeparam>
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
