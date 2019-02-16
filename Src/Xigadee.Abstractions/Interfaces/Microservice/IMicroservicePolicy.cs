using System;
using System.Collections.Generic;

namespace Xigadee
{
    /// <summary>
    /// This interface lists the policy options for the Microservice.
    /// </summary>
    public interface IMicroservicePolicy
    {
        /// <summary>
        /// Gets the Microservice default policy.
        /// </summary>
        MicroservicePolicy Microservice { get; }
        /// <summary>
        /// Gets the task manager policy.
        /// </summary>
        TaskManagerPolicy TaskManager { get; }
        /// <summary>
        /// Gets the resource monitor policy.
        /// </summary>
        ResourceContainerPolicy ResourceMonitor { get; }
        /// <summary>
        /// Gets the command container policy.
        /// </summary>
        CommandContainerPolicy CommandContainer { get; }
        /// <summary>
        /// Gets the communication policy.
        /// </summary>
        CommunicationContainerPolicy Communication { get; }
        /// <summary>
        /// Gets the scheduler policy.
        /// </summary>
        SchedulerContainerPolicy Scheduler { get; }
        /// <summary>
        /// Gets the data collection policy.
        /// </summary>
        DataCollectionContainerPolicy DataCollection { get; }
        /// <summary>
        /// Gets the service handler policy.
        /// </summary>
        ServiceHandlerContainerPolicy ServiceHandlers { get; }
    }

    #region MicroservicePolicy
    /// <summary>
    /// This policy hold miscellaneous settings not converted in the specific policy classes.
    /// </summary>
    public class MicroservicePolicy : PolicyBase
    {
        /// <summary>
        /// This is the frequency that the Microservice status is compiled and logged to the data collector.
        /// </summary>
        public TimeSpan FrequencyStatisticsGeneration { get; set; } = TimeSpan.FromSeconds(15);
        /// <summary>
        /// This is the frequency for calculating time outs for overrunning processes.
        /// </summary>
        public TimeSpan FrequencyTasksTimeout { get; set; } = TimeSpan.FromMinutes(1);
        /// <summary>
        /// This is the frequency for calling the data collector to flush any long running statistics.
        /// </summary>
        public TimeSpan FrequencyDataCollectionFlush { get; set; } = TimeSpan.FromMinutes(15);
        /// <summary>
        /// This is the autotune poll frequency. Leave this value null to disable the poll.
        /// </summary>
        public TimeSpan? FrequencyAutotune { get; set; } = null;


        /// <summary>
        /// This is the maximum transit count that a message can take before it errors out. 
        /// This is to stop unexpected loops causing the system to crash.
        /// </summary>
        public int DispatcherTransitCountMax { get; set; } = 20;

        /// <summary>
        /// This setting sets the dispatcher action if an incoming message cannot be resolved.
        /// </summary>
        public DispatcherUnhandledAction DispatcherUnresolvedRequestMode { get; set; } = DispatcherUnhandledAction.AttemptResponseFailMessage;

        /// <summary>
        /// This setting sets the dispatcher action if an outgoing channel cannot be resolved.
        /// </summary>
        public DispatcherUnhandledAction DispatcherInvalidChannelMode { get; set; } = DispatcherUnhandledAction.AttemptResponseFailMessage;

    } 
    #endregion

    #region TaskManagerPolicy
    /// <summary>
    /// This is the policy object for the TaskManager, that determines how it operates.
    /// </summary>
    public class TaskManagerPolicy : PolicyBase
    {
        /// <summary>
        /// Gets or sets a value indicating whether the TransmissionPayload trace flag should be set to true.
        /// </summary>
        public bool TransmissionPayloadTraceEnabled { get; set; }

        /// <summary>
        /// This is the internal array containing the priority levels.
        /// </summary>
        protected PriorityLevelReservation[] mPriorityLevels = null;

        private object syncLock = new object();

