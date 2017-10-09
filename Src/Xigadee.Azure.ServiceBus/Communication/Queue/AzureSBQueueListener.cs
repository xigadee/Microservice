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
using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This is the Azure service bus queue listener class.
    /// </summary>
    [DebuggerDisplay("AzureSBQueueListener: {MappingChannelId} {ChannelId}")]
    public class AzureServiceBusQueueListener : AzureSBListenerBase<QueueClient, Microsoft.Azure.ServiceBus.Message>
    {
        #region ClientCreate()
        /// <summary>
        /// This override sets the receive options for the client.
        /// </summary>
        /// <returns>Returns the client.</returns>
        protected override AzureClientHolder<QueueClient, Microsoft.Azure.ServiceBus.Message> ClientCreate(ListenerPartitionConfig partition)
        {
            var client = base.ClientCreate(partition);

            client.Type ="Queue Listener";
            client.Name = mPriorityClientNamer(Connection.EntityName, partition.Priority);

            client.AssignMessageHelpers();

            //client.FabricInitialize = () =>
            //{
            //    var queuedesc = Connection.QueueFabricInitialize(client.Name, lockDuration: partition.FabricMaxMessageLock);
            //};

            client.SupportsQueueLength = true;

            //client.QueueLength = () =>
            //{
            //    try
            //    {
            //        var desc = Connection.NamespaceManager.GetQueue(client.Name);

            //        client.QueueLengthLastPoll = DateTime.UtcNow;

            //        if (IsDeadLetterListener)
            //            return desc.MessageCountDetails.DeadLetterMessageCount;
            //        else
            //            return desc.MessageCountDetails.ActiveMessageCount;
            //    }
            //    catch (Exception)
            //    {
            //        return null;
            //    }
            //};

            //client.ClientCreate = () =>
            //{
            //    var messagingFactory = MessagingFactory.CreateFromConnectionString(Connection.ConnectionString);

            //    string queueName = IsDeadLetterListener ? QueueClient.FormatDeadLetterPath(client.Name) : client.Name;

            //    var queue = messagingFactory.CreateQueueClient(queueName);

            //    return queue;
            //};

            //client.MessageReceive = async (c,t) =>
            //{
            //    return await client.Client.ReceiveBatchAsync(c??10, TimeSpan.FromMilliseconds(t??500));
            //};

            return client;
        } 
        #endregion
    }
}
