using System.Collections.Generic;

namespace Xigadee
{
    /// <summary>
    /// This class contains the statistics for the scheduler collection.
    /// </summary>
    /// <seealso cref="Xigadee.CollectionStatistics" />
    public class SchedulerContainerStatistics : CollectionStatistics
    {
        #region Name
        /// <summary>
        /// This is the service name.
        /// </summary>
        public override string Name
        {
            get
            {
                return base.Name;
            }

            set
            {
                base.Name = value;
            }
        }
        #endregion
        #region ItemCount
        /// <summary>
        /// The item count.
        /// </summary>
        public override int ItemCount
        {
            get
            {
                return base.ItemCount;
            }

            set
            {
                base.ItemCount = value;
            }
        }
        #endregion
        #region DefaultPollInMs
        /// <summary>
        /// Displays the default poll time in milliseconds.
        /// </summary>
        public int DefaultPollInMs { get; set; }
        #endregion

        /// <summary>
        /// Gets or sets the schedule statistics.
        /// </summary>
        public List<Schedule> Schedules { get; set; }
    }
}
