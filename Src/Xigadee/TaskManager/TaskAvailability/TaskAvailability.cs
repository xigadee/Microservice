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
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This is a transient class that shows the current task slot availability for a specific Task Manager priority level.
    /// </summary>
    public class TaskAvailability: StatisticsBase<TaskAvailabilityStatistics>, ITaskAvailability
    {
        #region Declarations
        /// <summary>
        /// This collection holds a reference to the active tasks to ensure that returning or killed processes are not counted twice.
        /// </summary>
        private ConcurrentDictionary<long, Guid> mActiveBag;
        /// <summary>
        /// This is the current count of the internal running tasks.
        /// </summary>
        private TaskManagerPrioritySettings mPriorityInternal;
        /// <summary>
        /// This is the status requests.
        /// </summary>
        private TaskManagerPrioritySettings[] mPriorityStatus;

        /// <summary>
        /// This is the incremental process counter.
        /// </summary>
        private long mProcessSlot = 0;

        /// <summary>
        /// This is the current count of the killed tasks that have been freed up because the task has failed to return.
        /// </summary>
        private int mTasksKilled = 0;
        /// <summary>
        /// This is the number of killed processes that did return.
        /// </summary>
        private long mTasksKilledDidReturn = 0;

        /// <summary>
        /// This is the current maximum amount of task that can be processed in parallel.
        /// </summary>
        private int mTasksMaxConcurrent;

        private int mReservedSlots;

        private ConcurrentDictionary<Guid, Reservation> mReservations;

        #endregion
        #region Constructor
        /// <summary>
        /// This constructor sets the allowed levels.
        /// </summary>
        /// <param name="levels">The level count.</param>
        /// <param name="maxConcurrent">The maximum number of allowed executing tasks.</param>
        public TaskAvailability(int levels, int maxConcurrent)
        {
            Levels = levels;
            mTasksMaxConcurrent = maxConcurrent;

            mPriorityInternal = new TaskManagerPrioritySettings(TaskTracker.PriorityInternal);

            mPriorityStatus = new TaskManagerPrioritySettings[levels];
            Enumerable.Range(0, levels).ForEach((l) => mPriorityStatus[l] = new TaskManagerPrioritySettings(l));

            mActiveBag = new ConcurrentDictionary<long, Guid>();
            mReservations = new ConcurrentDictionary<Guid, Reservation>();
            mReservedSlots = 0;
        }
        #endregion

        #region StatisticsRecalculate()
        /// <summary>
        /// This method calculates the statistics for the task manager availability.
        /// </summary>
        protected override void StatisticsRecalculate(TaskAvailabilityStatistics stats)
        {
            stats.TasksMaxConcurrent = mTasksMaxConcurrent;

            stats.Active = mActiveBag.Count;
            stats.SlotsAvailable = Count;

            stats.Killed = mTasksKilled;
            stats.KilledDidReturn = mTasksKilledDidReturn;

            if (mPriorityStatus != null)
                stats.Levels = mPriorityStatus
                    .Union(new TaskManagerPrioritySettings[] { mPriorityInternal })
                    .OrderByDescending((s) => s.Level)
                    .Select((s) => s.Debug)
                    .ToArray();

            stats.Reservations = mReservations.Select((r) => r.Value.Debug).ToArray();
        }
        #endregion

        #region BulkheadReserve(int level, int slotCount, int overage)
        /// <summary>
        /// This method sets the bulk head reservation for a particular level.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="slotCount"></param>
        /// <param name="overage"></param>
        /// <returns>Returns true if the reservation was successful.</returns>
        public bool BulkheadReserve(int level, int slotCount, int overage)
        {

            if (slotCount < 0)
                return false;

            if (level < LevelMin || level > LevelMax)
                return false;

            mPriorityStatus[level].BulkHeadSet(slotCount, overage);

            return true;
        }
        #endregion

        #region LevelMin
        /// <summary>
        /// This is the minimum task priority level.
        /// </summary>
        public int LevelMin { get { return 0; } }
        #endregion
        #region LevelMax
        /// <summary>
        /// This is the maximum task priority level.
        /// </summary>
        public int LevelMax { get { return Levels - 1; } }
        #endregion

        #region Levels
        /// <summary>
        /// This is the maximum number of priority levels
        /// </summary>
        public int Levels { get; }
        #endregion

        #region Level(int priority)
        /// <summary>
        /// Find any available slots for the level.
        /// </summary>
        /// <param name="priority">The priority.</param>
        /// <returns>returns the available slots.</returns>
        public int Level(int priority)
        {
            if (Count > 0)
                return Count;

            if (priority > LevelMax)
                priority = LevelMax;

            if (priority < LevelMin)
                priority = LevelMin;

            //OK, do we have any bulkhead reservation.
            do
            {
                int available = mPriorityStatus[priority].Available;

                if (available > 0)
                    return available;

                priority--;
            }
            while (priority >= LevelMin);

            return 0;
        }
        #endregion

        #region Increment(TaskTracker tracker)
        /// <summary>
        /// This method adds a tracker to the availability counters.
        /// </summary>
        /// <param name="tracker">The tracker to add.</param>
        /// <returns>Returns the tracker id.</returns>
        public long Increment(TaskTracker tracker)
        {
            if (tracker.ProcessSlot.HasValue && mActiveBag.ContainsKey(tracker.ProcessSlot.Value))
                throw new ArgumentOutOfRangeException($"The tracker has already been submitted.");

            tracker.ProcessSlot = Interlocked.Increment(ref mProcessSlot);
            if (!mActiveBag.TryAdd(tracker.ProcessSlot.Value, tracker.Id))
                throw new ArgumentOutOfRangeException($"The tracker has already been submitted.");

            if (tracker.IsInternal)
                mPriorityInternal.Increment();
            else
                mPriorityStatus[tracker.Priority.Value].Increment();

            return tracker.ProcessSlot.Value;
        } 
        #endregion
        #region Decrement(TaskTracker tracker, bool force = false)
        /// <summary>
        /// This method removes a tracker from the availability counters.
        /// </summary>
        /// <param name="tracker">The tracker to remove.</param>
        /// <param name="force">A flag indicating whether the tracker was forceably deleted.</param>
        public void Decrement(TaskTracker tracker, bool force = false)
        {
            if (!tracker.ProcessSlot.HasValue)
                throw new ArgumentOutOfRangeException($"The tracker does not have a process slot set.");

            Guid value;
            if (!mActiveBag.TryRemove(tracker.ProcessSlot.Value, out value))
                return;

            //Remove the internal task count.
            if (tracker.IsInternal)
                mPriorityInternal.Decrement(tracker.IsKilled);
            else
                mPriorityStatus[tracker.Priority.Value].Decrement(tracker.IsKilled);

            if (tracker.IsKilled)
            {
                Interlocked.Decrement(ref mTasksKilled);
                if (!force)
                    Interlocked.Increment(ref mTasksKilledDidReturn);
            }
        }
        #endregion


        public bool ReservationMake(Guid id, int priority, int taken)
        {
            try
            {
                Interlocked.Add(ref mReservedSlots, taken);
                mPriorityStatus[priority].Reserve(taken);
                return mReservations.TryAdd(id, new Reservation {Id=id, Priority = priority, Taken = taken });
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public bool ReservationRelease(Guid id)
        {
            try
            {
                Reservation reserve;
                if (mReservations.TryRemove(id, out reserve))
                {
                    Interlocked.Add(ref mReservedSlots, reserve.Taken * -1);
                    mPriorityStatus[reserve.Priority].Release(reserve.Taken);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public int ReservationsAvailable(int priority)
        {
            try
            {
                //We get the availability as we proceed though the client collection
                //This may change as slots are released from other processes.
                int slotsAvailable = Level(priority);
                if (slotsAvailable <= 0)
                    return 0;

                var ps = mPriorityStatus[priority];

                int actual = slotsAvailable - ps.Reserved + ps.Overage;

                return actual;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #region Count
        /// <summary>
        /// This figure is the number of remaining task slots available. Internal tasks are removed from the running tasks.
        /// </summary>
        public int Count
        {
            get
            {
                return mTasksMaxConcurrent - (mActiveBag.Count - mPriorityInternal.Active);
            }
        }
        #endregion

        #region Class -> Reservation
        /// <summary>
        /// This class holds a temporary slot reservation.
        /// </summary>
        [DebuggerDisplay("{Debug}")]
        private class Reservation
        {
            public Guid Id;

            public int Taken;

            public int Priority;

            public int Start { get; } = Environment.TickCount;

            public string Debug
            {
                get
                {
                    var extent = TimeSpan.FromMilliseconds(ConversionHelper.CalculateDelta(Environment.TickCount, Start));

                    return $"Id={Id} Priority={Priority} Reserved={Taken} Extent={StatsCounter.LargeTime(extent)}";
                }
            }
        } 
        #endregion
    }
}
