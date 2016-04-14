#region using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
 
#endregion
namespace Xigadee
{
    /// <summary>
    /// This class holds the schedule jobs within the Microservice.
    /// </summary>
    public class SchedulerContainer : CollectionContainerBase<Schedule, SchedulerStatistics>, IScheduler, ITaskManagerProcess, IServiceLogger
    {
        #region Declarations
        /// <summary>
        /// This is the policy for the scheduler.
        /// </summary>
        protected readonly SchedulerPolicy mPolicy;
        #endregion
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="policy">The scheduler policy.</param>
        public SchedulerContainer(SchedulerPolicy policy = null)
            : base(null)
        {
            mPolicy = policy ?? new SchedulerPolicy();
        }
        #endregion

        #region Register...
        /// <summary>
        /// This method registers a schedule.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="frequency"></param>
        /// <param name="name"></param>
        /// <param name="initialWait"></param>
        /// <param name="initialTime"></param>
        /// <param name="shouldPoll"></param>
        /// <param name="isInternal"></param>
        /// <returns>Returns the new schedule after it has been added to the collection.</returns>
        public Schedule Register(Func<Schedule, CancellationToken, Task> action
            , TimeSpan? frequency
            , string name = null
            , TimeSpan? initialWait = null
            , DateTime? initialTime = null
            , bool shouldPoll = true
            , bool isInternal = false
            )
        {
            var schedule = new Schedule(action, name)
            {
                Frequency = frequency,
                InitialWait = initialWait,
                InitialTime = initialTime,
                ShouldPoll = shouldPoll,
                IsInternal = isInternal
            };

            return Register(schedule);
        }
        /// <summary>
        /// This method registers a schedule.
        /// </summary>
        /// <param name="schedule">The schedule object.</param>
        /// <returns>Returns the schedule after it has been added to the schedule collection.</returns>
        public Schedule Register(Schedule schedule)
        {
            schedule.Recalculate();

            Add(schedule);

            return schedule;
        } 
        #endregion

        #region Unregister(Schedule schedule)
        /// <summary>
        /// This removes a schedule from the collection.
        /// </summary>
        /// <param name="schedule">The schedule to remove.</param>
        /// <returns>Returnes true if the schedule is removed successfully.</returns>
        public bool Unregister(Schedule schedule)
        {
            schedule.ShouldPoll = false;
            return base.Remove(schedule);
        } 

        #endregion

        protected override void StatisticsRecalculate()
        {
            base.StatisticsRecalculate();

            mStatistics.DefaultPollInMs = mPolicy.DefaultPollInMs;
            mStatistics.Schedules = Items.ToList();
        }

        //#region SchedulerStop()
        ///// <summary>
        ///// This method stops all current schedule.
        ///// </summary>
        //public void SchedulerStop()
        //{
        //    foreach (var schedule in mScheduler.Items)
        //    {
        //        schedule.ShouldPoll = false;
        //    }
        //}
        //#endregion

        #region Logger
        /// <summary>
        /// This is the system wide logger.
        /// </summary>
        public ILoggerExtended Logger
        {
            get; set;
        }
        #endregion
        #region Submit
        /// <summary>
        /// This is the action path back to the TaskManager.
        /// </summary>
        public Action<TaskTracker> Submit
        {
            get; set;
        }
        #endregion
        #region CanProcess()
        /// <summary>
        /// Check whether we have any schedules.
        /// </summary>
        /// <returns>Returns true of we can process.</returns>
        public bool CanProcess()
        {
            return Status == ServiceStatus.Running && Count > 0 && Submit != null;
        }
        #endregion

        #region Process(TaskManagerAvailability availability)
        /// <summary>
        /// This method processes any outstanding schedules and created a new task.
        /// </summary>
        public void Process(ITaskAvailability availability)
        {
            foreach (Schedule schedule in Items.Where((c) => c.ShouldPoll))
            {
                if (schedule.Active)
                {
                    schedule.PollSkip();
                    continue;
                }

                schedule.Start();

                TaskTracker tracker = TaskTrackerCreate(schedule);

                tracker.Execute = async (token) =>
                {
                    try
                    {
                        await schedule.Execute(token);
                    }
                    catch (Exception ex)
                    {
                        Logger?.LogException(string.Format("Schedule failed: {0}", schedule.Name), ex);
                    }
                };

                tracker.ExecuteComplete = ScheduleComplete;

                Submit(tracker);
            }
        }
        #endregion

        #region TaskTrackerCreate(Schedule schedule)
        /// <summary>
        /// This private method builds the payload consistently for the incoming payload.
        /// </summary>
        /// <param name="schedule">The schedule to add to a tracker.</param>
        /// <returns>Returns a tracker of type payload.</returns>
        private TaskTracker TaskTrackerCreate(Schedule schedule)
        {
            TaskTracker tracker = new TaskTracker(TaskTrackerType.Schedule, null);
            tracker.IsLongRunning = schedule.IsLongRunning;

            if (schedule.IsInternal)
                tracker.Priority = TaskTracker.PriorityInternal;
            else
                tracker.Priority = 2;

            tracker.Context = schedule;
            tracker.Name = schedule.Name;

            return tracker;
        }
        #endregion
        #region ScheduleComplete(Task task, TaskTracker tracker)
        /// <summary>
        /// This method is used to complete a schedule request and to 
        /// recalculate the next schedule.
        /// </summary>
        /// <param name="tracker">The tracker object.</param>
        private void ScheduleComplete(TaskTracker tracker, bool failed, Exception ex)
        {
            var schedule = tracker.Context as Schedule;

            schedule.Complete(!failed, isException: failed, lastEx: ex, exceptionTime: DateTime.UtcNow);
        }
        #endregion

    }
}
