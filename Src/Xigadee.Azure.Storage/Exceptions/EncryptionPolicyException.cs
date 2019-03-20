using System;

namespace Xigadee
{
    /// <summary>
    /// This exception is thrown when a storage logger is set to throw an exception
    /// when encryption is set as mandatory, but an encryption handler has not been set.
    /// </summary>
    public class AzureStorageDataCollectorEncryptionPolicyException:Exception
    {
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="support">The support options.</param>
        public AzureStorageDataCollectorEncryptionPolicyException(DataCollectionSupport support)
        {
            Support = support;
        }
        /// <summary>
        /// This is the type of logging.
        /// </summary>
        public DataCollectionSupport Support { get; set; }
    }
}
