using System;

namespace Xigadee
{
    /// <summary>
    /// This attribute is used to specify a class that should be registered as a singleton for the application.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ConfigurationSetAttribute : Attribute
    {
        /// <summary>
        /// This attribute is used to specify that the configuration class should be created automatically and bound to the key specified..
        /// </summary>
        /// <param name="settingsKey">The derived type to register for the singleton. Leave this blank if you do not wish to cast.</param>
        public ConfigurationSetAttribute(string settingsKey = null)
        {
            SettingsKey = settingsKey;
        }

        /// <summary>
        /// This is the registration type for the Singelton.
        /// </summary>
        public string SettingsKey { get; }
    }

    /// <summary>
    /// This attribute is used to specify that the configuration type should not be supported.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class DoNotConfigurationSetAttribute : Attribute
    {
        /// <summary>
        /// You can specify a cast type that should be registered instead of the parameter or property return type.
        /// If this method cannot cast to this type, you will get an exception during runtime.
        /// </summary>
        public DoNotConfigurationSetAttribute()
        {
        }

    }
}
