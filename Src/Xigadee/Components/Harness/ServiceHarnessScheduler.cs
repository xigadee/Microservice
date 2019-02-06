using System;
using System.Linq;
using System.Threading;

namespace Xigadee
{
    /// <summary>
    /// This is the test harness for the scheduler.
    /// </summary>
    public class ServiceHarnessScheduler : SchedulerContainer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceHarnessScheduler"/> class.
        /// </summary>
        public ServiceHarnessScheduler()
        {
            TaskSubmit = ProcessScheduleTask;
        }

        /// <summary>
        /// This method is used to manually process the schedule.
        /// </summary>
        /// <param name="tracker">The tracker.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">tracker - The tracker object was not of type schedule.</exception>
        protected virtual void ProcessScheduleTask(TaskTracker tracker)
        {
            if (tracker.Type != TaskTrackerType.Schedule)
                throw new ArgumentOutOfRangeException("tracker", "The tracker object was not of type schedule.");

            Exception failedEx = null;

            try
            {
                var token = new CancellationToken();
                tracker.Execute(token).Wait(token);
                
            }
            catch (Exception ex)
            {
                failedEx = ex;
            }
            finally
            {
                tracker.ExecuteComplete(tracker, failedEx != null, failedEx);
            }
        }

        /// <summary>
        /// Triggers execution of the schedule synchronously.
        /// </summary>
        /// <param name="Id">The schedule Id.</param>
        /// <returns>Returns true if the schedule is resolved and executed.</returns>
        public bool Execute(Guid Id)
        {
            var schedule = Items.FirstOrDefault((s) => s.Id == Id);

            if (schedule != null)
                Execute(schedule, true);

            return schedule != null;
        }
    }
}
