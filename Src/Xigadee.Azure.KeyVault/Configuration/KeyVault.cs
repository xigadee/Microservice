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
        [ConfigSetting(KeyVault)]
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
            pipeline.ConfigurationOverrideSet(KeyKeyVaultClientId, keyVaultClientId);
            pipeline.ConfigurationOverrideSet(KeyKeyVaultClientSecret, keyVaultClientSecret);
            pipeline.ConfigurationOverrideSet(KeyKeyVaultSecretBaseUri, keyVaultSecretBaseUri);
            return pipeline;
        }

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
