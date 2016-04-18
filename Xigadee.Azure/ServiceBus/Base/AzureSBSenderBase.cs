#region using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging; 
#endregion
namespace Xigadee
{
    public abstract class AzureSBSenderBase<C, M> : MessagingSenderBase<C, M, AzureClientHolder<C, M>>
        where C : ClientEntity
    {
        #region Declarations
        /// <summary>
        /// This is the Azure connection class.
        /// </summary>
        protected AzureConnection mAzureSB;
        #endregion
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="channelId">The internal channel id used to resolve the comms resource.</param>
        /// <param name="connectionString">The Azure connection string.</param>
        /// <param name="connectionName">The specific connection name to use.</param>
        public AzureSBSenderBase(string channelId
            , string connectionString
            , string connectionName
            , IEnumerable<SenderPartitionConfig> priorityPartitions
            , IBoundaryLogger boundaryLogger = null) 
            :base(channelId, priorityPartitions, boundaryLogger) 
        {
            mAzureSB = new AzureConnection() { ConnectionName = connectionName, ConnectionString = connectionString };
        } 
        #endregion


        protected override AzureClientHolder<C, M> ClientCreate(SenderPartitionConfig partition)
        {
            var client = base.ClientCreate(partition);

            client.Start = () =>
            {
                client.Client = client.ClientCreate();
                client.IsActive = true;
            };

            client.ClientClose = () => client.Client.Close();

            return client;
        }
    }
}
