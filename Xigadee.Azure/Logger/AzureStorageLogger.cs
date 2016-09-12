#region Copyright
// Copyright Hitachi Consulting
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Auth;

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

            // If there is a category specified and it contains valid digits or characters then make it part of the log name to make it easier to filter log events
            if (!string.IsNullOrEmpty(logEvent.Category) && logEvent.Category.Any(char.IsLetterOrDigit))
                return string.Format("{0}_{1}_{2}", logEvent.GetType().Name, new string(logEvent.Category.Where(char.IsLetterOrDigit).ToArray()), Guid.NewGuid().ToString("N"));

            return string.Format("{0}_{1}", logEvent.GetType().Name, Guid.NewGuid().ToString("N"));
        }
    }
}
