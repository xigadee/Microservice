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

        public int Overage { get; private set; }

        public int Active { get { return mActive; } }

        public int Available
        {
            get
            {
                int result = BulkHead - mActive;
                return result > 0 ? result : 0;
            }
        }

        public void BulkHeadSet(int slotCount, int overage)
        {
            BulkHead = slotCount;
            Overage = overage;
        }

        public void Reserve(int count)
        {
            Interlocked.Add(ref mReserved, count);
        }

        public void Release(int count)
        {
            Interlocked.Add(ref mReserved, count*-1);
        }

        public int Reserved { get { return mReserved; } }

        public void Increment()
        {
            Interlocked.Increment(ref mActive);
            Interlocked.Increment(ref mCount);
        }

        public void Decrement(bool force)
        {
            Interlocked.Decrement(ref mActive);
            if (force)
                Interlocked.Increment(ref mKilled);
        }

        public string Debug
        {
            get
            {
                return $"Level={Level} Hits={mCount} Active={mActive} Available={Available} Bulkhead={BulkHead} Reserved={mReserved} Killed={mKilled}";
            }
        }
    }
}
