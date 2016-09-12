using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This is the base data collection statistics.
    /// </summary>
    public class DataCollectionStatistics: MessagingStatistics
    {

        public EventSourceContainerStatistics EventSource { get; set; }

        public LoggingStatistics Logging { get; set; }


    }
}
