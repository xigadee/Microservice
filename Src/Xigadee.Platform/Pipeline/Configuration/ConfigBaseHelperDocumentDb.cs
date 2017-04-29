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

namespace Xigadee
{

    public static class ConfigBaseHelperDocumentDb
    {
        /// <summary>
        /// A configuration key shortcut.
        /// </summary>
        [ConfigSettingKey("DocumentDb")]
        public const string KeyDocDBAccountName = "DocDBAccountName";
        /// <summary>
        /// A configuration key shortcut.
        /// </summary>
        [ConfigSettingKey("DocumentDb")]
        public const string KeyDocDBAccountAccessKey = "DocDBAccountAccessKey";
        /// <summary>
        /// A configuration key shortcut.
        /// </summary>
        [ConfigSettingKey("DocumentDb")]
        public const string KeyDocDBDatabaseName = "DocDBDatabaseName";
        /// <summary>
        /// A configuration key shortcut.
        /// </summary>
        [ConfigSettingKey("DocumentDb")]
        public const string KeyDocDBCollectionName = "DocDBCollectionName";

        /// <summary>
        /// This is the config reserved id for the DocumentDb Connection string
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <param name="throwExceptionIfNotFound">Throw and exception if the key is not found. The default is false.</param>
        /// <returns>Returns the config value.</returns>
        [ConfigSetting("DocumentDb")]
        public static DocumentDbConnection DocDBConnection(this IEnvironmentConfiguration config, bool throwExceptionIfNotFound = false) 
            => DocumentDbConnection.ToConnection(config.DocDBAccountName(throwExceptionIfNotFound), config.DocDBAccountAccessKey(throwExceptionIfNotFound));
        /// <summary>
        /// This is the config reserved id for the DocumentDb account name.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <param name="throwExceptionIfNotFound">Throw and exception if the key is not found. The default is false.</param>
        /// <returns>Returns the config value.</returns>
        [ConfigSetting("DocumentDb")]
        public static string DocDBAccountName(this IEnvironmentConfiguration config, bool throwExceptionIfNotFound = false) 
            => config.PlatformOrConfigCache(KeyDocDBAccountName, throwExceptionIfNotFound: throwExceptionIfNotFound);
        /// <summary>
        /// This is the config reserved id for the DocumentDb account access key.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <param name="throwExceptionIfNotFound">Throw and exception if the key is not found. The default is false.</param>
        /// <returns>Returns the config value.</returns>
        [ConfigSetting("DocumentDb")]
        public static string DocDBAccountAccessKey(this IEnvironmentConfiguration config, bool throwExceptionIfNotFound = false) 
            => config.PlatformOrConfigCache(KeyDocDBAccountAccessKey, throwExceptionIfNotFound: throwExceptionIfNotFound);
        /// <summary>
        /// This is the config reserved id for the DocumentDb default database name.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <param name="throwExceptionIfNotFound">Throw and exception if the key is not found. The default is false.</param>
        /// <returns>Returns the config value.</returns>
        [ConfigSetting("DocumentDb")]
        public static string DocDBDatabaseName(this IEnvironmentConfiguration config, bool throwExceptionIfNotFound = false) 
            => config.PlatformOrConfigCache(KeyDocDBDatabaseName, throwExceptionIfNotFound: throwExceptionIfNotFound);
        /// <summary>
        /// This is the config reserved id for the DocumentDb default collection name.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <param name="throwExceptionIfNotFound">Throw and exception if the key is not found. The default is false.</param>
        /// <returns>Returns the config value.</returns>
        [ConfigSetting("DocumentDb")]
        public static string DocDBCollectionName(this IEnvironmentConfiguration config, bool throwExceptionIfNotFound = false) 
            => config.PlatformOrConfigCache(KeyDocDBCollectionName, throwExceptionIfNotFound: throwExceptionIfNotFound);
    }
}
