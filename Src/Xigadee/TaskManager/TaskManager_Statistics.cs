using System.Threading;

namespace Xigadee
{
    public partial class TaskManager
    {
        /// <summary>
        /// This class holds the current Task Manager statistics.
        /// </summary>
        /// <seealso cref="Xigadee.MessagingStatistics" />
        public class Statistics: MessagingStatistics
        {
            private long mTimeouts;

            /// <summary>
            /// Initializes a new instance of the <see cref="TaskManagerStatistics"/> class.
            /// </summary>
            public Statistics()
            {
            }


            public void TimeoutRegister(long count)
            {
                Interlocked.Add(ref mTimeouts, count);
            }

            public ICpuStats Cpu { get; set; }

            public TaskAvailabilityStatistics Availability { get; set; }

            public QueueTrackerContainerStatistics Queues { get; set; }

            public string[] Running { get; set; }

            /// <summary>
            /// Gets or sets the current active task count.
            /// </summary>
            public int TaskCount { get; set; }

            public bool InternalQueueActive { get; set; }

            public int? InternalQueueLength { get; set; }

            /// <summary>
            /// This is the message logged for simple loggers.
            /// </summary>
            public override string Message
            {
                get
                {
                    return $"A=[{Availability.Message}];Q=[{Queues.Message}];T={TaskCount}";
                }
                set
                {
                }
            }
        }
    }
}
