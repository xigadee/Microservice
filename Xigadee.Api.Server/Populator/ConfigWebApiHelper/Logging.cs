using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Auth;

namespace Xigadee
{
    public static class ConfigWebApiHelperLogging
    {
        [ConfigSettingKey("WebApiLogging")]
        public const string KeyWebLogFilterLevels = "WebLogFilterLevels";
        /// <summary>
        /// Blob logging filter level
        /// </summary>
        [ConfigSetting("WebApiLogging")]
        public static IList<string> WebLogFilterLevels(this ConfigWebApi config) => config.PlatformOrConfigCache(KeyWebLogFilterLevels, "All")?.Split(',').ToList() ?? new List<string>();
    }
}
