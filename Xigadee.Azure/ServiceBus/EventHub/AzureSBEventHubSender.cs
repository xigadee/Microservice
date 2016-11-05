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
    /// <summary>
    /// This is the default sender for the Azure Event Hub.
    /// </summary>
    public class AzureSBEventHubSender : AzureSBSenderBase<EventHubClient, EventData>
    {
        #region Constructor
        /// <summary>
        /// This is the default constructor for the Azure service bus sender.
        /// </summary>
        public AzureSBEventHubSender() :base()
        {
            PriorityPartitions = SenderPartitionConfig.Init(1).ToList();
        } 
        #endregion

        #region ClientCreate()
        /// <summary>
        /// This override sets the transmit options for the client.
        /// </summary>
        /// <returns>Returns the client.</returns>
        protected override AzureClientHolder<EventHubClient, EventData> ClientCreate(SenderPartitionConfig partition)
        {
            var client = base.ClientCreate(partition);
            client.Name = mPriorityClientNamer(AzureConn.ConnectionName, partition.Priority);

            client.AssignMessageHelpers();

            client.FabricInitialize = () => AzureConn.EventHubFabricInitialize(client.Name);
            //Set the method that creates the client.
            client.ClientCreate = () => EventHubClient.CreateFromConnectionString(AzureConn.ConnectionString, AzureConn.ConnectionName);

            //We have to do this due to the stupid inheritance rules for Azure Service Bus.
            client.MessageTransmit = async (b) => await client.Client.SendAsync(b);

            return client;
        } 
        #endregion
    }
}
