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

#region using
using System;
using System.Linq;
using System.Collections.Concurrent;
using System.Collections.Generic;
#endregion
namespace Xigadee
{
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
        #region Constructors
        /// <summary>
        /// This is the empty constructor that sets the default settings with config settings set to 10.
        /// </summary>
        public ConfigBase() : this(10) { }
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public ConfigBase(int? appSettingsPriority = null)
        {
            //Set the resolver with the override collection.
            ResolversClear();

            PriorityAppSettings = appSettingsPriority;

            //Set the default app settings config resolver.
            if (PriorityAppSettings.HasValue)
                ResolverSet(PriorityAppSettings.Value, new ConfigResolverAppSettings());
        }
        #endregion

        #region OverrideSettings
        /// <summary>
        /// This property returns the override settings.
        /// </summary>
        public ConfigResolverMemory OverrideSettings { get { return this[int.MaxValue] as ConfigResolverMemory; } } 
        #endregion

        #region PriorityAppSettings
        /// <summary>
        /// This is the priority value for the AppSettings resolver. If null then AppSettings are not set.
        /// </summary>
        public int? PriorityAppSettings { get; } 
        #endregion

        #region Resolvers
        /// <summary>
        /// This property returns the resolvers currently set for the collection.
        /// </summary>
        public IEnumerable<KeyValuePair<int, ConfigResolver>> Resolvers
        {
            get
            {
                return mConfigResolvers;
            }
        }
        #endregion

        #region this[int? key]...
        /// <summary>
        /// This property setter allows for easy access to the resolvers.
        /// </summary>
        /// <param name="key">The key value. If null this returns the override settings.</param>
        /// <returns>Returns the config resolver.</returns>
        public ConfigResolver this[int key]
        {
            get
            {
                return mConfigResolvers[key];
            }
            set
            {
                ResolverSet(key, value);
            }
        } 
        #endregion

        #region ResolversClear()
        /// <summary>
        /// This method clears the resolvers from the collection.
        /// </summary>
        public void ResolversClear()
        {
            mConfigResolvers.Clear();
            //Set the default override resolver for manual settings that override the base settings.
            var resolver = new ConfigResolverMemory();
            mConfigResolvers.AddOrUpdate(int.MaxValue, resolver, (k, v) => resolver);
        }
        #endregion
        #region ResolverSet(int priority, ConfigResolverBase resolver)
        /// <summary>
        /// This method sets a new resolver in the config hierarchy.
        /// </summary>
        /// <param name="priority">The priorty level, which should be below int max.</param>
        /// <param name="resolver">The resolver.</param>
        public void ResolverSet(int priority, ConfigResolver resolver)
        {
            if (priority == int.MaxValue)
                throw new ArgumentOutOfRangeException("priority", "priority cannot be int max as that is a reserved value");
            if (resolver == null)
                throw new ArgumentNullException("resolver cannot be null");

            mConfigResolvers.AddOrUpdate(priority, resolver, (k, v) => resolver);
        } 
        #endregion

        #region Flush()
        /// <summary>
        /// This method flushes the cache of stored values.
        /// </summary>
        public void Flush()
        {
            mConfigCache.Clear();
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

            //This method scans the resolvers and gets the values based on priority.
            //Note that this value if cached so the overheads of doing this are only incurred once.
            lock (syncLock)
            {
                value = mConfigResolvers
                    .OrderByDescending((k) => k.Key)
                    .Where((k) => k.Value.CanResolve(key))
                    .Select((k) => k.Value.Resolve(key))
                    .FirstOrDefault();
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
