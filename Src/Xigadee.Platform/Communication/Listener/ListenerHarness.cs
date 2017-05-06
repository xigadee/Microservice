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
    public abstract class ListenerHarness<L>: MessagingHarness<L>
        where L:class, IListener, IService, IRequireSharedServices
    {

        protected override void Configure(L service)
        {
            base.Configure(service);

            service.ChannelId = "internal";
            service.PriorityPartitions = PriorityPartitions;
        }

        public override void Start()
        {
            base.Start();
            Clients = new ClientPriorityCollection(new IListener[] { Service }.ToList(), null, PollAlgorithm,0);
        }

        public virtual IListenerClientPollAlgorithm PollAlgorithm => new MultipleClientPollSlotAllocationAlgorithm();

        public ClientPriorityCollection Clients { get; set; }

        #region PriorityPartitions
        /// <summary>
        /// This is the set of default priority partitions. Override if you wish to change.
        /// </summary>
        public virtual List<ListenerPartitionConfig> PriorityPartitions => ListenerPartitionConfig.Init(0, 1).ToList();
        #endregion
    }
}
