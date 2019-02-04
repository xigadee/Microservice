using System;
using System.Diagnostics;
namespace Xigadee
{

    /// <summary>
    /// This class holds the current status of the Microservice container.
    /// </summary>
    [DebuggerDisplay("{Name}-{Status} @ {LogTime}")]
    public class MicroserviceStatistics : MessagingStatistics, ILogStoreName
    {
        #region Constructor
        /// <summary>
        /// This is the statistics default constructor.
        /// </summary>
        public MicroserviceStatistics() : base()
        {

        }
        #endregion

        #region Name
        /// <summary>
        /// This override places the name at the top of the JSON
        /// </summary>
        public override string Name
        {
            get
            {
                return Id?.Name;
            }

            set
            {
            }
        }
        #endregion

        /// <summary>
        /// This is the Microservice identifier collection.
        /// </summary>
        public MicroserviceId Id { get; set; }

        /// <summary>
        /// This is the current status of the service.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// This is the last time that the statistics were updated.
        /// </summary>
        public DateTime LogTime { get; set; }

        /// <summary>
        /// This is the service uptime.
        /// </summary>
        public string Uptime
        {
            get
            {
                var span = LogTime - Id.StartTime;
                return StatsCounter.LargeTime(span);
            }
        }

        /// <summary>
        /// This is the task manager statistics.
        /// </summary>
        public TaskManagerStatistics Tasks { get; set; }
        /// <summary>
        /// This is a list of the handlers active on the system and their status.
        /// </summary>
        public CommunicationContainerStatistics Communication { get; set; }
        /// <summary>
        /// This is the command container statistics/
        /// </summary>
        public CommandContainerStatistics Commands { get; set; }
        /// <summary>
        /// The resource statistics.
        /// </summary>
        public ResourceContainerStatistics Resources { get; set; }
        /// <summary>
        /// The service handler statistics.
        /// </summary>
        public ServiceHandlerContainerStatistics ServiceHandlers { get; set; }
        /// <summary>
        /// The scheduler statistics.
        /// </summary>
        public SchedulerContainerStatistics Scheduler { get; set; }
        /// <summary>
        /// The data collection statistics. These include the logger, event source and telemetry statistics.
        /// </summary>
        public DataCollectionContainerStatistics DataCollection { get; set; }

        #region StorageId
        /// <summary>
        /// This is the Id used in the undelying storage.
        /// </summary>
        public string StorageId
        {
            get
            {
                return string.Format("{0}_{3:yyyyMMddHHmmssFFF}_{1}_{2}", Id.Name, Id.MachineName, Id.ServiceId, LogTime);
            }
        }
        #endregion
    }


}
