namespace Xigadee
{
    ///// <summary>
    ///// This interface is used for Azure service bus messaging.
    ///// </summary>
    ///// <typeparam name="P">The partition config type.</typeparam>
    //public interface IAzureServiceBusMessagingService<P>: IMessagingService<P>, IAzureServiceBusMessagingService
    //    where P : PartitionConfig
    //{

    //}

    /// <summary>
    /// This is the default Azure Service Bus properties.
    /// </summary>
    /// <seealso cref="Xigadee.IMessagingService{P}" />
    /// <seealso cref="Xigadee.IAzureServiceBusMessagingService" />
    public interface IAzureServiceBusMessagingService
    {
        /// <summary>
        /// This is the Azure Service Bus connection information.
        /// </summary>
        AzureServiceBusConnection Connection { get; set; }

        /// <summary>
        /// Gets or sets the Azure Service Bus entity name.
        /// </summary>
        string EntityName { get; set; }
    }
}
