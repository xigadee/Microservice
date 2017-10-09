using System;

namespace Xigadee
{
    /// <summary>
    /// This exception is thrown if Azure components are specified, but the pipeline configuration connection string has not been set.
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class AzureServiceBusConnectionException: ConfigurationMissingSettingException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AzureConnectionException"/> class.
        /// </summary>
        public AzureServiceBusConnectionException(string key):base(key, $"The Azure ServiceBus connection string '{key}' is null or empty. Please check the config settings has been set.")
        {
        }
    }
}
