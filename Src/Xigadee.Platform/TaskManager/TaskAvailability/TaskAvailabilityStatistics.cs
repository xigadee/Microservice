using System;

namespace Xigadee
{
    /// <summary>
    /// This is the statistics class for Task Availability
    /// </summary>
    public class TaskAvailabilityStatistics: StatusBase
    {
        public int TasksMaxConcurrent { get; set; }

        public int SlotsAvailable { get; set; }

        public string[] Levels { get; set; }

        public int Killed { get; set; }

        public long KilledDidReturn { get; set; }

        public int Active { get; set; }


        public string[] Running { get; set; }

        public string[] Reservations { get; set; }

        /// <summary>
        /// This is the message logged for simple loggers.
        /// </summary>
        public override string Message
        {
            get
            {
                return $"MX={TasksMaxConcurrent}|A={Active}|AV={SlotsAvailable}|K={Killed}|KR={KilledDidReturn}";
            }
            set { }
        }
    }
}
