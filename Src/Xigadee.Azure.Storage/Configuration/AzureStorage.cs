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

using Microsoft.WindowsAzure.Storage.Auth;

namespace Xigadee
{
    public static partial class AzureStorageExtensionMethods
    {
        /// <summary>
        /// Returns the Azure Storage credentials.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <param name="throwExceptionIfNotFound">if set to <c>true</c> [throw exception if not found].</param>
        /// <returns>The Azure Storage container credential.</returns>
        [ConfigSetting("AzureStorage")]
        public static StorageCredentials AzureStorageCredentials(this IEnvironmentConfiguration config, bool throwExceptionIfNotFound = true)
        {
            if (string.IsNullOrEmpty(config.AzureStorageAccountName(throwExceptionIfNotFound)) 
                || string.IsNullOrEmpty(config.AzureStorageAccountAccessKey(throwExceptionIfNotFound)))
                    return null;

            return new StorageCredentials(config.AzureStorageAccountName(), config.AzureStorageAccountAccessKey());
        }
    }
}