        #region Constructor
        /// <summary>
        /// This constructor sets the default bulkhead configuration.
        /// </summary>
        public TaskManagerPolicy()
        {
            //Set the default bulk head level
            BulkheadReserve(0, 0, 0);
            BulkheadReserve(1, 8, 8);
            BulkheadReserve(2, 2, 2);
            BulkheadReserve(3, 1, 2);
        }
        #endregion
        #region BulkheadReserve(int level, int slotCount, int overage =0)
        /// <summary>
        /// This method can be called to set a bulkhead reservation.
        /// </summary>
        /// <param name="level">The bulkhead level.</param>
        /// <param name="slotCount">The number of slots reserved.</param>
        /// <param name="overage">The permitted overage.</param>
        public void BulkheadReserve(int level, int slotCount, int overage = 0)
        {
            if (level < 0)
                throw new ArgumentOutOfRangeException("level must be a positive integer");

            var res = new PriorityLevelReservation { Level = level, SlotCount = slotCount, Overage = overage };

            lock (syncLock)
            {
                if (mPriorityLevels != null && level <= (PriorityLevels - 1))
                {
                    mPriorityLevels[level] = res;
                    return;
                }

                var pLevel = new PriorityLevelReservation[level + 1];

                if (mPriorityLevels != null)
                    Array.Copy(mPriorityLevels, pLevel, mPriorityLevels.Length);

                pLevel[level] = res;

                mPriorityLevels = pLevel;
            }
        }
        #endregion

        /// <summary>
        /// This is the number of priority levels supported in the Task Manager.
        /// </summary>
        public int PriorityLevels { get { return (mPriorityLevels?.Length ?? 0); } }

        /// <summary>
        /// This is the list of priority level reservations.
        /// </summary>
        public IEnumerable<PriorityLevelReservation> PriorityLevelReservations
        {
            get
            {
                return mPriorityLevels;
            }
        }

        /// <summary>
        /// This is the time that a process is marked as killed after it has been marked as cancelled.
        /// </summary>
        public TimeSpan? ProcessKillOverrunGracePeriod { get; set; } = TimeSpan.FromSeconds(15);
        /// <summary>
        /// This is maximum target percentage usage limit.
        /// </summary>
        public int ProcessorTargetLevelPercentage { get; set; }
        /// <summary>
        /// This is the maximum number overload processes permitted.
        /// </summary>
        public int OverloadProcessLimitMax { get; set; }
        /// <summary>
        /// This is the minimum number of overload processors available.
        /// </summary>
        public int OverloadProcessLimitMin { get; set; }
        /// <summary>
        /// This is the maximum time that an overload process task can run.
        /// </summary>
        public int OverloadProcessTimeInMs { get; set; }
        /// <summary>
        /// This is the maximum number of concurrent requests.
        /// </summary>
        public int ConcurrentRequestsMax { get; set; } = Environment.ProcessorCount * 16;
        /// <summary>
        /// This is the minimum number of concurrent requests.
        /// </summary>
        public int ConcurrentRequestsMin { get; set; } = Environment.ProcessorCount * 2;

        /// <summary>
        /// This is the default time that the process loop should pause before another cycle if it is not triggered
        /// by a task submission or completion. The default is 200 ms.
        /// </summary>
        public int LoopPauseTimeInMs { get; set; } = 50;
        /// <summary>
        /// This override specifies that internal jobs do not get added to the internal queue, but get executed directly.
        /// </summary>
        public bool ExecuteInternalDirect { get; set; } = true;

    }

    /// <summary>
    /// This is the reservation settings for the particular priority level.
    /// </summary>
    public class PriorityLevelReservation
    {
        /// <summary>
        /// This is the priority level.
        /// </summary>
        public int Level { get; set; }
        /// <summary>
        /// This is the slot count.
        /// </summary>
        public int SlotCount { get; set; }
        /// <summary>
        /// This is the overage limit.
        /// </summary>
        public int Overage { get; set; }
    } 
    #endregion

