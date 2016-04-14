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
    /// This is a transient class that shows the current task slot availability.
    /// </summary>
    public class TaskAvailability: ITaskAvailability
    {
        #region Declarations
        /// <summary>
        /// This is the current count of the internal running tasks.
        /// </summary>
        private TaskManagerPrioritySettings mPriorityInternal;
        /// <summary>
        /// This is the status requests.
        /// </summary>
        private TaskManagerPrioritySettings[] mPriorityStatus;

        private int mActive = 0;

        private long mProcessSlot = 0;

        /// <summary>
        /// This is the current count of the killed tasks that have been freed up because the task has failed to return.
        /// </summary>
        private int mTasksKilled = 0;

        private long mTasksKilledDidReturn = 0;
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
            TasksMaxConcurrent = maxConcurrent;

            mPriorityInternal = new TaskManagerPrioritySettings(TaskTracker.PriorityInternal);

            mPriorityStatus = new TaskManagerPrioritySettings[levels];
            Enumerable.Range(0, levels).ForEach((l) => mPriorityStatus[l] = new TaskManagerPrioritySettings(l));
        }
        #endregion

        #region BulkheadReserve(int level, int slotCount)
        /// <summary>
        /// This method sets the bulk head reservation for a particular level.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="slotCount"></param>
        /// <returns></returns>
        public bool BulkheadReserve(int level, int slotCount)
        {

            if (slotCount < 0)
                return false;

            if (level < LevelMin || level > LevelMax)
                return false;

            mPriorityStatus[level].BulkHeadReservation = slotCount;

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

        public int Level(int priority)
        {
            return 10;
        }

        public int TasksMaxConcurrent { get; }

        public long Increment(TaskTracker tracker)
        {
            Interlocked.Increment(ref mActive);

            if (tracker.IsInternal)
                Interlocked.Increment(ref mPriorityInternal.Active);
            else
                Interlocked.Increment(ref mPriorityStatus[tracker.Priority.Value].Active);

            tracker.ProcessSlot = Interlocked.Increment(ref mProcessSlot);

            return tracker.ProcessSlot.Value;
        }

        public void Decrement(TaskTracker tracker, bool force = false)
        {
            Interlocked.Decrement(ref mActive);

            //Remove the internal task count.
            if (tracker.IsInternal)
                Interlocked.Decrement(ref mPriorityInternal.Active);
            else
                Interlocked.Decrement(ref mPriorityStatus[tracker.Priority.Value].Active);

            if (tracker.IsKilled)
            {
                Interlocked.Decrement(ref mTasksKilled);
                if (!force)
                    Interlocked.Increment(ref mTasksKilledDidReturn);
            }
        }

        #region Available
        /// <summary>
        /// This figure is the number of remaining task slots available. Internal tasks are removed from the running tasks.
        /// </summary>
        public int Count
        {
            get
            {
                return TasksMaxConcurrent - (mActive - mPriorityInternal.Active - mTasksKilled);
            }
        }
        #endregion


    }
}
