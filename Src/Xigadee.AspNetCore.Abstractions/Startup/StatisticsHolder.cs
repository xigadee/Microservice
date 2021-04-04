using System;

namespace Xigadee
{
    /// <summary>
    /// This holder is registered with the IOC container and contains the current statistics.
    /// </summary>
    public class StatisticsHolder
    {
        /// <summary>
        /// This is the current statistics
        /// </summary>
        public MicroserviceStatistics Statistics { get; private set; }
        /// <summary>
        /// This is the last update time.
        /// </summary>
        public DateTime? LastUpdate { get; private set; }

        /// <summary>
        /// This method sets the statistics.
        /// </summary>
        /// <param name="statistics">The stats.</param>
        public void Load(MicroserviceStatistics statistics)
        {
            Statistics = statistics;
            LastUpdate = DateTime.UtcNow;
        }
    }
}
