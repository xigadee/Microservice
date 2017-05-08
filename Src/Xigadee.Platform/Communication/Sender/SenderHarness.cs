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

using System.Collections.Generic;
using System.Linq;

namespace Xigadee
{
    /// <summary>
    /// This abstract harness holds a message listener and allows unit testing to be performed on it.
    /// </summary>
    /// <typeparam name="L">The listener type.</typeparam>
    public abstract class SenderHarness<L> : MessagingHarness<L>
        where L : class, ISender, IService
    {
        /// <summary>
        /// This override sets the priority partitions.
        /// </summary>
        /// <param name="service">The service.</param>
        protected override void Configure(L service)
        {
            base.Configure(service);

            service.PriorityPartitions = PriorityPartitions;
        }
        /// <summary>
        /// This is the set of default priority partitions. Override if you wish to change.
        /// </summary>
        public virtual List<SenderPartitionConfig> PriorityPartitions => SenderPartitionConfig.Init(0, 1).ToList();
    }
}
