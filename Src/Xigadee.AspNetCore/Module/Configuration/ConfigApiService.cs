using System.Collections.Generic;

namespace Xigadee
{
    /// <summary>
    /// This is the root configuration service.
    /// </summary>
    public class ConfigApiService
    {
        /// <summary>
        /// Gets or sets the service logging name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the user-security module mode 
        /// </summary>
        public ConfigModuleMode UserSecurity { get; set; }

        /// <summary>
        /// Gets or sets the secret module mode 
        /// </summary>
        public ConfigModuleMode Secret { get; set; }

        /// <summary>
        /// Gets or sets the certificate module mode 
        /// </summary>
        public ConfigModuleMode Certificate { get; set; }

        /// <summary>
        /// Gets or sets the connections for the application.
        /// </summary>
        public Dictionary<string, string> Connections { get; set; }
    }

    /// <summary>
    /// This class is used to add helper functionality to the settings class.
    /// </summary>
    public static class ConfigApiServiceHelper
    {
        /// <summary>
        /// Resolves a setting against a connection.
        /// </summary>
        /// <param name="service">The service settings.</param>
        /// <param name="setting">The module mode.</param>
        /// <returns>Returns the connection string, or null.</returns>
        public static string ConnectionResolve(this ConfigApiService service, ConfigModuleMode setting)
        {
            string value = null;

            if (!string.IsNullOrEmpty(setting?.Connection))
                service.Connections.TryGetValue(setting.Connection, out value);

            return value;
        }
    }
}
