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
                throw new ArgumentNullException("Service bus connection string cannot be resolved. Please check the config settings has been set.");

            return conn;
        }
        #endregion
    }
}
