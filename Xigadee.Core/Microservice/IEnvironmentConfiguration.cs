using System;

namespace Xigadee
{
    public interface IEnvironmentConfiguration
    {
        Func<string, string, string> Resolver { get; set; }
        bool ResolverFirst { get; set; }

        string PlatformOrConfigCache(string key, string defaultValue = null);
        bool PlatformOrConfigCacheBool(string key, string defaultValue = null);
        int PlatformOrConfigCacheInt(string key, int? defaultValue = default(int?));
    }
}