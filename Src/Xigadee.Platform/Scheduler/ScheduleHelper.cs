using System;

namespace Xigadee
{
    /// <summary>
    /// This static class is used to set the context schedule.
    /// </summary>
    public static class ScheduleHelper
    {
        /// <summary>
        /// Sets the next poll time.
        /// </summary>
        /// <param name="ctx">The scheduler context.</param>
        /// <param name="nextUTCTime">The next UTC time to run the schedule.</param>
        public static void SetNextPollTime(this CommandScheduleInlineContext ctx, DateTime nextUTCTime)
        {
            ctx.Schedule.UTCPollTime = nextUTCTime;
        }

        /// <summary>
        /// Sets the next poll time.
        /// </summary>
        /// <param name="ctx">The scheduler context.</param>
        /// <param name="timeSpan">The time span before the schedule should run again.</param>
        public static void SetNextPollTime(this CommandScheduleInlineContext ctx, TimeSpan timeSpan)
        {
            ctx.Schedule.UTCPollTime = DateTime.UtcNow.Add(timeSpan);
        }
    }
}
