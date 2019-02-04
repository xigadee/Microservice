using System.Collections.Generic;

namespace Xigadee
{
    /// <summary>
    /// This interface is used by extension method that provide shortcuts for specific key/value pairs
    /// from the environment configuration.
    /// </summary>
    public interface IEnvironmentConfiguration
    {
        ConfigResolverMemory OverrideSettings { get; } 

        ConfigResolver this[int key] { get; set; }

        int? PriorityAppSettings { get; }

        void ResolversClear();

        void ResolverSet(int priority, ConfigResolver resolver);

        IEnumerable<KeyValuePair<int, ConfigResolver>> Resolvers{get;}

        string PlatformOrConfigCache(string key, string defaultValue = null, bool throwExceptionIfNotFound = false);

        bool PlatformOrConfigCacheBool(string key, string defaultValue = null, bool throwExceptionIfNotFound = false);

        int PlatformOrConfigCacheInt(string key, int? defaultValue = default(int?), bool throwExceptionIfNotFound = false);

        void CacheFlush();

        /// <summary>
        /// This property returns true if all the keys can be resolved.
        /// </summary>
        /// <param name="keys">The collection of keys to resolve.</param>
        /// <returns>Returns true if all keys are resolved.</returns>
        bool CanResolve(params string[] keys);
    }
}