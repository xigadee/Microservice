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
        /// <summary>
        /// The reserved keyword.
        /// </summary>
        public const string KeyVault = "KeyVault";

        /// <summary>
        /// The reserved keyword for the KeyVault client identifier
        /// </summary>
        [ConfigSettingKey(KeyVault)]
        public const string KeyKeyVaultClientId = "KeyVaultClientId";
        /// <summary>
        /// The reserved keyword for the KeyVault client secret
        /// </summary>
        [ConfigSettingKey(KeyVault)]
        public const string KeyKeyVaultClientSecret = "KeyVaultClientSecret";
        /// <summary>
        /// The reserved keyword for the KeyVault secret base URI
        /// </summary>
        [ConfigSettingKey(KeyVault)]
        public const string KeyKeyVaultSecretBaseUri = "KeyVaultSecretBaseUri";
        /// <summary>
        /// Retrieves the KeyVault client identifier.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <returns>The client id value.</returns>
        [ConfigSetting(KeyVault)]
        public static string KeyVaultClientId(this IEnvironmentConfiguration config) => config.PlatformOrConfigCache(KeyKeyVaultClientId);
        /// <summary>
        /// Retrieves the KeyVault client secret.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <returns>The client secret.</returns>
        [ConfigSetting(KeyVault)]
        public static string KeyVaultClientSecret(this IEnvironmentConfiguration config) => config.PlatformOrConfigCache(KeyKeyVaultClientSecret);
        /// <summary>
        /// Retrieves the KeyVault secret base URI.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <returns>The Uri.</returns>
        [ConfigSetting(KeyVault)]
        public static string KeyVaultSecretBaseUri(this IEnvironmentConfiguration config) => config.PlatformOrConfigCache(KeyKeyVaultSecretBaseUri);

    }
}
