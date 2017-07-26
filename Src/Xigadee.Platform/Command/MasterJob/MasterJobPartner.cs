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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This class is used to record the standby partner for a master job.
    /// </summary>
    public class MasterJobPartner
    {
        /// <summary>
        /// The constructor is passed the partner service id.
        /// </summary>
        /// <param name="Id">The remote service id.</param>
        /// <param name="isStandby">Specifies whether the remote partner is in standby mode.</param>
        public MasterJobPartner(string Id, bool isStandby = true)
        {
            LastNotification = DateTime.UtcNow;
            ServiceId = Id;
            IsStandby = isStandby;
        }
        /// <summary>
        /// The time of last notification.
        /// </summary>
        public DateTime LastNotification { get; }
        /// <summary>
        /// The microservice is.
        /// </summary>
        public string ServiceId { get; }
        /// <summary>
        /// Specifies whether this partner is in standby.
        /// </summary>
        public bool IsStandby{get;}
    }
}
