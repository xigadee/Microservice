using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Auth;

namespace Xigadee
{
    public static class ConfigBaseHelperServiceBus
    {
        [ConfigKeySettingName]
        public const string KeyServiceBusConnection = "ServiceBusConnection";

        public static string ServiceBusConnection(this ConfigBase config) => config.PlatformOrConfigCache(KeyServiceBusConnection);
    }
}
