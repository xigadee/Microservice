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
    /// <summary>
    /// This class contains the master job messaging action types used to negotiate control.
    /// </summary>
    public static class MasterJobStates
    {
        public const string WhoIsMaster = "whoismaster";
        public const string RequestingControl1 = "requestingcontrol1";
        public const string RequestingControl2 = "requestingcontrol2";
        public const string TakingControl = "takingcontrol";

        public const string IAmMaster = "iammaster";
        public const string IAmStandby = "iamstandby";

        public const string ResyncMaster = "resyncmaster";
    }

    /// <summary>
    /// This is the current status of the job.
    /// </summary>
    public enum MasterJobState: int
    {
        /// <summary>
        /// The master job is disabled.
        /// </summary>
        Disabled = -1,

        Inactive = 0,
        VerifyingComms = 1,
        Starting = 2,
        Requesting1 = 3,
        Requesting2 = 4,
        TakingControl = 5,
        Active = 10
    }
}
