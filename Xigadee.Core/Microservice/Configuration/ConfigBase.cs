#region using
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
#endregion
namespace Xigadee
{
    public abstract class ConfigResolver
    {
        public abstract string Resolve(string key);
    }

    public class AppSettingsConfigResolver: ConfigResolver
    {
        public override string Resolve(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }

    /// <summary>
    /// This is the configuration base class for configuration. This class resolves settings from
    /// a number of sources, specifically the Azure configuration and the Windows configuration.
    /// You can also insert a resolver function that can intercept calls and allow for testing containers 
    /// to insert their own config.
    /// </summary>
    public class ConfigBase: IEnvironmentConfiguration
    {
        #region Declarations
        private object syncLock = new object();
        /// <summary>
        /// This is the configuration collection that holds the configuration keys.
        /// </summary>
        private readonly ConcurrentDictionary<string, string> mConfigCache = new ConcurrentDictionary<string, string>(); 
        /// <summary>
        /// This is the set of resolvers that are available to resolve the configuration settings for the application.
        /// </summary>
        private readonly ConcurrentDictionary<int, ConfigResolver> mConfigResolvers = new ConcurrentDictionary<int, ConfigResolver>();
        #endregion

        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public ConfigBase()
        {
            //Set the default config resolver.
            mConfigResolvers.AddOrUpdate(0, new AppSettingsConfigResolver(), (k,v) => v);
        }

        #region Flush()
        /// <summary>
        /// This method flushes the cache of stored values.
        /// </summary>
        public void Flush()
        {
            mConfigCache.Clear();
        }
        #endregion

        #region Resolver
        /// <summary>
        /// This is the resolves function that can be injected in to the config resolution flow.
        /// </summary>
        [Obsolete("ConfigResolver will replace this.", false)]
        public Func<string, string, string> Resolver
        {
            get; set;
        } 
        #endregion
        #region ResolverFirst
        /// <summary>
        /// This boolean property specifies whether the system should read from the internal collection
        /// or go straight to the azure/windows config setting.
        /// This ensures that we can insert new values during testing.
        /// </summary>
        [Obsolete("ConfigResolver will replace this.", false)]
        public bool ResolverFirst
        {
            get; set;
        } 
        #endregion

        #region PlatformOrConfig(string key)
        /// <summary>
        /// This method returns a string value from configuration. It tries to resolve values from 
        /// either the resolver function, or from the Azure configuration and then the windows configuration.
        /// </summary>
        /// <param name="key">The key to resolve.</param>
        /// <returns>The value or null if not resolved.</returns>
        protected virtual string PlatformOrConfig(string key)
        {
            string value = null;

            if (ResolverFirst)
                value = Resolver?.Invoke(key, null);

            if (value == null)
            {
                try
                {
                    value = ConfigurationManager.AppSettings[key];
                }
                catch (Exception ex)
                {
                    // Unable to retrieve from app settings
                }

                if (value == null && !ResolverFirst)
                    value = Resolver?.Invoke(key, null);
            }

            return value;
        } 
        #endregion

        #region PlatformOrConfigCache(string key, string defaultValue = null)
        /// <summary>
        /// This method resolves a specific value or insert the default value.
        /// </summary>
        /// <param name="key">The key to resolve.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>Returns the setting or the default.</returns>
        public string PlatformOrConfigCache(string key, string defaultValue = null)
        {
            string value = null;
            if (!mConfigCache.TryGetValue(key, out value))
            {
                value = mConfigCache.GetOrAdd(key, PlatformOrConfig(key) ?? defaultValue);
            }

            return value;
        } 
        #endregion

        #region PlatformOrConfigCacheBool(string key, string defaultValue = null)
        /// <summary>
        /// This method resolves a specific value or insert the default value for boolean properties.
        /// </summary>
        /// <param name="key">The key to resolve.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>Returns the setting or the default as boolean false.</returns>
        public virtual bool PlatformOrConfigCacheBool(string key, string defaultValue = null)
        {
            return Convert.ToBoolean(PlatformOrConfigCache(key, defaultValue));
        } 
        #endregion

        #region PlatformOrConfigCacheInt(string key, int? defaultValue = null)
        /// <summary>
        /// This method resolves a specific value or insert the default value for boolean properties.
        /// </summary>
        /// <param name="key">The key to resolve.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>Returns the setting or the default as boolean false.</returns>
        public virtual int PlatformOrConfigCacheInt(string key, int? defaultValue = null)
        {
            return Convert.ToInt32(PlatformOrConfigCache(key, defaultValue?.ToString()));
        } 
        #endregion

    }
}
