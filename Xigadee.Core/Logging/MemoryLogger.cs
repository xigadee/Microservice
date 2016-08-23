using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This logger can be used for diagnotic purposes, and will hold a set of logger messages in memory, based on the 
    /// size parameter passed through in the constructor.
    /// </summary>
    public class MemoryLogger: ServiceBase<LoggingStatistics>, ILogger, IServiceOriginator
    {
        public MemoryLogger()
        {

        }

        /// <summary>
        /// The service originator.
        /// </summary>
        public string OriginatorId
        {
            get;set;
        }

        public async Task Log(LogEvent logEvent)
        {
        }

        protected override void StartInternal()
        {
        }

        protected override void StopInternal()
        {
        }
    }
}
