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
        public static string LogStorageAccountName(this ConfigBase config) => config.PlatformOrConfigCache("LogStorageAccountName", config.StorageAccountName());

        public static string LogStorageAccountAccessKey(this ConfigBase config) => config.PlatformOrConfigCache("LogStorageAccountAccessKey", config.StorageAccountAccessKey());

        public static StorageCredentials LogStorageCredentials(this ConfigBase config)
        {
            if (string.IsNullOrEmpty(config.LogStorageAccountName()) || string.IsNullOrEmpty(config.LogStorageAccountAccessKey()))
                return config.StorageCredentials();

            return new StorageCredentials(config.LogStorageAccountName(), config.LogStorageAccountAccessKey());
        }


        public static string StorageAccountName(this ConfigBase config) => config.PlatformOrConfigCache("StorageAccountName");

        public static string StorageAccountAccessKey(this ConfigBase config) => config.PlatformOrConfigCache("StorageAccountAccessKey");

        public static StorageCredentials StorageCredentials(this ConfigBase config)
        {
            if (string.IsNullOrEmpty(config.StorageAccountName()) || string.IsNullOrEmpty(config.StorageAccountAccessKey()))
                return null;

            return new StorageCredentials(config.StorageAccountName(), config.StorageAccountAccessKey());
        }
    }
}
