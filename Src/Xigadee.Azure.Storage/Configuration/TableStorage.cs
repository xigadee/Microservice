namespace Xigadee
{
    public static partial class AzureStorageExtensionMethods
    {
        /// <summary>
        /// This is the key definition for the table storage config holder.
        /// </summary>
        [ConfigSettingKey("TableStorage")]
        public const string KeyAzureTableStorageConfigSASKey = "AzureTableStorageConfigSASKey";

        /// <summary>
        /// This shortcut setting can be used to resolve the SAS key.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <returns>Returns the string.</returns>
        [ConfigSetting("TableStorage")]
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
