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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This class is used to hold the priority settings.
    /// </summary>
    [DebuggerDisplay("{Debug}")]
    public class TaskManagerPrioritySettings
    {
        #region Declarations
        private long mCount;

        private int mActive;

        private int mKilled;

        private int mReserved;
        #endregion

        internal TaskManagerPrioritySettings(int level)
        {
            Level = level;
        }

        /// <summary>
        /// This is the priority level.
        /// </summary>
        public int Level { get; }

        /// <summary>
        /// This is the defined number of slots reserved for a particular priority.
        /// </summary>
        public int BulkHead { get; private set; }
        /// <summary>
        /// This is the allowed overage for the priority level.
        /// </summary>
        public int Overage { get; private set; }
        /// <summary>
        /// This is the number of active slots for the specific priority.
        /// </summary>
        public int Active { get { return mActive; } }
        /// <summary>
        /// This is the number of available slots for the priority level.
        /// </summary>
        public int Available
        {
            get
            {
                int result = BulkHead - mActive;
                return result > 0 ? result : 0;
            }
        }
        /// <summary>
        /// This method sets the bulkhead slot count and allowed overage settings.
        /// </summary>
        /// <param name="slotCount">The permitted active reserved slot count.</param>
        /// <param name="overage">The allowed overage about the active slot count.</param>
        public void BulkHeadSet(int slotCount, int overage)
        {
            BulkHead = slotCount;
            Overage = overage;
        }
        /// <summary>
        /// This method reserves a number of permitted slots.
        /// </summary>
        /// <param name="count">The number of slots to reserve.</param>
        public void Reserve(int count)
        {
            Interlocked.Add(ref mReserved, count);
        }
        /// <summary>
        /// This method releases the reserved slots.
        /// </summary>
        /// <param name="count">The number of unused slots.</param>
        public void Release(int count)
        {
            Interlocked.Add(ref mReserved, count*-1);
        }
        /// <summary>
        /// This is the number of current reserved slots during a poll.
        /// </summary>
        public int Reserved { get { return mReserved; } }

        /// <summary>
        /// This method increments the active slots.
        /// </summary>
        public void Increment()
        {
            Interlocked.Increment(ref mActive);
            Interlocked.Increment(ref mCount);
        }
        /// <summary>
        /// This method decrements the active slots.
        /// </summary>
        /// <param name="force">If this is set to true, the number of killed processes are incremented.</param>
        public void Decrement(bool force)
        {
            Interlocked.Decrement(ref mActive);
            if (force)
                Interlocked.Increment(ref mKilled);
        }

        /// <summary>
        /// This is a debug string used for reporting status to the statistics class.
        /// </summary>
        public string Debug
        {
            get
            {
                return $"Level={Level} Hits={mCount} Active={mActive} Available={Available} Bulkhead={BulkHead} Reserved={mReserved} Killed={mKilled}";
            }
        }
    }
}
