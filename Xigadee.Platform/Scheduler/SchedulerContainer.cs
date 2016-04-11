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
    public class SchedulerContainer : CollectionContainerBase<Schedule, SchedulerStatistics>, IScheduler
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
        /// <param name="poll">The poll interval in milliseconds. The default is 1 second, 1000 ms.</param>
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
    }
}
