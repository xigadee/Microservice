using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public static class ConfigBaseHelperService
    {
        public const string KeyEnvironment = "Environment";
        public const string KeyClient = "Client";
        public const string KeyServiceDisabled = "ServiceDisabled";

        public static bool ServiceDisabled(this ConfigBase config) => config.PlatformOrConfigCacheBool(KeyServiceDisabled);

        public static string Client(this ConfigBase config) => config.PlatformOrConfigCache(KeyClient);

        public static string Environment(this ConfigBase config) => config.PlatformOrConfigCache(KeyEnvironment);

    }

}
