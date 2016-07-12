using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public static class ConfigBaseHelperService
    {
        public static bool ServiceDisabled(this ConfigBase config) => config.PlatformOrConfigCacheBool("ServiceDisabled");

        public static string Client(this ConfigBase config) => config.PlatformOrConfigCache("Client");

        public static string Environment(this ConfigBase config) => config.PlatformOrConfigCache("Environment");
    }
}
