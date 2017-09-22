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

using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace Xigadee
{
    public static partial class AzureKeyVaultExtensionMethods
    {
        /// <summary>
        /// Retrieves the KeyVault client credential.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <returns>The client credentials.</returns>
        [ConfigSetting("KeyVault")]
        public static ClientCredential KeyVaultClientCredential(this IEnvironmentConfiguration config)
        {
            if (string.IsNullOrEmpty(config.KeyVaultClientId()) || string.IsNullOrEmpty(config.KeyVaultClientSecret()))
                return null;

            return new ClientCredential(config.KeyVaultClientId(), config.KeyVaultClientSecret());
        }

        /// <summary>
        /// This extension allows the Key Vault connection values to be manually set as override parameters.
        /// </summary>
        /// <param name="pipeline">The incoming pipeline.</param>
        /// <param name="keyVaultClientId">The Key Vault client Id.</param>
        /// <param name="keyVaultClientSecret">The Key Vault client secret.</param>
        /// <param name="keyVaultSecretBaseUri">The Key Vault secret base Uri.</param>
        /// <returns>The pass-through of the pipeline.</returns>
        public static P ConfigOverrideSetKeyVaultConnection<P>(this P pipeline, string keyVaultClientId, string keyVaultClientSecret, string keyVaultSecretBaseUri)
            where P : IPipeline
        {
            pipeline.ConfigurationOverrideSet(AzureExtensionMethods.KeyKeyVaultClientId, keyVaultClientId);
            pipeline.ConfigurationOverrideSet(AzureExtensionMethods.KeyKeyVaultClientSecret, keyVaultClientSecret);
            pipeline.ConfigurationOverrideSet(AzureExtensionMethods.KeyKeyVaultSecretBaseUri, keyVaultSecretBaseUri);
            return pipeline;
        }
    }
}
