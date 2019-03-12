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
        public virtual ScheduleTimerConfig JobPollSchedule { get; set; } = new ScheduleTimerConfig(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(10));
        /// <summary>
        /// Gets or sets a value indicating whether job poll is long running. The default is false.
        /// </summary>
        public virtual bool JobPollIsLongRunning { get; set; } = false;
    }
}
