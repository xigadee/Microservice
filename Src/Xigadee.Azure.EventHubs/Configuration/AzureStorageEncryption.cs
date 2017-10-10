namespace Xigadee
{
    /// <summary>
    /// This settings determines the specific encryption settings for the data type.
    /// </summary>
    public enum AzureStorageEncryption
    {
        /// <summary>
        /// There is no encrpytion on the blob
        /// </summary>
        None,
        /// <summary>
        /// The blob is encrpyted when an encrpytion handler is present.
        /// </summary>
        BlobWhenPresent,
        /// <summary>
        /// The blob is always encrpyted. If no handler is present, an exception will be thrown.
        /// </summary>
        BlobAlwaysWithException
    }
}
