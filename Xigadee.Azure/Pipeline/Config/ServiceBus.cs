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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Auth;

namespace Xigadee
{
    public static partial class AzureExtensionMethods
    {
        [ConfigSettingKey("ServiceBus")]
        public const string KeyServiceBusConnection = "ServiceBusConnection";

        [ConfigSetting("ServiceBus")]
        public static string ServiceBusConnection(this IEnvironmentConfiguration config) => config.PlatformOrConfigCache(KeyServiceBusConnection);

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
                throw new AzureConnectionException("Service bus connection string cannot be resolved. Please check the config settings has been set.");

            return conn;
        }
        #endregion
    }
}
