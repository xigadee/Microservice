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
using System.Threading;
using System.Threading.Tasks; 
#endregion
namespace Xigadee
{
    /// <summary>
    /// This base class holds the process requests.
    /// </summary>
    public class RequestTracker
    {
        public RequestTracker()
        {
            UTCStart = DateTime.UtcNow;
            TTL = TimeSpan.FromMinutes(5);
        }

        public string Id { get; set; }

        public TransmissionPayload Payload { get; set; }

        public DateTime UTCStart { get; set; }

        public TimeSpan TTL { get; set; }

        public object State { get; set; }

        public bool HasExpired
        {
            get
            {
                return DateTime.UtcNow > UTCStart.Add(TTL);
            }
        }
    }
}
