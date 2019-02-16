using System;

namespace Xigadee
{
    /// <summary>
    /// This exception is throw if a required key is missing.
    /// </summary>
    public class ConfigAssertKeyNotPresentException: Exception
    {
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="message">The error message</param>
        public ConfigAssertKeyNotPresentException(string key, string message) : base($"Required config key not found '{key}': {message}")
        {
            Key = key;
        }

        /// <summary>
        /// This is the key that the exception was thrown for.
        /// </summary>
        public string Key { get; }
    }
}
