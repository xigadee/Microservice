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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xigadee;

namespace Xigadee
{
    /// <summary>
    /// This class is used to mock the standard data collection for the holder.
    /// </summary>
    public class ServiceHarnessDataCollection : IDataCollection
    {
        /// <summary>
        /// This is a collection of the events generated.
        /// </summary>
        public ConcurrentQueue<EventHolder> Events = new ConcurrentQueue<EventHolder>();
        /// <summary>
        /// This method writes the events to the queue.
        /// </summary>
        /// <param name="eventData">The event data.</param>
        /// <param name="support">The collection support type.</param>
        /// <param name="sync">The sync flag. Ignored for this usage.</param>
        /// <param name="claims">The current claims.</param>
        public void Write(EventBase eventData, DataCollectionSupport support, bool sync = false, ClaimsPrincipal claims = null)
        {
            Events.Enqueue(new EventHolder(support, claims) { Data = eventData, Sync = sync, Timestamp = Environment.TickCount });
        }
    }
}
