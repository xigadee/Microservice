using System;
using System.Collections.Generic;

namespace Xigadee
{
    /// <summary>
    /// This class the entity cache statistics
    /// </summary>
    public class EntityCacheStatistics:CommandStatistics
    {
        #region Declarations
        private StatsContainer mStatsCreateUpdate;
        private StatsContainer mStatsDelete;
        #endregion
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public EntityCacheStatistics():base()
        {
            mStatsCreateUpdate = new StatsContainer();
            mStatsDelete = new StatsContainer();
        }
        #endregion

        public long CurrentCached { get; set; }

        public long CurrentCachedEntities { get; set; }

        public long CurrentCacheLimit { get; set; }

        public long WaitCycles { get; set; }

        public long Removed { get; set; }

        public long Added { get; set; }

        public DateTime? LastScheduleTime{ get; set; }

        public bool TrackEvents { get; set; }

        public TimeSpan? DefaultExpiry { get; set; }

        public List<string> Current { get; set; }
    }
}
