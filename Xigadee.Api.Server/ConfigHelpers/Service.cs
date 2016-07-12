using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Auth;

namespace Xigadee
{
    public static class ConfigBaseHelperService
    {
        /// <summary>
        /// Blob logging filter level
        /// </summary>
        public static IList<string> WebLogFilterLevels(this ConfigBase config) => config.PlatformOrConfigCache("WebLogFilterLevels", "All")?.Split(',').ToList() ?? new List<string>();

        public static bool ServiceDisabled(this ConfigBase config) => config.PlatformOrConfigCacheBool("ServiceDisabled");

        public static string Client(this ConfigBase config) => config.PlatformOrConfigCache("Client");

        public static string Environment(this ConfigBase config) => config.PlatformOrConfigCache("Environment");
    }
}
