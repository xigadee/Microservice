using Microsoft.Azure.ServiceBus;
namespace Xigadee
{
    public static partial class AzureServiceBusExtensionMethods
    {
        #region AzureServiceBusPropertiesSet ...
        /// <summary>
        /// Sets the Azure Service Bus connection properties.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="config">The configuration.</param>
        public static void AzureServiceBusPropertiesSet(this IAzureServiceBusMessagingService service
            , IEnvironmentConfiguration config)
        {
            config.ServiceBusConnectionValidate(null);
            var connection = config.ServiceBusConnection();
            service.Connection = new AzureServiceBusConnection(
                new ServiceBusConnectionStringBuilder(connection)
                , ReceiveMode.PeekLock
                , null);
        } 
        #endregion

        #region ServiceBusConnectionValidate(this IEnvironmentConfiguration Configuration, string serviceBusConnection)
        /// <summary>
        /// This method validates that the service bus connection is set.
        /// </summary>
        /// <param name="Configuration">The configuration.</param>
        /// <param name="serviceBusConnection">The alternate connection.</param>
        /// <returns>Returns the connection from either the parameter or from the settings.</returns>
        private static string ServiceBusConnectionValidate(this IEnvironmentConfiguration Configuration, string serviceBusConnection)
        {
            var conn = serviceBusConnection ?? Configuration.ServiceBusConnection();

            if (string.IsNullOrEmpty(conn))
                throw new AzureServiceBusConnectionException(KeyServiceBusConnection);

            return conn;
        }
        #endregion

        #region ConfigOverrideSetServiceBusConnection<P> ...
        /// <summary>
        /// This extension allows the Service Bus connection values to be manually set as override parameters.
        /// </summary>
        /// <param name="pipeline">The incoming pipeline.</param>
        /// <param name="connection">The Service Bus connection.</param>
        /// <returns>The pass-through of the pipeline.</returns>
        public static P ConfigOverrideSetServiceBusConnection<P>(this P pipeline, string connection)
            where P : IPipeline
        {
            pipeline.ConfigurationOverrideSet(KeyServiceBusConnection, connection);
            return pipeline;
        } 
        #endregion
    }
}
