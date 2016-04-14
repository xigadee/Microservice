﻿using System;
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
        #endregion

        internal TaskManagerPrioritySettings(int level)
        {
            Level = level;
        }

        public int Level { get; }

        public int BulkHeadReservation { get; private set; }

        public int Active { get { return mActive; } }

        public int Available
        {
            get
            {
                int result = BulkHeadReservation - mActive;
                return result>0?result:0;
            }
        }

        public void BulkHeadReserve(int slotCount)
        {
            BulkHeadReservation = slotCount;
        }

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
                return $"Level={Level} Hits={mCount} Active={mActive} Available={Available} Reserved={BulkHeadReservation} Killed={mKilled}";
            }
        }
    }
}
