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

using System.Threading;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This is a concrete implementation of the base class that allows the key parts of the class to be implemented in the underlying container.
    /// </summary>
    /// <typeparam name="D">The data type.</typeparam>
    /// <typeparam name="I">The interface type.</typeparam>
    public class ActionQueueCollection<D, I>: ActionQueueCollectionBase<D, I, ActionQueueCollectionStatistics, ActionQueuePolicy>
    {
        Action<D, I> mAction;

        /// <summary>
        /// This is the default constructor for the queue implementation.
        /// </summary>
        /// <param name="items">The underlying items.</param>
        /// <param name="policy">The policy.</param>
        /// <param name="action">The action.</param>
        public ActionQueueCollection(IEnumerable<I> items, ActionQueuePolicy policy, Action<D,I> action) : base(items, policy)
        {
            mAction = action;
        }

        /// <summary>
        /// This override executes the action passed in the constructor.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="item">The event item.</param>
        protected override void Process(D data, I item)
        {
            mAction(data,item);
        }
    }

    /// <summary>
    /// Thia collection is used by Logging and EventSource
    /// </summary>
    /// <typeparam name="D"></typeparam>
    /// <typeparam name="I"></typeparam>
    /// <typeparam name="S">The statistics</typeparam>
    /// <typeparam name="P">The policy</typeparam>
    public abstract class ActionQueueCollectionBase<D, I, S, P>: CollectionContainerBase<I, S>, IActionQueue, ITaskManagerProcess
        where S : ActionQueueCollectionStatistics, new()
        where P : ActionQueuePolicy, new()
    {
        #region Declarations
        /// <summary>
        /// This is the underlying policy.
        /// </summary>
        protected P mPolicy;
        /// <summary>
        /// This is the concurrent queue that contains the incoming messages.
        /// </summary>
        protected ConcurrentQueue<ActionQueueContainer<D>> mQueue;
        /// <summary>
        /// This is the mRE that is used to signal incoming messages to the thread loop.
        /// </summary>
        protected ManualResetEventSlim mReset;
        /// <summary>
        /// This is the thread loop used to log messages.
        /// </summary>
        protected Thread mThreadLog;
        /// <summary>
        /// This counter holds the current actve overload process tasks.
        /// </summary>
        protected int mOverloadProcessCount = 0;
        /// <summary>
        /// The number of overload hits.
        /// </summary>
        protected long mOverloadProcessHits = 0; 
        /// <summary>
        /// This is teh number of time the overload process has been hit.
        /// </summary>
        protected int mOverloadTaskCount = 0;
        #endregion
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="items">The underlying action items.</param>
        /// <param name="policy">The collection policy.</param>
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
        /// This property indicates whether the collection is active.
        /// </summary>
        protected virtual bool Active
        {
            get; set;
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
        /// This counter holds the current actve overload process tasks.
        /// </summary>
        public int OverloadProcessCount { get { return mOverloadProcessCount; } }
        #endregion
        #region OverloadProcessHits
        /// <summary>
        /// This count holds the number of time the overload process has been triggered for the class.
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
                return ProcessQueue(mPolicy.OverloadProcessTimeInMs);
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
                int count = ProcessQueue();
            }
        }
        #endregion
        #region ProcessQueue(int? timespaninms = null)
        /// <summary>
        /// This method will write the current items in the queue to the stream processor.
        /// </summary>
        protected virtual int ProcessQueue(int? timespaninms = null)
        {
            ActionQueueContainer<D> logEvent;

            DateTime start = DateTime.UtcNow;

            int items = 0;
            do
            {
                while (mQueue.TryDequeue(out logEvent))
                {
                    EventWrite(logEvent.Data);

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

        #region EventSubmit(D eventData, bool async)
        /// <summary>
        /// This method submits a data item for processing.
        /// </summary>
        /// <param name="eventData">The event data.</param>
        /// <param name="async">A flag indicating whether the data should be processed immediately or enqueued.</param>
        public virtual void EventSubmit(D eventData, bool async)
        {
            if (!Active)
                throw new ServiceNotStartedException();

            if (async)
                EventEnqueue(eventData);
            else
                EventWrite(eventData);
        }
        #endregion

        #region EventWrite(D eventData)
        /// <summary>
        /// This method calls the process method for each of the underlying items, for the eventData. 
        /// </summary>
        /// <param name="eventData">The event data.</param>
        protected virtual void EventWrite(D eventData)
        {
            //Parallel.ForEach(Items, (l) => ProcessItem(eventData, l));
            Items.ForEach((l) => ProcessItem(eventData, l));
        }
        #endregion
        #region EventEnqueue(D eventData)
        /// <summary>
        /// This method queues the incoming event data to be processed on the queue thread.
        /// </summary>
        /// <param name="eventData">The incoming event</param>
        protected virtual void EventEnqueue(D eventData)
        {
            if (!Active)
                throw new ServiceNotStartedException();

            var item = new ActionQueueContainer<D> { Data = eventData, Timestamp = StatisticsInternal.ActiveIncrement() };

            mQueue.Enqueue(item);

            mReset.Set();
        }
        #endregion

        #region ProcessItem(D eventData, I item)
        /// <summary>
        /// This method wraps the individual request in a safe wrapper.
        /// </summary>
        /// <param name="eventData">The event data.</param>
        /// <param name="item">The item to process.</param>
        protected virtual void ProcessItem(D eventData, I item)
        {
            try
            {
                Process(eventData, item);
            }
            catch (Exception ex)
            {
                //We don't want unexpected exceptions here and to stop the other loggers working.
                StatisticsInternal.ErrorIncrement();
                StatisticsInternal.Ex = ex;
            }
        }
        #endregion

        /// <summary>
        /// This abstract method should be overridden to process the specific event data against the specific container.
        /// </summary>
        /// <param name="eventData">The event data.</param>
        /// <param name="container">The container.</param>
        protected abstract void Process(D eventData, I container);

        #region class -> ActionQueueContainer<IR>
        /// <summary>
        /// This is an internal class which is used to hold the pending request.
        /// </summary>
        /// <typeparam name="IR">The item type.</typeparam>
        protected class ActionQueueContainer<IR>
        {
            /// <summary>
            /// The time stamp.
            /// </summary>
            public int Timestamp { get; set; }
            /// <summary>
            /// The queued data.
            /// </summary>
            public IR Data { get; set; }
        } 
        #endregion
    }

}
