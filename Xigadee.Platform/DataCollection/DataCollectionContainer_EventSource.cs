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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public partial class DataCollectionContainer
    {

        private void WriteSync<K, E>(IEventSource eventSource, string originatorId, EventSourceEntry<K, E> entry, DateTime? utcTimeStamp)
        {
            int numberOfRetries = 0;

            while (true)
            {
                try
                {
                    eventSource.Write(originatorId, entry, utcTimeStamp).Wait();

                    return;
                }
                catch (Exception ex)
                {
                    if (numberOfRetries >= mPolicy.EventSourceRetryLimit)
                    {
                        this.LogException(string.Format("Unable to log to event source {0} for {1}-{2}-{3}", eventSource.GetType().Name, entry.EntityType, entry.Key, entry.EntityVersion), ex);
                        throw;
                    }
                }

                Task.Delay(TimeSpan.FromMilliseconds(numberOfRetries * 100)).Wait();

                numberOfRetries++;
            }
        }

        /// <summary>
        /// This is the external method to submit events to the event source.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <param name="originatorId"></param>
        /// <param name="entry"></param>
        /// <param name="utcTimeStamp"></param>
        /// <param name="sync"></param>
        /// <returns></returns>
        public async Task Write<K, E>(string originatorId, EventSourceEntry<K, E> entry, DateTime? utcTimeStamp = default(DateTime?), bool sync = false)
        {
            Write(new EventSourceEvent { OriginatorId = originatorId, Entry = entry, UtcTimeStamp = utcTimeStamp }, DataCollectionSupport.EventSource, sync);
        }

    }
}
