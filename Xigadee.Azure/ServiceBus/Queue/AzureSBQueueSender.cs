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
    /// <summary>
    /// This is the Azure service bus sender component used to transmit messages to a remote service.
    /// </summary>
    [DebuggerDisplay("AzureSBQueueSender: {ChannelId}")]
    public class AzureSBQueueSender : AzureSBSenderBase<QueueClient,BrokeredMessage>
    {
        #region Declarations
        #endregion
        #region Constructor
        /// <summary>
        /// This is the default constructor for the Azure service bus sender.
        /// </summary>
        /// <param name="channelId">The channel Id of the sender.</param>
        /// <param name="connectionString">The Azure connection string.</param>
        /// <param name="connectionName">The connection name.</param>
        public AzureSBQueueSender(string channelId, string connectionString, string connectionName
            , IEnumerable<SenderPartitionConfig> priorityPartitions
            , IBoundaryLogger boundaryLogger = null) :
            base(channelId, connectionString, connectionName, priorityPartitions, boundaryLogger) 
        { 
        } 
        #endregion

        #region ClientCreate()
        /// <summary>
        /// This override sets the transmit options for the client.
        /// </summary>
        /// <returns>Returns the client.</returns>
        protected override AzureClientHolder<QueueClient, BrokeredMessage> ClientCreate(SenderPartitionConfig partition)
        {
            var client = base.ClientCreate(partition);

            client.Type = "Queue Sender";
            client.Name = mPriorityClientNamer(mAzureSB.ConnectionName, partition.Id);

            client.AssignMessageHelpers();

            client.FabricInitialize = () => mAzureSB.QueueFabricInitialize(client.Name);

            //Set the method that creates the client.
            client.ClientCreate = () => QueueClient.CreateFromConnectionString(mAzureSB.ConnectionString, client.Name);

            //We have to do this due to the stupid inheritance rules for Azure Service Bus.
            client.MessageTransmit = async (b) => await client.Client.SendAsync(b);

            return client;
        } 
        #endregion
    }
}
