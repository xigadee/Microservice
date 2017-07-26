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
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;
using Xigadee;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This context holds the current state of the master job process for the command.
    /// </summary>
    public class MasterJobContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MasterJobContext"/> class.
        /// </summary>
        public MasterJobContext()
        {
            Partners = new ConcurrentDictionary<string, MasterJobPartner>();
            JobSchedules = new Dictionary<Guid, MasterJobHolder>();
        }

        /// <summary>
        /// This collection holds the list of master job standby partners.
        /// </summary>
        public ConcurrentDictionary<string, MasterJobPartner> Partners { get; }

        public Dictionary<Guid, MasterJobHolder> JobSchedules { get; }

        /// <summary>
        /// The current status.
        /// </summary>
        public DateTime? mCurrentMasterReceiveTime;

        public string mCurrentMasterServiceId;

        public int mCurrentMasterPollAttempts;

        public Random mRandom = new Random(Environment.TickCount);

        /// <summary>
        /// The timestamp for the last negotiation message out.
        /// </summary>
        public DateTime? MessageLastOut { get; set; }
        /// <summary>
        /// The timestamp for the last negotiation message received.
        /// </summary>
        public DateTime? MessageLastIn { get; set; }

        /// <summary>
        /// This holds the master job collection.
        /// </summary>
        public Dictionary<Guid, MasterJobHolder> mMasterJobs;

        /// <summary>
        /// This is the current state of the MasterJob
        /// </summary>
        public MasterJobState State { get; set; }

        #region MasterJobPartnerAdd(string originatorServiceId, bool isStandby)
        /// <summary>
        /// The method add the MasterJob Partner.
        /// </summary>
        /// <param name="originatorServiceId">The originator service identifier.</param>
        /// <param name="isStandby">if set to <c>true</c> [is standby].</param>
        public void PartnerAdd(string originatorServiceId, bool isStandby)
        {
            var record = new MasterJobPartner(originatorServiceId, isStandby);
            Partners.AddOrUpdate(record.ServiceId, s => record, (s, o) => record);
        }
        #endregion

        public void Start()
        {
            Partners.Clear();
        }
    }
}
