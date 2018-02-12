//using Microsoft.Azure.ServiceBus;
//using System;

//namespace Xigadee
//{
//    /// <summary>
//    /// This is the Azure Service Bus Queue agent.
//    /// </summary>
//    /// <seealso cref="Xigadee.AzureServiceBusBridgeAgentBase" />
//    public class AzureServiceBusQueueBridgeAgent : AzureServiceBusBridgeAgentBase
//    {
        
//        /// <summary>
//        /// Initializes a new instance of the <see cref="AzureServiceBusQueueBridgeAgent"/> class.
//        /// </summary>
//        /// <param name="connectionString">The service bus connection string.</param>
//        /// <param name="receiveMode">The default receive mode.</param>
//        /// <param name="retryPolicy">The default retry policy.</param>
//        public AzureServiceBusQueueBridgeAgent(ServiceBusConnectionStringBuilder connectionString
//            , ReceiveMode receiveMode = ReceiveMode.PeekLock
//            , RetryPolicy retryPolicy = null) 
//            : base(connectionString, receiveMode, retryPolicy)
//        {
            
//        }

//        /// <summary>
//        /// This method returns a new listener.
//        /// </summary>
//        /// <returns>The queue listener.</returns>
//        public override IListener GetListener()
//        {
//            var listener = new AzureServiceBusQueueListener();

//            listener.Connection = Connection;

//            return listener; 
//        }

//        /// <summary>
//        /// Gets a listener agent for the bridge.
//        /// </summary>
//        /// <param name="properties">The service bus extended properties.</param>
//        /// <returns>
//        /// Returns the listener agent.
//        /// </returns>
//        public override IListener GetListener(AzureServiceBusExtendedProperties properties)
//        {
//            var listener = new AzureServiceBusQueueListener();

//            listener.Connection = Connection;
//            listener.EntityName = properties.EntityName;

//            return listener;
//        }

//        /// <summary>
//        /// This method returns a new sender.
//        /// </summary>
//        /// <returns>The queue sender.</returns>
//        public override ISender GetSender()
//        {
//            var sender = new AzureServiceBusQueueSender();

//            sender.Connection = Connection;

//            return sender;
//        }


//        /// <summary>
//        /// Gets a sender for the bridge.
//        /// </summary>
//        /// <param name="properties">The service bus extended properties.</param>
//        /// <returns>
//        /// Returns the sender agent.
//        /// </returns>
//        public override ISender GetSender(AzureServiceBusExtendedProperties properties)
//        {
//            var sender = new AzureServiceBusQueueSender();

//            sender.Connection = Connection;
//            sender.EntityName = properties.EntityName;
//            return sender;
//        }
//    }
//}
