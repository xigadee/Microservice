using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This policy hold miscellaneous settings not convered in the specific policy classes.
    /// </summary>
    public class MicroservicePolicy:PolicyBase
    {
        /// <summary>
        /// This is the frquency that the Microservice status is compiled and logged to the data collector.
        /// </summary>
        public TimeSpan FrequencyStatisticsGeneration { get; set; } = TimeSpan.FromSeconds(15);
        /// <summary>
        /// This is the frquency for calculating time outs for overrunning processes.
        /// </summary>
        public TimeSpan FrequencyTasksTimeout { get; set; } = TimeSpan.FromMinutes(1);
        /// <summary>
        /// This is is the frequency for calling the data collector to flush any long running statistics.
        /// </summary>
        public TimeSpan FrequencyDataCollectionFlush { get; set; } = TimeSpan.FromMinutes(15);
        /// <summary>
        /// This is the autotune poll frquency. Leave this value null to disable the poll.
        /// </summary>
        public TimeSpan? FrequencyAutotune { get; set; } = null;


        /// <summary>
        /// This is the maximum transit count that a message can take before it errors out. 
        /// This is to stop unexpected loops causing the system to crash.
        /// </summary>
        public int DispatcherTransitCountMax { get; set; } = 20;

        /// <summary>
        /// This setting sets the dispatcher action if an incoming message cannnot be resolved.
        /// </summary>
        public DispatcherUnhandledAction DispatcherUnresolvedRequestMode { get; set; } = DispatcherUnhandledAction.AttemptResponseFailMessage;

        /// <summary>
        /// This setting sets the dispatcher action if an outgoing channel cannnot be resolved.
        /// </summary>
        public DispatcherUnhandledAction DispatcherInvalidChannelMode { get; set; } = DispatcherUnhandledAction.AttemptResponseFailMessage;

    }

    /// <summary>
    /// This enum specifies the behaviour to take when receiving an unhandled message.
    /// </summary>
    public enum DispatcherUnhandledAction
    {
        /// <summary>
        /// An exception will be raised and logged.
        /// </summary>
        Exception,
        /// <summary>
        /// The message will be logged and ignored.
        /// </summary>
        Ignore,
        /// <summary>
        /// The dispatcher will attempt to send a 500 error response to the originating sender.
        /// </summary>
        AttemptResponseFailMessage
    }

}
