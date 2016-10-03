using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public class MicroservicePolicy:PolicyBase
    {
        /// <summary>
        /// This is the frquency that the Microservice status is compiled and logged to the data collector.
        /// </summary>
        public TimeSpan FrequencyStatisticsGeneration { get; set; } = TimeSpan.FromSeconds(15);

        public TimeSpan FrequencyTasksTimeout { get; set; } = TimeSpan.FromMinutes(1);

        public TimeSpan FrequencyDataCollectionFlush { get; set; } = TimeSpan.FromMinutes(15);

        public TimeSpan? FrequencyAutotune { get; set; } = null;


        /// <summary>
        /// This is the maximum transit count that a message can take before it errors out. 
        /// This is to stop unexpected loops causing the system to crash.
        /// </summary>
        public int DispatcherTransitCountMax { get; set; } = 20;

        /// <summary>
        /// This flag ignores unhandled messages and marks then as complete from the queue.
        /// </summary>
        public bool DispatcherUnhandledMessagesIgnore { get; set; } = true;
    }
}
