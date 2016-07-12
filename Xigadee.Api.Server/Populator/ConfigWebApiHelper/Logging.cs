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
        /// <summary>
        /// Blob logging filter level
        /// </summary>
        public static IList<string> WebLogFilterLevels(this ConfigWebApi config) => config.PlatformOrConfigCache("WebLogFilterLevels", "All")?.Split(',').ToList() ?? new List<string>();
    }
}
