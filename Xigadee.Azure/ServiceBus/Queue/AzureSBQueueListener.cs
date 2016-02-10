#region using
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Threading; 
#endregion
namespace Xigadee
{
    /// <summary>
    /// This is the Azure service bus queue listener class.
    /// </summary>
    public class AzureSBQueueListener : AzureSBListenerBase<QueueClient,BrokeredMessage>
    {
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="channelId">The channelId.</param>
        /// <param name="connectionString">The service bus connection string.</param>
        /// <param name="connectionName">The connection name.</param>
        /// <param name="defaultTimeout">The default timeout for an incoming message.</param>
        /// <param name="isDeadLetterListener">Specifies whether this listener should listen on the deadletter partition</param>
        /// <param name="priorityPartitions">An integer array containing the number of priority partitions for the listener</param>
        /// <param name="mappingChannelId">This is the mapping channel. Incoming messages will have this channel appended to the payload messages.</param>
        public AzureSBQueueListener(string channelId, string connectionString, string connectionName
            , ListenerPartitionConfig[] priorityPartitions
            , bool isDeadLetterListener = false
            , string mappingChannelId = null
            , IEnumerable<ResourceProfile> resourceProfiles = null
            , IBoundaryLogger boundaryLogger = null)
            : base(channelId, connectionString, connectionName, priorityPartitions, isDeadLetterListener, mappingChannelId, resourceProfiles:resourceProfiles, boundaryLogger:boundaryLogger)
        {
        }
        #endregion

        #region ClientCreate()
        /// <summary>
        /// This override sets the receive options for the client.
        /// </summary>
        /// <returns>Returns the client.</returns>
        protected override AzureClientHolder<QueueClient, BrokeredMessage> ClientCreate(ListenerPartitionConfig partition)
        {
            var client = base.ClientCreate(partition);

            client.Type ="Queue Listener";
            client.Name = mPriorityClientNamer(mAzureSB.ConnectionName, partition.Id);

            client.AssignMessageHelpers();

            client.FabricInitialize = () =>
            {
                var queuedesc = mAzureSB.QueueFabricInitialize(client.Name);
            };

            client.SupportsQueueLength = true;

            client.QueueLength = () =>
            {
                try
                {
                    var desc = mAzureSB.NamespaceManager.GetQueue(client.Name);

                    client.QueueLengthLastPoll = DateTime.UtcNow;

                    if (client.IsDeadLetter)
                        return desc.MessageCountDetails.DeadLetterMessageCount;
                    else
                        return desc.MessageCountDetails.ActiveMessageCount;
                }
                catch (Exception)
                {
                    return null;
                }
            };

            client.ClientCreate = () =>
            {
                var messagingFactory = MessagingFactory.CreateFromConnectionString(mAzureSB.ConnectionString);

                string queueName = client.IsDeadLetter ? QueueClient.FormatDeadLetterPath(client.Name) : client.Name;

                var queue = messagingFactory.CreateQueueClient(queueName);

                return queue;
            };

            client.MessageReceive = async (c,t) =>
            {
                return await client.Client.ReceiveBatchAsync(c??10, TimeSpan.FromMilliseconds(t??500));
            };

            return client;
        } 
        #endregion
    }
}
