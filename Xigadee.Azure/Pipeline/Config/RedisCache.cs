using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Auth;

namespace Xigadee
{
    public static partial class AzureExtensionMethods
    {
        [ConfigSettingKey("RedisCache")]
        public const string KeyRedisCacheConnection = "RedisCacheConnection";

        [ConfigSetting("RedisCache")]
        public static string RedisCacheConnection(this IEnvironmentConfiguration config) => config.PlatformOrConfigCache(KeyRedisCacheConnection);
    }
}
