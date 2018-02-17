//#region using
//using Microsoft.Azure.ServiceBus;
//using System;
//#endregion
//namespace Xigadee
//{
//    /// <summary>
//    /// This is the Service Bus Queue Listener harness.
//    /// </summary>
//    public class AzureServiceBusQueueListenerHarness: ListenerHarness<AzureServiceBusQueueListener>
//    {
//        /// <summary>
//        /// Configures the specified configuration for the Azure Service Bus.
//        /// </summary>
//        /// <param name="configuration">The configuration.</param>
//        /// <param name="channelId">The channel identifier.</param>
//        /// <param name="mappingChannelId">The actual channel id for the Azure Service Bus queue.</param>
//        /// <param name="boundaryLoggingActive">Sets boundary logging as active.</param>
//        public override void Configure(IEnvironmentConfiguration configuration
//            , string channelId
//            , string mappingChannelId = null
//            , bool boundaryLoggingActive = true)
//        {
//            base.Configure(configuration, channelId, boundaryLoggingActive);
//            //Service.ListenerMappingChannelId = mappingChannelId;
//            //Service.AzureServiceBusPropertiesSet(configuration);
//        }
//    }
//    /// <summary>
//    /// This is the Service Bus Topic Listener harness
//    /// </summary>
//    public class AzureServiceBusTopicListenerHarness : ListenerHarness<AzureServiceBusTopicListener>
//    {
//        /// <summary>
//        /// Configures the specified configuration for the Azure Service Bus.
//        /// </summary>
//        /// <param name="configuration">The configuration.</param>
//        /// <param name="channelId">The channel identifier.</param>
//        /// <param name="mappingChannelId">The actual channel id for the Azure Service Bus queue.</param>
//        /// <param name="boundaryLoggingActive">Sets boundary logging as active.</param>
//        public override void Configure(IEnvironmentConfiguration configuration
//            , string channelId
//            , string mappingChannelId = null
//            , bool boundaryLoggingActive = true)
//        {
//            base.Configure(configuration, channelId, boundaryLoggingActive);
//            //Service.ListenerMappingChannelId = mappingChannelId;
//            //Service.AzureServiceBusPropertiesSet(configuration);
//        }
//    }
//}
