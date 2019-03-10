namespace Xigadee
{
    /// <summary>
    /// This settings determines the specific encryption settings for the data type.
    /// </summary>
    public enum AzureStorageEncryption
    {
        /// <summary>
        /// There is no encryption on the blob.
        /// </summary>
        None,
        /// <summary>
        /// The blob is encrypted when an encryption handler is present.
        /// </summary>
        BlobWhenPresent,
        /// <summary>
        /// The blob is always encrypted. If no handler is present, an exception will be thrown.
        /// </summary>
        BlobAlwaysWithException
    }
}
