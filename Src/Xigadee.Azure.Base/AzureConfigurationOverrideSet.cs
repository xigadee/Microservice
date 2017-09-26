using System;

namespace Xigadee
{
    public static partial class AzureBaseHelper
    {
        /// <summary>
        /// This method adds an override setting for the Azure settings and clears the cache.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="keyEnum">The key shortcut.</param>
        /// <param name="value"></param>
        /// <returns>Returns the pipeline.</returns>
        public static P AzureConfigurationOverrideSet<P>(this P pipeline, AzureConfigShortcut keyEnum, string value)
        where P : IPipeline
        {
            string key = AzureConfigShortcutConvert(keyEnum);
            pipeline.Configuration.OverrideSettings.Add(key, value);
            pipeline.Configuration.CacheFlush();
            return pipeline;
        }

        /// <summary>
        /// This method adds an override setting for the Azure settings and clears the cache.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="keyEnum">The key shortcut.</param>
        /// <param name="value">The value function.</param>
        /// <returns>Returns the pipeline.</returns>
        public static P AzureConfigurationOverrideSet<P>(this P pipeline, AzureConfigShortcut keyEnum, Func<IEnvironmentConfiguration, string> value)
        where P : IPipeline
        {
            if (value == null)
                throw new ArgumentNullException("value", $"{nameof(AzureConfigurationOverrideSet)}: the value function cannot be null");
            return pipeline.AzureConfigurationOverrideSet(keyEnum, value(pipeline.Configuration));
        }

        /// <summary>
        /// This extension method returns the string equivalent for the enumeration shortcut.
        /// </summary>
        /// <param name="keyEnum">The key enum.</param>
        /// <returns>Returns the string equivalent used in the configuration settings.</returns>
        public static string ToSettingKey(this AzureConfigShortcut keyEnum)
        {
            return AzureConfigShortcutConvert(keyEnum);
        }
        /// <summary>
        /// Azures the configuration shortcut convert.
        /// </summary>
        /// <param name="keyEnum">The key enum.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">This is thrown if there are unsupported enumeration types.</exception>
        public static string AzureConfigShortcutConvert(AzureConfigShortcut keyEnum)
        {
            switch (keyEnum)
            {
                //Storage
                case AzureConfigShortcut.StorageAccountAccessKey:
                    return KeyStorageAccountAccessKey;
                case AzureConfigShortcut.StorageAccountName:
                    return KeyStorageAccountName;

                //Azure Storage
                case AzureConfigShortcut.AzureStorageAccountAccessKey:
                    return KeyAzureStorageAccountAccessKey;
                case AzureConfigShortcut.AzureStorageAccountName:
                    return KeyAzureStorageAccountName;

                //Logging
                case AzureConfigShortcut.LogStorageAccountName:
                    return KeyLogStorageAccountName;
                case AzureConfigShortcut.LogStorageAccountAccessKey:
                    return KeyLogStorageAccountAccessKey;

                //Redis
                case AzureConfigShortcut.RedisCacheConnection:
                    return KeyRedisCacheConnection;

                //Service Bus
                case AzureConfigShortcut.ServiceBusConnection:
                    return KeyServiceBusConnection;

                //EventHubs
                case AzureConfigShortcut.EventHubsConnection:
                    return KeyEventHubsConnection;

                //Application Insights
                case AzureConfigShortcut.ApplicationInsights:
                    return KeyApplicationInsights;
                case AzureConfigShortcut.ApplicationInsightsLoggingLevel:
                    return KeyApplicationInsightsLoggingLevel;

                //KeyVault
                case AzureConfigShortcut.KeyVaultClientId:
                    return KeyKeyVaultClientId;
                case AzureConfigShortcut.KeyVaultClientSecret:
                    return KeyKeyVaultClientSecret;
                case AzureConfigShortcut.KeyVaultSecretBaseUri:
                    return KeyKeyVaultSecretBaseUri;

                //Azure Table Storage config key
                case AzureConfigShortcut.AzureTableStorageConfigSASKey:
                    return KeyAzureTableStorageConfigSASKey;

                default:
                    throw new ArgumentOutOfRangeException($"{nameof(AzureConfigShortcutConvert)} - Azure Configuration Shortcut key is not supported: {keyEnum.ToString()}");
            }
        }
    }
}
