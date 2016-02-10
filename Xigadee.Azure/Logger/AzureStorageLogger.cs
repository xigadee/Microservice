using Microsoft.WindowsAzure.Storage.Auth;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public class AzureStorageLogger : AzureStorageLoggingBase<LogEvent>, ILogger
    {
        public AzureStorageLogger(StorageCredentials credentials, string serviceName, string containerName = "log"
            , ResourceProfile resourceProfile = null)
            :base(credentials, containerName, serviceName, resourceProfile:resourceProfile)
        {
        }

        public async Task Log(LogEvent logEvent)
        {
            await Output(mIdMaker(logEvent) + ".json", mDirectoryMaker(logEvent), logEvent);
        }

        protected override string DirectoryMaker(LogEvent logEvent)
        {
            string level = Enum.GetName(typeof(LoggingLevel), logEvent.Level);

            return string.Format("{0}/{1}/{2:yyyy-MM-dd}/{2:HH}", mServiceName, level, DateTime.UtcNow);
        }

        protected override string IdMaker(LogEvent logEvent)
        {
            if (logEvent is ILogStoreName)
                return ((ILogStoreName)logEvent).StorageId;
            else
                return string.Format("{0}_{1}", logEvent.GetType().Name, Guid.NewGuid().ToString("N"));
        }
    }
}
