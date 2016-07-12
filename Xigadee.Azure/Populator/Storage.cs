using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Auth;

namespace Xigadee
{
    public static class ConfigBaseHelperStorage
    {
        [ConfigSettingKey("Storage")]
        public const string KeyStorageAccountName = "StorageAccountName";
        [ConfigSettingKey("Storage")]
        public const string KeyStorageAccountAccessKey = "StorageAccountAccessKey";

        [ConfigSetting("Storage")]
        public static string StorageAccountName(this ConfigBase config) => config.PlatformOrConfigCache(KeyStorageAccountName);

        [ConfigSetting("Storage")]
        public static string StorageAccountAccessKey(this ConfigBase config) => config.PlatformOrConfigCache(KeyStorageAccountAccessKey);

        [ConfigSetting("Storage")]
        public static StorageCredentials StorageCredentials(this ConfigBase config)
        {
            if (string.IsNullOrEmpty(config.StorageAccountName()) || string.IsNullOrEmpty(config.StorageAccountAccessKey()))
                return null;

            return new StorageCredentials(config.StorageAccountName(), config.StorageAccountAccessKey());
        }
    }
}
