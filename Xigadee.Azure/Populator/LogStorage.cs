using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Auth;

namespace Xigadee
{
    public static class ConfigBaseHelperLogStorage
    {
        [ConfigSettingKey("LogStorage")]
        public const string KeyLogStorageAccountName = "LogStorageAccountName";
        [ConfigSettingKey("LogStorage")]
        public const string KeyLogStorageAccountAccessKey = "LogStorageAccountAccessKey";


        [ConfigSetting("LogStorage")]
        public static string LogStorageAccountName(this IEnvironmentConfiguration config) => config.PlatformOrConfigCache(KeyLogStorageAccountName, config.StorageAccountName());

        [ConfigSetting("LogStorage")]
        public static string LogStorageAccountAccessKey(this IEnvironmentConfiguration config) => config.PlatformOrConfigCache(KeyLogStorageAccountAccessKey, config.StorageAccountAccessKey());

        [ConfigSetting("LogStorage")]
        public static StorageCredentials LogStorageCredentials(this IEnvironmentConfiguration config)
        {
            if (string.IsNullOrEmpty(config.LogStorageAccountName()) || string.IsNullOrEmpty(config.LogStorageAccountAccessKey()))
                return config.StorageCredentials();

            return new StorageCredentials(config.LogStorageAccountName(), config.LogStorageAccountAccessKey());
        }
    }
}
