//using Microsoft.Azure.ServiceBus;
//using System;

//namespace Xigadee
//{
//    /// <summary>
//    /// This is the Azure Service Bus Topic Agent.
//    /// </summary>
//    /// <seealso cref="Xigadee.AzureServiceBusBridgeAgentBase" />
//    public class AzureServiceBusTopicBridgeAgent : AzureServiceBusBridgeAgentBase
//    {
//        /// <summary>
//        /// Initializes a new instance of the <see cref="AzureServiceBusTopicBridgeAgent"/> class.
//        /// </summary>
//        /// <param name="connectionString">The service bus connection string.</param>
//        /// <param name="receiveMode">The default receive mode.</param>
//        /// <param name="retryPolicy">The default retry policy.</param>
//        public AzureServiceBusTopicBridgeAgent(ServiceBusConnectionStringBuilder connectionString
//            , ReceiveMode receiveMode = ReceiveMode.PeekLock
//            , RetryPolicy retryPolicy = null
//            ) 
//            : base(connectionString, receiveMode, retryPolicy)
//        {
//        }

//        /// <summary>
//        /// This method returns a new listener.
//        /// </summary>
//        /// <returns>
//        /// The queue listener.
//        /// </returns>
//        public override IListener GetListener()
//        {
//            var listener = new AzureServiceBusTopicListener();

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
//            var listener = new AzureServiceBusTopicListener();

//            listener.Connection = Connection;
//            listener.EntityName = properties.EntityName;

//            return listener;
//        }

//        /// <summary>
//        /// This method returns a new sender.
//        /// </summary>
//        /// <returns>
//        /// The queue sender.
//        /// </returns>
//        public override ISender GetSender()
//        {
//            var sender = new AzureServiceBusTopicSender();

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
//            var sender = new AzureServiceBusTopicSender();

//            sender.Connection = Connection;
//            sender.EntityName = properties.EntityName;

//            return sender;
//        }
//    }
//}
