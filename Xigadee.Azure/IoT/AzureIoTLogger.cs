using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This class sends data to the IoT stream for analysis.
    /// </summary>
    public class AzureIoTLogger: ServiceBase<LoggingStatistics>, IServiceLogger, IRequireSharedServices, ILogger
    {
        public AzureIoTLogger()
        {

        }

        public ILoggerExtended Logger
        {
            get;set;
        }

        public ISharedService SharedServices
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
