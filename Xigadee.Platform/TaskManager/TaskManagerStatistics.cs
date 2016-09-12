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
using System.Threading;
using System.Threading.Tasks;

namespace Xigadee
{
    public class TaskManagerStatistics: MessagingStatistics
    {
        public TaskManagerStatistics()
        {
        }

        private long mTimeouts;

        public void TimeoutRegister(long count)
        {
            Interlocked.Add(ref mTimeouts, count);
        }

        public ICpuStats Cpu { get; set; }

        public TaskAvailabilityStatistics Availability { get; set; }

        public string[] Running { get; set; }

        public bool AutotuneActive { get; set; }

        public int TaskCount { get; set; }

        public int? InternalQueueLength { get; set; }

        public QueueTrackerStatistics Queues { get; set; }
    }
}