    #region ResourceContainerPolicy
    /// <summary>
    /// This policy is used by the ResourceContainer class
    /// </summary>
    public class ResourceContainerPolicy : PolicyBase
    {
    }
    #endregion

    #region CommandContainerPolicy
    /// <summary>
    /// This is the specific policy for the command container.
    /// </summary>
    public class CommandContainerPolicy : PolicyBase
    {
    }
    #endregion

    #region CommunicationContainerPolicy
    /// <summary>
    /// This is the policy that defines how the communication component operates.
    /// </summary>
    public class CommunicationContainerPolicy : PolicyBase
    {
        /// <summary>
        /// Set outgoing routing information to lower-case. This is important as messaging protocols such as
        /// Service Bus can be case sensitive when running subscription filters.
        /// </summary>
        public bool ServiceMessageHeaderConvertToLowercase { get; set; } = true;
        /// <summary>
        /// Gets or sets a value indicating whether the TransmissionPayload trace flag should be set to true.
        /// </summary>
        public bool TransmissionPayloadTraceEnabled { get; set; }
        /// <summary>
        /// This is the default time that a process submitted from a listener can execute for. The default value is 30 seconds.
        /// </summary>
        public TimeSpan? ListenerRequestTimespan { get; set; } = null;
        /// <summary>
        /// This is the default boundary logging status. When the specific status is not set, this value 
        /// will be used. The default setting is false.
        /// </summary>
        public bool BoundaryLoggingActiveDefault { get; set; }
        /// <summary>
        /// This property specifies that channel can be created automatically if they do not exist.
        /// If this is set to false, an error will be generated when a message is sent to a channel
        /// that has not been explicitly created.
        /// </summary>
        public bool AutoCreateChannels { get; set; } = true;
        /// <summary>
        /// This is the default algorithm used to assign poll cycles to the various listeners.
        /// </summary>
        public virtual IListenerClientPollAlgorithm ListenerClientPollAlgorithm { get; set; } 
    }
    #endregion

    #region SchedulerContainerPolicy
    /// <summary>
    /// This is the scheduler policy.
    /// </summary>
    /// <seealso cref="Xigadee.PolicyBase" />
    public class SchedulerContainerPolicy : PolicyBase
    {
        /// <summary>
        /// Gets or sets the default poll in milliseconds.
        /// </summary>
        public virtual int DefaultPollInMs { get; set; } = 100;

        /// <summary>
        /// Gets or sets the default task priority that schedules are set for the Task Manager,.
        /// </summary>
        public int DefaultTaskPriority { get; set; } = 2;
    }
    #endregion

    #region DataCollectionContainerPolicy
    /// <summary>
    /// This class holds the data collection policy.
    /// </summary>
    public class DataCollectionContainerPolicy : PolicyBase
    {
        /// <summary>
        /// This is the maximum time that an overload process should run.
        /// </summary>
        public int OverloadProcessTimeInMs { get; set; } = 10000; //10s
                                                                  /// <summary>
                                                                  /// This is the maximum number of overload tasks that should be run concurrently.
                                                                  /// </summary>
        public int OverloadMaxTasks { get; set; } = 2;
        /// <summary>
        /// This is the threshold at which point the overload tasks will be triggered.
        /// </summary>
        public int? OverloadThreshold { get; set; } = 1000;
        /// <summary>
        /// This is the number of retry attempts to be made if the write fails.
        /// </summary>
        public int RetryLimit { get; set; } = 0;

        /// <summary>
        /// This is the event source retry limit.
        /// </summary>
        public int EventSourceRetryLimit { get; set; } = 3;
    }
    #endregion

    #region ServiceHandlerContainerPolicy
    /// <summary>
    /// This is the policy container.
    /// </summary>
    public class ServiceHandlerContainerPolicy : PolicyBase
    {
    }
    #endregion
}
