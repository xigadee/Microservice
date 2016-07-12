using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Auth;

namespace Xigadee
{
    public static class ConfigBaseHelperRedis
    {
        public const string KeyEntityRedisCacheConnection = "EntityRedisCacheConnection";

        public static string EntityRedisCacheConnection(this ConfigBase config) => config.PlatformOrConfigCache(KeyEntityRedisCacheConnection);
    }
}
