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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#endregion
namespace Xigadee
{
    /// <summary>
    /// This class tracks any jobs that are enqueued and records how long they are held in the queue.
    /// </summary>
    [DebuggerDisplay("Level={Level} Queue={Count}")]
    public class QueueTracker: StatisticsBase<QueueTrackerStatistics>, IQueueTracker
    {
        #region Declarations
        /// <summary>
        /// This is the internal queue holder.
        /// </summary>
        ConcurrentQueue<QueueTrackerHolder> mQueue; 
        #endregion
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public QueueTracker()
        {
            mQueue = new ConcurrentQueue<QueueTrackerHolder>();
        }
        #endregion

        /// <summary>
        /// Gets or sets the queue level.
        /// </summary>
        public int Level { get; protected set; }

        #region Configure(int level)
        /// <summary>
        /// Gets or sets the queue level.
        /// </summary>
        /// <param name="level">The specific level</param>
        public void Configure(int level)
        {
            Level = level;
        } 
        #endregion

        #region Enqueue(TaskTracker item)
        /// <summary>
        /// This method enqueues the message.
        /// </summary>
        /// <param name="item">The task tracker to enqueue.</param>
        public void Enqueue(TaskTracker item)
        {
            var wrapper = new QueueTrackerHolder { Item = item, Ingress = StatisticsInternal.ActiveIncrement() };
            mQueue.Enqueue(wrapper);
            StatisticsInternal.WaitingSet(Count);
        } 
        #endregion
        #region TryDequeue(out TaskTracker item)
        /// <summary>
        /// This method attemps to dequeue a message.
        /// </summary>
        /// <param name="item">The item dequeued or null</param>
        /// <returns>Returns true if a message was dequeued successfully.</returns>
        public bool TryDequeue(out TaskTracker item)
        {
            item = null;
            QueueTrackerHolder wrapper;
            if (mQueue.TryDequeue(out wrapper))
            {
                item = wrapper.Item;
                StatisticsInternal.ActiveDecrement(wrapper.Ingress);
                return true;
            }
            return false;
        } 
        #endregion

        #region IsEmpty
        /// <summary>
        /// This property returns true if the queue
        /// </summary>
        public bool IsEmpty
        {
            get { return mQueue.IsEmpty; }
        } 
        #endregion
        #region Count
        /// <summary>
        /// This is the number of messages currently in the queue.
        /// </summary>
        public int Count
        {
            get
            {
                return mQueue.Count;
            }
        } 
        #endregion

        /// <summary>
        /// This method is used to set the statistics for the queue.
        /// </summary>
        /// <param name="stats">The incoming statistics.</param>
        protected override void StatisticsRecalculate(QueueTrackerStatistics stats)
        {
            stats.Level = Level;
            stats.Waiting = Count;
        }

    }
}
