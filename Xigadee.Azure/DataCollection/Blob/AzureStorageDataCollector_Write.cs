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
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Xigadee
{
    public partial class AzureStorageDataCollector
    {

        /// <summary>
        /// Output the data for the two option type.
        /// </summary>
        /// <param name="option">The storage options</param>
        /// <param name="e">The event object.</param>
        protected void Write(AzureStorageDataCollectorOptions option, EventBase e)
        {
            List<Task> mActions = new List<Task>();

            if ((option.Behaviour & AzureStorageDataCollectorOptions.StorageBehaviour.Blob) > 0)
                mActions.Add(OutputBlob(option, e));
            if ((option.Behaviour & AzureStorageDataCollectorOptions.StorageBehaviour.Blob) > 0)
                mActions.Add(OutputTable(option, e));

            Task.WhenAll(mActions).Wait();
        }

        protected virtual void WriteLogEvent(LogEvent e)
        {
            string level = Enum.GetName(typeof(LoggingLevel), e.Level);
            string id = "";
            string folder = string.Format("{0}/{1}/{2:yyyy-MM-dd}/{2:HH}", mServiceName, level, DateTime.UtcNow);

            //if (e is ILogStoreName)
            //    return ((ILogStoreName)logEvent).StorageId;

            //// If there is a category specified and it contains valid digits or characters then make it part of the log name to make it easier to filter log events
            //if (!string.IsNullOrEmpty(logEvent.Category) && logEvent.Category.Any(char.IsLetterOrDigit))
            //    return string.Format("{0}_{1}_{2}", logEvent.GetType().Name, new string(logEvent.Category.Where(char.IsLetterOrDigit).ToArray()), Guid.NewGuid().ToString("N"));

            //return string.Format("{0}_{1}", logEvent.GetType().Name, Guid.NewGuid().ToString("N"));

            Write(mPolicy.Log, e);
        }

        protected virtual void WriteEventSource(EventSourceEvent e)
        {
            string id = string.Format("{0}.json", string.Join("_", e.Entry.Key.Split(Path.GetInvalidFileNameChars())));
            string folder = string.Format("{0}/{1:yyyy-MM-dd}/{2}", mServiceName, e.Entry.UTCTimeStamp, e.Entry.EntityType);

            Write(mPolicy.EventSource, e);
        }

        protected virtual void WriteStatistics(MicroserviceStatistics e)
        {
            Write(mPolicy.Statistics, e);
        }

        protected virtual void WriteDispatcherEvent(DispatcherEvent e)
        {
            Write(mPolicy.Dispatcher, e);
        }

        protected virtual void WriteBoundaryEvent(BoundaryEvent e)
        {
            Write(mPolicy.Boundary, e);
        }

        protected virtual void WriteTelemetryEvent(TelemetryEvent e)
        {
            Write(mPolicy.Telemetry, e);
        }

        protected virtual void WriteCustom(EventBase e)
        {
            Write(mPolicy.Custom, e);
        }


    }
}
