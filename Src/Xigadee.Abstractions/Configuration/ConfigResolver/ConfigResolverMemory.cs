#region using
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This class holds the manual override settings.
    /// </summary>
    public class ConfigResolverMemory: ConfigResolver
    {
        private ConcurrentDictionary<string, string> mSettings = new ConcurrentDictionary<string, string>();

        /// <summary>
        /// This method manually adds a key to the collection.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The string value to add.</param>
        public virtual void Add(string key, string value)
        {
            mSettings.AddOrUpdate(key, value, (a,b) => value);
        }

        /// <summary>
        /// This method clears the memory cache.
        /// </summary>
        public virtual void Clear()
        {
            mSettings.Clear();
        }

        /// <summary>
        /// This method validates whether the key can be resolved.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Returns true if the key is resolved.</returns>
        public override bool CanResolve(string key)
        {
            return mSettings.ContainsKey(key);
        }

        /// <summary>
        /// This method resolves the key value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>This is the settings value, null if not set.</returns>
        public override string Resolve(string key)
        {
            if (mSettings.ContainsKey(key))
                return mSettings[key];

            return null;
        }
    }
}
