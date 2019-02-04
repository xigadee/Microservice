using System.Collections.Generic;
using System.Linq;

namespace Xigadee
{
    /// <summary>
    /// This class holds the queue tracker container statistics.
    /// </summary>
    public class QueueTrackerContainerStatistics: StatusBase
    {
        /// <summary>
        /// This is the list of queues and their statistics.
        /// </summary>
        public List<QueueTrackerStatistics> Queues { get; set; }
        /// <summary>
        /// Gets or sets the total of pending requests.
        /// </summary>
        public int Waiting { get; set; }

        /// <summary>
        /// This is the message logged for simple loggers.
        /// </summary>
        public override string Message
        {
            get
            {
                var result = string.Concat(Queues?.Select((q) => q.Message + "|")??new string[] {"|"});
                return result.Substring(0, result.Length -1);
            }
            set
            {
            }
        }
    }
}
