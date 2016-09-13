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
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
#endregion
namespace Xigadee
{
    [DebuggerDisplay("AzureSBTopicSender: {ChannelId}")]
    public class AzureSBTopicSender : AzureSBSenderBase<TopicClient,BrokeredMessage>
    {
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="channelId">The internal channel id used to resolve the comms resource.</param>
        /// <param name="connectionString">The Azure connection string.</param>
        /// <param name="connectionName">The specific connection name to use.</param>
        public AzureSBTopicSender(string channelId, string connectionString
            , string connectionName
            , IEnumerable<SenderPartitionConfig> priorityPartitions
            , IBoundaryLogger boundaryLogger = null) :
            base(channelId, connectionString, connectionName, priorityPartitions, boundaryLogger) { } 
        #endregion

        protected override AzureClientHolder<TopicClient, BrokeredMessage> ClientCreate(SenderPartitionConfig partition)
        {
            var client =  base.ClientCreate(partition);

            client.Type = "Topic Sender";

            client.Name = mPriorityClientNamer(mAzureSB.ConnectionName, partition.Priority);

            client.AssignMessageHelpers();

            client.FabricInitialize = () => mAzureSB.TopicFabricInitialize(client.Name);

            client.ClientCreate = () => TopicClient.CreateFromConnectionString(mAzureSB.ConnectionString, client.Name);

            client.MessageTransmit = async (b) => await client.Client.SendAsync(b);

            return client;
        }
    }
}
