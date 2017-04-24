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

using Microsoft.Azure.EventHubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Xigadee
{
    /// <summary>
    /// This collector is used to connect Xigadee event logging to Azure EventHubs fabric.
    /// </summary>
    public class EventHubsDataCollector: DataCollectorBase<DataCollectorStatistics, EventHubsDataCollectorPolicy>
    {
        protected EventHubClient mEventHubClient;
        protected readonly string mConnection;

        public EventHubsDataCollector(string connection
            , EventHubsDataCollectorPolicy policy = null
            , ResourceProfile resourceProfile = null
            , EncryptionHandlerId encryptionId = null
            , DataCollectionSupport? supportMap = null) : base(encryptionId, resourceProfile, supportMap, policy)
        {
            mConnection = connection;
        }

        protected override void StartInternal()
        {
            base.StartInternal();
            mEventHubClient = EventHubClient.CreateFromConnectionString(mConnection);
        }

        protected override void StopInternal()
        {
            mEventHubClient.Close();
            base.StopInternal();
        }
    }
}
