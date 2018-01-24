using System;

namespace Xigadee
{
    public partial class Microservice
    {
        /// <summary>
        /// This policy hold miscellaneous settings not converted in the specific policy classes.
        /// </summary>
        public class Policy: PolicyBase
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
    }
}
