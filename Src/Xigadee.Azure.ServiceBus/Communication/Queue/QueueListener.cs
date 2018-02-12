//#region using
//using Microsoft.Azure.ServiceBus;
//using System;
//using System.Collections.Generic;
//using System.Threading;
//using System.Diagnostics;
//#endregion
//namespace Xigadee
//{
//    /// <summary>
//    /// This is the Azure service bus queue listener class.
//    /// </summary>
//    [DebuggerDisplay("AzureSBQueueListener: {MappingChannelId} {ChannelId}")]
//    public class AzureServiceBusQueueListener : AzureSBListenerBase<QueueClient, Microsoft.Azure.ServiceBus.Message>
//    {
//        #region ClientCreate()
//        /// <summary>
//        /// This override sets the receive options for the client.
//        /// </summary>
//        /// <returns>Returns the client.</returns>
//        protected override AzureClientHolder<QueueClient, Microsoft.Azure.ServiceBus.Message> ClientCreate(ListenerPartitionConfig partition)
//        {
//            var client = base.ClientCreate(partition);

//            client.Type ="Queue Listener";
//            client.Name = mPriorityClientNamer(EntityName ?? ChannelId, partition.Priority);

//            client.AssignMessageHelpers();

//            //client.FabricInitialize = () =>
//            //{
//            //    var queuedesc = Connection.QueueFabricInitialize(client.Name, lockDuration: partition.FabricMaxMessageLock);
//            //};

//            client.SupportsQueueLength = false;

//            //client.QueueLength = () =>
//            //{
//            //    try
//            //    {
//            //        var desc = Connection.NamespaceManager.GetQueue(client.Name);

//            //        client.QueueLengthLastPoll = DateTime.UtcNow;

//            //        if (IsDeadLetterListener)
//            //            return desc.MessageCountDetails.DeadLetterMessageCount;
//            //        else
//            //            return desc.MessageCountDetails.ActiveMessageCount;
//            //    }
//            //    catch (Exception)
//            //    {
//            //        return null;
//            //    }
//            //};

//            client.ClientCreate = () =>
//            {            
//                string queueName = IsDeadLetterListener ? EntityNameHelper.FormatDeadLetterPath(client.Name) : client.Name;

//                var qClient = new QueueClient(Connection.Connection.ToString(), queueName, Connection.DefaultReceiveMode, Connection.DefaultRetryPolicy);
                
//                return qClient;
//            };

//            //client.ClientCreate = () =>
//            //{
//            //    var messagingFactory = MessagingFactory.CreateFromConnectionString(Connection.ConnectionString);

//            //    string queueName = IsDeadLetterListener ? QueueClient.FormatDeadLetterPath(client.Name) : client.Name;

//            //    var queue = messagingFactory.CreateQueueClient(queueName);

//            //    return queue;
//            //};

//            //client.MessageReceive = async (c,t) =>
//            //{
//            //    return await client.Client.ReceiveBatchAsync(c??10, TimeSpan.FromMilliseconds(t??500));
//            //};

//            return client;
//        } 
//        #endregion
//    }
//}
