#region Copyright
// Copyright Hitachi Consulting
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using Microsoft.Azure.ServiceBus;
using Xigadee;
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
        /// <param name="channelId">The channel identifier.</param>
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
