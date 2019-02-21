using Microsoft.WindowsAzure.Storage.Auth;

namespace Xigadee
{
    public static partial class AzureStorageExtensionMethods
    {
        /// <summary>
        /// The reserved keyword.
        /// </summary>
        public const string Storage = "Storage";

        /// <summary>
        /// The storage account connection key
        /// </summary>
        [ConfigSettingKey(Storage)]
        public const string KeyStorageAccountName = "StorageAccountName";
        /// <summary>
        /// The storage account access key
        /// </summary>
        [ConfigSettingKey(Storage)]
        public const string KeyStorageAccountAccessKey = "StorageAccountAccessKey";
        /// <summary>
        /// The storage account connection value.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <returns></returns>
        [ConfigSetting(Storage)]
        public static string StorageAccountName(this IEnvironmentConfiguration config) => config.PlatformOrConfigCache(KeyStorageAccountName);
        /// <summary>
        /// The storage account access key value
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <returns></returns>
        [ConfigSetting(Storage)]
        public static string StorageAccountAccessKey(this IEnvironmentConfiguration config) => config.PlatformOrConfigCache(KeyStorageAccountAccessKey);


        /// <summary>
        /// The storage credentials.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <returns>The credentials</returns>
        [ConfigSetting(Storage)]
        public static StorageCredentials StorageCredentials(this IEnvironmentConfiguration config)
        {
            if (string.IsNullOrEmpty(config.StorageAccountName()) || string.IsNullOrEmpty(config.StorageAccountAccessKey()))
                return null;

            return new StorageCredentials(config.StorageAccountName(), config.StorageAccountAccessKey());
        }

        /// <summary>
        /// This extension allows the Azure storage extensions to be manually set as override parameters.
        /// </summary>
        /// <param name="pipeline">The incoming pipeline.</param>
        /// <param name="storageAccountName">The storage account name.</param>
        /// <param name="storageAccountAccessKey">The storage account key.</param>
        /// <returns>The pass-through of the pipeline.</returns>
        public static P ConfigOverrideSetAzureStorage<P>(this P pipeline, string storageAccountName, string storageAccountAccessKey)
            where P : IPipeline
        {
            pipeline.ConfigurationOverrideSet(KeyStorageAccountName, storageAccountName);
            pipeline.ConfigurationOverrideSet(KeyStorageAccountAccessKey, storageAccountAccessKey);
            return pipeline;
        }
    }
}
