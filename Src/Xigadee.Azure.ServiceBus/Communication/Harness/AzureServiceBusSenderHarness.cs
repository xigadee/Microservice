#region using
using Microsoft.Azure.ServiceBus;
using System;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This is the Service Bus Queue sender harness.
    /// </summary>
    public class AzureServiceBusQueueSenderHarness: SenderHarness<AzureServiceBusQueueSender>
    {
        /// <summary>
        /// Configures the specified configuration for the Azure Service Bus.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="channelId">The channel identifier.</param>
        /// <param name="boundaryLoggingActive">Sets boundary logging as active.</param>
        public override void Configure(IEnvironmentConfiguration configuration
            , string channelId
            , bool boundaryLoggingActive = true)
        {
            base.Configure(configuration, channelId, boundaryLoggingActive);
            Service.AzureServiceBusPropertiesSet(configuration);
        }

    }
    /// <summary>
    /// This is the Service Bus Topic sender harness.
    /// </summary>
    public class AzureServiceBusTopicSenderHarness : SenderHarness<AzureServiceBusTopicSender>
    {
        /// <summary>
        /// Configures the specified configuration for the Azure Service Bus.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="channelId">The channel identifier.</param>
        /// <param name="boundaryLoggingActive">Sets boundary logging as active.</param>
        public override void Configure(IEnvironmentConfiguration configuration
            , string channelId
            , bool boundaryLoggingActive = true)
        {
            base.Configure(configuration, channelId, boundaryLoggingActive);
            Service.AzureServiceBusPropertiesSet(configuration);
        }
    }

}
