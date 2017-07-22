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
        public MasterJobContext()
        {
            StandbyPartner = new ConcurrentDictionary<string, StandbyPartner>();
        }

        /// <summary>
        /// This collection holds the list of master job standby partners.
        /// </summary>
        public ConcurrentDictionary<string, StandbyPartner> StandbyPartner { get; }

        /// <summary>
        /// The current status.
        /// </summary>
        public DateTime? mCurrentMasterReceiveTime;

        public string mCurrentMasterServiceId;

        public int mCurrentMasterPollAttempts;

        public Random mRandom = new Random(Environment.TickCount);

        public DateTime? mMasterJobLastPollTime;
        /// <summary>
        /// This holds the master job collection.
        /// </summary>
        public Dictionary<Guid, MasterJobHolder> mMasterJobs;

        /// <summary>
        /// This is the current state of the MasterJob
        /// </summary>
        public MasterJobState State { get; set; }
    }
}
