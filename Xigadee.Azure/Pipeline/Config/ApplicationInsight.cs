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

        [ConfigSettingKey("ApplicationInsights")]
        public const string KeyApplicationInsights = "ApplicationInsightsKey";

        [ConfigSetting("ApplicationInsights")]
        public static string ApplicationInsightsKey(this IEnvironmentConfiguration config) => config.PlatformOrConfigCache(KeyApplicationInsights);

    }
}
