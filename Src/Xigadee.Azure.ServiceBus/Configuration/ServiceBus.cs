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
using Xigadee;
namespace Xigadee
{
    public static partial class AzureServiceBusExtensionMethods
    {

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
                throw new AzureServiceBusConnectionException(AzureBaseHelper.KeyServiceBusConnection);

            return conn;
        }
        #endregion


        /// <summary>
        /// This extension allows the Service Bus connection values to be manually set as override parameters.
        /// </summary>
        /// <param name="pipeline">The incoming pipeline.</param>
        /// <param name="connection">The Service Bus connection.</param>
        /// <returns>The pass-through of the pipeline.</returns>
        public static P ConfigOverrideSetServiceBusConnection<P>(this P pipeline, string connection)
            where P : IPipeline
        {
            pipeline.ConfigurationOverrideSet(AzureBaseHelper.KeyServiceBusConnection, connection);
            return pipeline;
        }
    }
}
