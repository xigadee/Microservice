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
    /// <summary>
    /// Thia collection is used by 
    /// </summary>
    /// <typeparam name="D"></typeparam>
    /// <typeparam name="I"></typeparam>
    /// <typeparam name="S"></typeparam>
    /// <typeparam name="P"></typeparam>
    public abstract class ActionQueueCollectionBase<D, I, S, P>: CollectionContainerBase<I, S>
        , IActionQueue, ITaskManagerProcess
        where S : ActionQueueCollectionStatistics, new()
        where P : ActionQueuePolicy, new()
    {
        #region Declarations
        protected P mPolicy;

        protected ConcurrentQueue<ActionQueueContainer<D>> mQueue;

        protected ManualResetEventSlim mReset;

        protected Thread mThreadLog;

        private bool mActive;
        
        protected int mOverloadProcessCount = 0;

        protected long mOverloadProcessHits = 0; 

        private int mOverloadTaskCount = 0;

        #endregion
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="items">The underlying action items.</param>
        protected ActionQueueCollectionBase(IEnumerable<I> items, P policy = null) : base(items)
        {
            mQueue = new ConcurrentQueue<ActionQueueContainer<D>>();
            mReset = new ManualResetEventSlim(false);
            mPolicy = policy ?? new P();
        }
        #endregion

        #region StatisticsRecalculate()
        /// <summary>
        /// This method recalculates the statistics and set the current queuelength.
        /// </summary>
        protected override void StatisticsRecalculate(S stats)
        {
            base.StatisticsRecalculate(stats);

            stats.QueueLength = mQueue.Count;
            stats.Overloaded = Overloaded;
            stats.OverloadProcessCount = mOverloadProcessCount;
            stats.OverloadProcessHits = mOverloadProcessHits;
            stats.OverloadThreshold = mPolicy.OverloadThreshold;
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
        public bool Overloaded { get { return mPolicy.OverloadThreshold.HasValue && mQueue.Count > mPolicy.OverloadThreshold.Value; } }
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

        #region TaskSubmit
        /// <summary>
        /// This is the action path back to the TaskManager.
        /// </summary>
        public Action<TaskTracker> TaskSubmit
        {
            get; set;
        }
        #endregion
        #region TaskAvailability
        /// <summary>
        /// This is the task availability collection
        /// </summary>
        public ITaskAvailability TaskAvailability { get; set; }
        #endregion

        #region OverloadProcessCount
        /// <summary>
        /// This counter holds the current actve overload process threads.
        /// </summary>
        public int OverloadProcessCount { get { return mOverloadProcessCount; } }
        #endregion
        #region OverloadProcessHits
        /// <summary>
        /// 
        /// </summary>
        public long OverloadProcessHits { get { return mOverloadProcessHits; } } 
        #endregion

        #region OverloadProcess()
        /// <summary>
        /// This method allows additional schedule threads to help clear the buffer.
        /// </summary>
        /// <returns>Returns the number of items processed.</returns>
        public async Task<int> OverloadProcess()
        {
            Interlocked.Increment(ref mOverloadProcessHits);

            try
            {
                Interlocked.Increment(ref mOverloadProcessCount);
                return WriteBuffer(mPolicy.OverloadProcessTimeInMs);
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

        #region CanProcess()
        /// <summary>
        /// This method will return true if overloaded and there are no overload tasks running.
        /// </summary>
        /// <returns>Returns true if the process can proceed.</returns>
        public virtual bool CanProcess()
        {
            return Status == ServiceStatus.Running && Overloaded && mOverloadTaskCount < mPolicy.OverloadMaxTasks;
        }
        #endregion
        #region --> Process()
        /// <summary>
        /// This method checks whether the process is overloaded and schedules a long running task to reduce the overload.
        /// </summary>
        public virtual void Process()
        {
            if (!Overloaded)
                return;

            TaskTracker tracker = new TaskTracker(TaskTrackerType.Overload, null);

            tracker.Name = GetType().Name;
            tracker.Caller = GetType().Name;
            tracker.IsLongRunning = true;
            tracker.Priority = 3;

            tracker.Execute = async (token) => await OverloadProcess();

            tracker.ExecuteComplete = (t, s, ex) => Interlocked.Decrement(ref mOverloadTaskCount);

            Interlocked.Increment(ref mOverloadTaskCount);

            TaskSubmit(tracker);
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
                    StatisticsInternal.ErrorIncrement();
                    StatisticsInternal.Ex = ex;
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

                    StatisticsInternal.ActiveDecrement(logEvent.Timestamp);

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
            item.Timestamp = StatisticsInternal.ActiveIncrement();
            mQueue.Enqueue(item);
            mReset.Set();
        }

        #region class -> ActionQueueContainer<I>
        /// <summary>
        /// This is an internal class which is used to hold the pending request.
        /// </summary>
        /// <typeparam name="I">The item type.</typeparam>
        protected class ActionQueueContainer<I>
        {
            public int Timestamp { get; set; }

            public I Data { get; set; }
        } 
        #endregion
    }

}
