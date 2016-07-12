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
        public const string KeyLogStorageAccountName = "LogStorageAccountName";
        public const string KeyLogStorageAccountAccessKey = "LogStorageAccountAccessKey";


        public static string LogStorageAccountName(this ConfigBase config) => config.PlatformOrConfigCache(KeyLogStorageAccountName, config.StorageAccountName());

        public static string LogStorageAccountAccessKey(this ConfigBase config) => config.PlatformOrConfigCache(KeyLogStorageAccountAccessKey, config.StorageAccountAccessKey());

        public static StorageCredentials LogStorageCredentials(this ConfigBase config)
        {
            if (string.IsNullOrEmpty(config.LogStorageAccountName()) || string.IsNullOrEmpty(config.LogStorageAccountAccessKey()))
                return config.StorageCredentials();

            return new StorageCredentials(config.LogStorageAccountName(), config.LogStorageAccountAccessKey());
        }
    }
}
