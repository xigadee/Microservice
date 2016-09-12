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
