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
    /// This class holds the specific statistics for the Queue Tracker.
    /// </summary>
    public class QueueTrackerStatistics: MessagingStatistics
    {
        /// <summary>
        /// This is the high water mark for the queue.
        /// </summary>
        public int MaxWaiting { get; protected set; } = -1;

        /// <summary>
        /// This is the time stamp when the high water mark was reached for the queue.
        /// </summary>
        public DateTime? MaxWaitingTime { get; protected set; }

        #region Current
        /// <summary>
        /// This is the number of messages currently in the queue.
        /// </summary>
        public int Waiting
        {
            get;set;
        }
        #endregion

        /// <summary>
        /// This method sets the high water mark for the queue statistics.
        /// </summary>
        /// <param name="count">The current count.</param>
        public void WaitingSet(int count)
        {
            if (count == 0 || count <= MaxWaiting)
                return;

            MaxWaiting = count;
            MaxWaitingTime = DateTime.UtcNow;
        }
    }
}
