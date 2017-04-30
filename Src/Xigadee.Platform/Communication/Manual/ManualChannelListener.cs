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
    /// This is the manual tester, primarily used for testing.
    /// </summary>
    public class ManualChannelListener: MessagingListenerBase<ManualChannelConnection, ManualChannelMessage, ManualChannelClientHolder>
    {

        protected override ManualChannelClientHolder ClientCreate(ListenerPartitionConfig partition)
        {
            var client = base.ClientCreate(partition);

            client.Name = mPriorityClientNamer(ChannelId, partition.Priority);

            client.ClientCreate = () => new ManualChannelConnection();
            client.ClientClose = () => { };

            return client;
        }

        /// <summary>
        /// This method injects a service message manually in to the Microservice.
        /// </summary>
        /// <param name="payload">The message payload.</param>
        /// <param name="priority">The optional priority. The default is 1.</param>
        public void Inject(TransmissionPayload payload, int? priority = null)
        {
            var client = ClientResolve(priority ?? mDefaultPriority ?? 1);

            client.Inject(payload);
        }

    }
}
