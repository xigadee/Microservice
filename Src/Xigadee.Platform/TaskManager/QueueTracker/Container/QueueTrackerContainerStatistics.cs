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
    /// <summary>
    /// This class holds the queue tracker container statistics.
    /// </summary>
    public class QueueTrackerContainerStatistics: StatusBase
    {
        /// <summary>
        /// This is the list of queues and their statistics.
        /// </summary>
        public List<QueueTrackerStatistics> Queues { get; set; }
        /// <summary>
        /// Gets or sets the total of pending requests.
        /// </summary>
        public int Waiting { get; set; }

        /// <summary>
        /// This is the message logged for simple loggers.
        /// </summary>
        public override string Message
        {
            get
            {
                var result = string.Concat(Queues?.Select((q) => q.Message + "|")??new string[] {"|"});
                return result.Substring(0, result.Length -1);
            }
            set
            {
            }
        }
    }
}
