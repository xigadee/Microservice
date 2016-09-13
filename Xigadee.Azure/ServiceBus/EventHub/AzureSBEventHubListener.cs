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
using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
#endregion
namespace Xigadee
{
    public class AzureSBEventHubListener : AzureSBListenerBase<EventHubClient, EventData>
    {
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="channelId">The channelId used to identify internally the comms layer.</param>
        /// <param name="connectionString">The Azure Service bus connection string.</param>
        /// <param name="connectionName"></param>
        /// <param name="defaultTimeout"></param>
        /// <param name="isDeadLetterListener"></param>
        public AzureSBEventHubListener(string channelId, string connectionString, string connectionName, IEnumerable<ResourceProfile> resourceProfiles = null)
            : base(channelId, connectionString, connectionName, ListenerPartitionConfig.Init(1), resourceProfiles:resourceProfiles)
        {
        } 
        #endregion


    }
}
