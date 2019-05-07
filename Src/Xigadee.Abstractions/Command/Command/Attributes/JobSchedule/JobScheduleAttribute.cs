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
using System.Diagnostics;

namespace Xigadee
{
    /// <summary>
    /// This attribute can be set against a command method to register it for a schedule job poll call.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class JobScheduleAttribute: JobScheduleAttributeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JobScheduleAttribute"/> class.
        /// </summary>
        /// <param name="initialWait">The initial wait before the job first polls. If this is null the job will be polled immediately and you can set the schedule manually.
        /// Leave this as null if you do not want an initial wait, but wish the job to poll after the frequency.
        /// The string should be in the format of a [ws][-]{ d | [d.]hh:mm[:ss[.ff]] }[ws] as defined in the Timespan.Parse method, i.e. 10 seconds is 00:00:10</param>
        /// <param name="frequency">The frequency of the poll after the initial wait.
        /// The string should be in the format of a [ws][-]{ d | [d.]hh:mm[:ss[.ff]] }[ws] as defined in the Timespan.Parse method.</param>
        /// <param name="isLongRunningProcess">Specifies whether the schedule is a long running process.</param>
        /// <param name="name">The optional name for the schedule. This will be displayed in the Microservice statistics.</param>
        /// <see cref="https://msdn.microsoft.com/en-us/library/se73z7b9(v=vs.110).aspx" />
        public JobScheduleAttribute(string name, string initialWait = null, string frequency = null, bool isLongRunningProcess = false) 
            : base(name, false, initialWait, frequency, isLongRunningProcess)
        {
        }
    }
}
