namespace Xigadee
{
    /// <summary>
    /// This enumeration is used to add a shortcut method to add azure config override settings.
    /// </summary>
    public enum AzureStorageConfigShortcut
    {
        /// <summary>
        /// The storage account name
        /// </summary>
        StorageAccountName,
        /// <summary>
        /// The storage account access key
        /// </summary>
        StorageAccountAccessKey,

        /// <summary>
        /// The Azure storage account name
        /// </summary>
        AzureStorageAccountName,
        /// <summary>
        /// The Azure storage account access key
        /// </summary>
        AzureStorageAccountAccessKey,

        /// <summary>
        /// The logging Azure storage account name
        /// </summary>
        LogStorageAccountName,
        /// <summary>
        /// The logging Azure storage account key
        /// </summary>
        LogStorageAccountAccessKey,

        /// <summary>
        /// The Azure Table storage configuration SAS key
        /// </summary>
        AzureTableStorageConfigSASKey,
    }
}
