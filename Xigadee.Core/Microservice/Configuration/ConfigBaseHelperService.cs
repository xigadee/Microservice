using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public static class ConfigBaseHelperService
    {
        [ConfigSettingKey("Service")]
        public const string KeyEnvironment = "Environment";
        [ConfigSettingKey("Service")]
        public const string KeyClient = "Client";
        [ConfigSettingKey("Service")]
        public const string KeyServiceDisabled = "ServiceDisabled";

        [ConfigSetting("Service")]
        public static bool ServiceDisabled(this IEnvironmentConfiguration config) => config.PlatformOrConfigCacheBool(KeyServiceDisabled);

        [ConfigSetting("Service")]
        public static string Client(this IEnvironmentConfiguration config) => config.PlatformOrConfigCache(KeyClient);

        [ConfigSetting("Service")]
        public static string Environment(this IEnvironmentConfiguration config) => config.PlatformOrConfigCache(KeyEnvironment);

    }

}
