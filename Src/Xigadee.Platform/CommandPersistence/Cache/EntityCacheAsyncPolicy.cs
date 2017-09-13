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

namespace Xigadee
{
    /// <summary>
    /// This is the policy extension class for the entity cache component.
    /// </summary>
    /// <seealso cref="Xigadee.CommandPolicy" />
    public class EntityCacheAsyncPolicy:CommandPolicy
    {
        /// <summary>
        /// Gets or sets a value indicating whether the class should monitor for changes to entities within the system.
        /// </summary>
        public bool EntityChangeTrackEvents { get; set; }
        /// <summary>
        /// Gets or sets the entity change events channel. This is the channel used to monitor entity changes within the system.
        /// </summary>
        public string EntityChangeEventsChannel { get; set; }
        /// <summary>
        /// Gets or sets the entity cache limit. The default is 200000.
        /// </summary>
        public int EntityCacheLimit { get; set; } = 200000;
        /// <summary>
        /// Gets or sets the entity default TTL. The default is 2 days.
        /// </summary>
        public TimeSpan EntityDefaultTTL { get; set; } = TimeSpan.FromDays(2);

        /// <summary>
        /// Gets or sets the entity poll schedule.
        /// </summary>
        public virtual ScheduleTimerConfig JobPollSchedule { get; set; } = new ScheduleTimerConfig();
        /// <summary>
        /// Gets or sets a value indicating whether job poll is long running. The default is false.
        /// </summary>
        public virtual bool JobPollIsLongRunning { get; set; } = false;
    }
}
