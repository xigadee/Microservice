#region using
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading;
#endregion
namespace Xigadee
{
    public abstract class ActionQueueCollectionBase<D, I, S>: CollectionContainerBase<I, S>, IActionQueue 
        where S : ActionQueueStatistics, new()
    {
        #region Declarations
        protected ConcurrentQueue<ActionQueueContainer<D>> mQueue;
        protected ManualResetEventSlim mReset;
        protected Thread mThreadLog;
        private bool mActive;
        protected int? mOverloadThreshold;
        protected int mOverloadProcessCount = 0;
        protected long mOverloadProcessHits = 0;
        #endregion
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="items">The underlying action items.</param>
        protected ActionQueueCollectionBase(IEnumerable<I> items, int? overloadThreshold = null) : base(items)
        {
            mQueue = new ConcurrentQueue<ActionQueueContainer<D>>();
            mReset = new ManualResetEventSlim(false);
            mOverloadThreshold = overloadThreshold;
        }
        #endregion

        #region StatisticsRecalculate()
        /// <summary>
        /// This method recalculates the statistics and set the current queuelength.
        /// </summary>
        protected override void StatisticsRecalculate()
        {
            base.StatisticsRecalculate();

            mStatistics.QueueLength = mQueue.Count;
            mStatistics.Overloaded = Overloaded;
            mStatistics.OverloadProcessCount = mOverloadProcessCount;
            mStatistics.OverloadProcessHits = mOverloadProcessHits;
            mStatistics.OverloadThreshold = mOverloadThreshold;
        }
        #endregion

        #region QueueLength
        /// <summary>
        /// This is the length of the queue.
        /// </summary>
        public int QueueLength { get { return mQueue.Count; } }
        #endregion
        #region Overloaded
        /// <summary>
        /// This property goes positive once the queue length exceeds the threshold amount (if set).
        /// </summary>
        public bool Overloaded { get { return mOverloadThreshold.HasValue && mQueue.Count > mOverloadThreshold.Value; } }
        #endregion

        #region Active
        /// <summary>
        /// This property is used for debugging.
        /// </summary>
        protected virtual bool Active
        {
            get
            {
                return mActive;
            }
            set
            {
                mActive = value;
            }
        } 
        #endregion

        /// <summary>
        /// This counter holds the current actve overload process threads.
        /// </summary>
        public int OverloadProcessCount { get { return mOverloadProcessCount; } }

        public long OverloadProcessHits { get { return mOverloadProcessHits; } }


        #region OverloadProcess(int timespaninms)
        /// <summary>
        /// This method allows additional schedule threads to help clear the buffer.
        /// </summary>
        /// <param name="timespaninms">The maximum timespan is milliseconds to execute.</param>
        /// <returns>Returns the number of items processed.</returns>
        public async Task<int> OverloadProcess(int timespaninms)
        {
            Interlocked.Increment(ref mOverloadProcessHits);

            try
            {
                Interlocked.Increment(ref mOverloadProcessCount);
                return WriteBuffer(timespaninms);
            }
            finally
            {
                Interlocked.Decrement(ref mOverloadProcessCount);
            }
        } 
        #endregion

        #region StartInternal/StopInternal
        /// <summary>
        /// This method starts the thread loop.
        /// </summary>
        protected override void StartInternal()
        {
            Active = true;
            mThreadLog = new Thread(SpinWrite);
            mThreadLog.Start();
        }
        /// <summary>
        /// This override stops the logging.
        /// </summary>
        protected override void StopInternal()
        {
            mReset.Set();
            Active = false;
            mThreadLog.Join();
        } 
        #endregion

        #region SpinWrite(object state)
        /// <summary>
        /// This method is used to manage logging using a single thread.
        /// </summary>
        /// <param name="state">The logged state.</param>
        protected virtual void SpinWrite(object state)
        {
            while (Active)
            {
                mReset.Wait(1000);
                mReset.Reset();
                int count = WriteBuffer();
            }
        }
        #endregion


        protected virtual void WriteEvent(D logEvent)
        {
            Items.ForEach((l) =>
            {
                try
                {
                    Process(logEvent, l);
                }
                catch (Exception ex)
                {
                    //We don't want unexpected exceptions here and to stop the other loggers working.
                    mStatistics.ErrorIncrement();
                    mStatistics.Ex = ex;
                }
            });
        }

        #region WriteBuffer()
        /// <summary>
        /// This method will write the current items in the queue to the stream processor.
        /// </summary>
        protected virtual int WriteBuffer(int? timespaninms = null)
        {
            ActionQueueContainer<D> logEvent;
            DateTime start = DateTime.UtcNow;
            int items = 0;
            do
            {
                while (mQueue.TryDequeue(out logEvent))
                {
                    WriteEvent(logEvent.Data);

                    items++;

                    mStatistics.ActiveDecrement(logEvent.Timestamp);

                    //Kick out every 100 loops if there is a timer limit.
                    if (timespaninms.HasValue && (items % 100 == 0))
                        break;
                }
            }
            while (mQueue.Count > 0 && (!timespaninms.HasValue || (DateTime.UtcNow - start).TotalMilliseconds<timespaninms));

            return items;
        }
        #endregion

        protected abstract void Process(D data, I item);

        protected virtual void Enqueue(D data)
        {
            if (!Active)
                throw new ServiceNotStartedException();

            var item = new ActionQueueContainer<D> { Data = data };
            item.Timestamp = mStatistics.ActiveIncrement();
            mQueue.Enqueue(item);
            mReset.Set();
        }

        protected class ActionQueueContainer<I>
        {
            public int Timestamp { get; set; }

            public I Data { get; set; }
        }
    }

}
