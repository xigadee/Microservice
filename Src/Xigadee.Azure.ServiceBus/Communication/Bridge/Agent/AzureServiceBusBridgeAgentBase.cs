using Microsoft.Azure.ServiceBus;
using System;

namespace Xigadee
{
    /// <summary>
    /// This class is used to configure extended Azure Service Bus properties.
    /// </summary>
    public class AzureServiceBusExtendedProperties
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AzureServiceBusExtendedProperties"/> class.
        /// </summary>
        /// <param name="entityName">Name of the Service Bus entity.</param>
        public AzureServiceBusExtendedProperties(string entityName)
        {
            EntityName = entityName;
        }

        /// <summary>
        /// Gets the name of the Service Bus entity.
        /// </summary>
        public string EntityName { get; }

        #region Implicit conversion from string
        /// <summary>
        /// Implicitly converts a string value in to a AzureServiceBusExtendedProperties with the string set to the Entity name.
        /// </summary>
        /// <param name="t">The entity name.</param>
        public static implicit operator AzureServiceBusExtendedProperties(string t)
        {
            return new AzureServiceBusExtendedProperties(t);
        }
        #endregion
    }

    /// <summary>
    /// This is the base abstract class for Service Bus Agents.
    /// </summary>
    /// <seealso cref="Xigadee.CommunicationFabricBridgeBase" />
    public abstract class AzureServiceBusBridgeAgentBase : CommunicationFabricBridgeBase, IAzureServiceBusFabricBridge
    {
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="connectionString">The service bus connection string.</param>
        /// <param name="receiveMode">The default receive mode.</param>
        /// <param name="retryPolicy">The default retry policy.</param>
        protected AzureServiceBusBridgeAgentBase(ServiceBusConnectionStringBuilder connectionString
            , ReceiveMode receiveMode = ReceiveMode.PeekLock
            , RetryPolicy retryPolicy = null):base(CommunicationFabricMode.NotSet)
        {
            Connection = new AzureServiceBusConnection(connectionString, receiveMode, retryPolicy);
        }

        /// <summary>
        /// Gets the service bus connection.
        /// </summary>
        protected AzureServiceBusConnection Connection { get; }

        /// <summary>
        /// Gets a listener agent for the bridge.
        /// </summary>
        /// <param name="properties">The service bus extended properties.</param>
        /// <returns>Returns the listener agent.</returns>
        public abstract IListener GetListener(AzureServiceBusExtendedProperties properties);
        /// <summary>
        /// Gets a sender for the bridge.
        /// </summary>
        /// <param name="properties">The service bus extended properties.</param>
        /// <returns>Returns the sender agent.</returns>
        public abstract ISender GetSender(AzureServiceBusExtendedProperties properties);

    }
}
