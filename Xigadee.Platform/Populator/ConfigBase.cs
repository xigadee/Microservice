#region using
using System;
using System.Collections.Generic;
using System.Configuration;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This is the configuration base class for configuration. This class resolves settings from
    /// a number of sources, specifically the Azure configuration and the Windows configuration.
    /// You can also insert a resolver function that can intercept calls and allow for testing containers 
    /// to insert their own config.
    /// </summary>
    public abstract class ConfigBase
    {
        private object syncLock = new object();
        /// <summary>
        /// This is the configuration collection that holds the configuration keys.
        /// </summary>
        private readonly Dictionary<string, string> mConfig = new Dictionary<string, string>();
        /// <summary>
        /// This boolean property specifies whether the system should read from the internal collection
        /// or go straight to the azure/windows config setting.
        /// This ensures that we can insert new values during testing.
        /// </summary>
        public bool ResolverFirst
        {
            get; set;
        }
        /// <summary>
        /// This method returns a string value from configuration. It tries to resolve values from 
        /// either the resolver function, or from the Azure configuration and then the windows configuration.
        /// </summary>
        /// <param name="key">The key to resolve.</param>
        /// <returns>The value or null if not resolved.</returns>
        protected virtual string PlatformOrConfig(string key)
        {
            string value = ResolverFirst && Resolver != null ? Resolver(key, null) : null;
            if (value != null)
                return value;

            try
            {
                value = value ?? ConfigurationManager.AppSettings[key];
            }
            catch (Exception ex)
            {
                // Unable to retrieve from app settings
            }

            if (Resolver != null)
                value = Resolver(key, value);

            return value;
        }
        /// <summary>
        /// This method resolves a specific value or insert the default value.
        /// </summary>
        /// <param name="key">The key to resolve.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>Returns the setting or the default.</returns>
        protected string PlatformOrConfigCache(string key, string defaultValue = null)
        {
            lock (syncLock)
            {
                if (!mConfig.ContainsKey(key))
                {
                    mConfig.Add(key, PlatformOrConfig(key) ?? defaultValue);
                }

                return mConfig[key];
            }

        }
        /// <summary>
        /// This method resolves a specific value or insert the default value for boolean properties.
        /// </summary>
        /// <param name="key">The key to resolve.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>Returns the setting or the default as boolean false.</returns>
        protected virtual bool PlatformOrConfigCacheBool(string key, string defaultValue = null)
        {
            lock (syncLock)
            {
                if (!mConfig.ContainsKey(key))
                {
                    mConfig.Add(key, PlatformOrConfig(key) ?? defaultValue);
                }

                return Convert.ToBoolean(mConfig[key]);
            }
        }

        /// <summary>
        /// This method resolves a specific value or insert the default value for boolean properties.
        /// </summary>
        /// <param name="key">The key to resolve.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>Returns the setting or the default as boolean false.</returns>
        protected virtual int PlatformOrConfigCacheInt(string key, int? defaultValue = null)
        {
            lock (syncLock)
            {
                if (!mConfig.ContainsKey(key))
                {
                    mConfig.Add(key, PlatformOrConfig(key) ?? defaultValue.ToString());
                }

                return Convert.ToInt32(mConfig[key]);
            }
        }

        /// <summary>
        /// This is the resolves function that can be injected in to the config resolution flow.
        /// </summary>
        public Func<string, string, string> Resolver
        {
            get; set;
        }
    }
}
