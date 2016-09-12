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

#region using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion
namespace Xigadee
{
    public abstract class EventSourceEntryBase
    {
        public EventSourceEntryBase()
        {
            UTCTimeStamp = DateTime.UtcNow;
        }

        public DateTime UTCTimeStamp { get; set; }

        public string BatchId { get; set; }

        public string CorrelationId { get; set; }

        public string EventType { get; set; }

        public string EntityType { get; set; }

        public string EntityVersion { get; set; }

        public string EntityVersionOld { get; set; }

        public abstract string Key { get; }

        public string EntitySource { get; set; }

        public string EntitySourceId { get; set; }

        public string EntitySourceName { get; set; }
    }

}
