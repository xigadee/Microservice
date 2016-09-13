#region Copyright
// Copyright Hitachi Consulting
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

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
        [ConfigSettingKey("Storage")]
        public const string KeyStorageAccountName = "StorageAccountName";
        [ConfigSettingKey("Storage")]
        public const string KeyStorageAccountAccessKey = "StorageAccountAccessKey";

        [ConfigSetting("Storage")]
        public static string StorageAccountName(this IEnvironmentConfiguration config) => config.PlatformOrConfigCache(KeyStorageAccountName);

        [ConfigSetting("Storage")]
        public static string StorageAccountAccessKey(this IEnvironmentConfiguration config) => config.PlatformOrConfigCache(KeyStorageAccountAccessKey);

        [ConfigSetting("Storage")]
        public static StorageCredentials StorageCredentials(this IEnvironmentConfiguration config)
        {
            if (string.IsNullOrEmpty(config.StorageAccountName()) || string.IsNullOrEmpty(config.StorageAccountAccessKey()))
                return null;

            return new StorageCredentials(config.StorageAccountName(), config.StorageAccountAccessKey());
        }
    }
}
