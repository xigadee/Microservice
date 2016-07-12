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
        [ConfigKeySettingName]
        public const string KeyStorageAccountName = "StorageAccountName";
        [ConfigKeySettingName]
        public const string KeyStorageAccountAccessKey = "StorageAccountAccessKey";

        public static string StorageAccountName(this ConfigBase config) => config.PlatformOrConfigCache(KeyStorageAccountName);

        public static string StorageAccountAccessKey(this ConfigBase config) => config.PlatformOrConfigCache(KeyStorageAccountAccessKey);

        public static StorageCredentials StorageCredentials(this ConfigBase config)
        {
            if (string.IsNullOrEmpty(config.StorageAccountName()) || string.IsNullOrEmpty(config.StorageAccountAccessKey()))
                return null;

            return new StorageCredentials(config.StorageAccountName(), config.StorageAccountAccessKey());
        }
    }
}
