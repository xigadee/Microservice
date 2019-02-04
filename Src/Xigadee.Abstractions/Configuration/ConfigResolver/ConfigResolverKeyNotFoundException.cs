using System;
namespace Xigadee
{
    /// <summary>
    /// THis exception is thrown when a key is not found when throwExceptionIfNotFound is set to true.
    /// </summary>
    public class ConfigResolverKeyNotFoundException:Exception
    {
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="key">The key that could not be resolved.</param>
        public ConfigResolverKeyNotFoundException(string key):base($"The configuration key '{key}' cannot be resolved.")
        {
            Key = key;
        }

        /// <summary>
        /// The missing key.
        /// </summary>
        public string Key { get; }
    }
}
