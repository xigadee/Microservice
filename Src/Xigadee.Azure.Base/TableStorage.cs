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

namespace Xigadee
{
    public static partial class AzureExtensionMethods
    {
        public const string TableStorage = "TableStorage";

        /// <summary>
        /// This is the key definition for the table storage config holder.
        /// </summary>
        [ConfigSettingKey(TableStorage)]
        public const string KeyAzureTableStorageConfigSASKey = "AzureTableStorageConfigSASKey";

        /// <summary>
        /// This shortcut setting can be used to resolve the SAS key.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <returns>Returns the string.</returns>
        [ConfigSetting(TableStorage)]
        public static string AzureTableStorageConfigSASKey(this IEnvironmentConfiguration config) => config.PlatformOrConfigCache(KeyAzureTableStorageConfigSASKey);
        /// <summary>
        /// The Azure table storage configuration default partition key, currently "config"
        /// </summary>
        public const string AzureTableStorageConfigDefaultPartitionKey = "config";
        /// <summary>
        /// The Azure table storage configuration default property key, currently "Value"
        /// </summary>
        public const string AzureTableStorageConfigDefaultPropertyKey = "Value";
        /// <summary>
        /// The Azure table storage configuration default table name, currently "Configuration"
        /// </summary>
        public const string AzureTableStorageConfigDefaultTableName = "Configuration";
        /// <summary>
        /// The Azure table storage configuration default priority, currently 30.
        /// </summary>
        public const int AzureTableStorageConfigDefaultPriority = 30;

    }
}
