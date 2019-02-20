using System;

namespace Xigadee
{
    /// <summary>
    /// This exception is raised when a required configuration key cannot be found.
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class ConfigurationMissingSettingException:Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationMissingSettingException"/> class.
        /// </summary>
        /// <param name="missingKey">The missing key.</param>
        /// <param name="message">The error message.</param>
        public ConfigurationMissingSettingException(string missingKey, string message):base(message)
        {

        }
        /// <summary>
        /// Gets the missing key.
        /// </summary>
        public string MissingKey { get; }
    }
}
