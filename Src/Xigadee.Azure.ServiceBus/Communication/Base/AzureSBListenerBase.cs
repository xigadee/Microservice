#region using
using Microsoft.Azure.ServiceBus;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This is the abstract listener base.
    /// </summary>
    public abstract class AzureSBListenerBase<C, M> : MessagingListenerBase<C, M, AzureClientHolder<C, M>>//, IAzureServiceBusMessagingService<ListenerPartitionConfig>
        where C : ClientEntity
    {
        #region Connection
        /// <summary>
        /// This is the Azure connection class.
        /// </summary>
        public AzureServiceBusConnection Connection { get; set; }
        #endregion
        #region EntityName
        /// <summary>
        /// This is the Azure Service Bus entity name. Usually this will map the Xigadee channel identifier, but can be overridden in specific circumstances.
        /// </summary>
        public string EntityName { get; set; } 
        #endregion

        #region IsDeadLetterListener
        /// <summary>
        /// This property identifies whether the listener is a deadletter listener.
        /// </summary>
        public bool IsDeadLetterListener
        {
            get;
            set;
        } 
        #endregion

        //#region ClientCreate()
        ///// <summary>
        ///// This method sets the start and stop listener methods.
        ///// </summary>
        ///// <returns>The client.</returns>
        //protected override AzureClientHolder<C, M> ClientCreate(ListenerPartitionConfig partition)
        //{
        //    var client = base.ClientCreate(partition);

        //    client.ClientClose = () => 
        //    {
        //        if (client.Client != null)
        //            client.Client.CloseAsync().Wait();
        //    };
          
        //    return client;
        //}
        //#endregion

        #region SettingsValidate()
        /// <summary>
        /// This override is used to validate the listener configuration settings on startup.
        /// </summary>
        protected override void SettingsValidate()
        {
            if (Connection == null)
                throw new CommunicationAgentStartupException("Connection", "Connection cannot be null");

            base.SettingsValidate();
        } 
        #endregion
    }
}
