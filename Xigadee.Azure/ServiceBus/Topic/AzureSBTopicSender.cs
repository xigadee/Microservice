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
        public AzureSBTopicSender(string channelId, string connectionString, string connectionName, SenderPartitionConfig[] priorityPartitions
            , IBoundaryLogger boundaryLogger = null) :
            base(channelId, connectionString, connectionName, priorityPartitions, boundaryLogger) { } 
        #endregion

        protected override AzureClientHolder<TopicClient, BrokeredMessage> ClientCreate(SenderPartitionConfig partition)
        {
            var client =  base.ClientCreate(partition);

            client.Type = "Topic Sender";

            client.Name = mPriorityClientNamer(mAzureSB.ConnectionName, partition.Id);

            client.AssignMessageHelpers();

            client.FabricInitialize = () => mAzureSB.TopicFabricInitialize(client.Name);

            client.ClientCreate = () => TopicClient.CreateFromConnectionString(mAzureSB.ConnectionString, client.Name);

            client.MessageTransmit = async (b) => await client.Client.SendAsync(b);

            return client;
        }
    }
}
