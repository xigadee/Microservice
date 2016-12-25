using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This enumeration determines the specific storage behavior for the data type.
    /// </summary>
    [Flags]
    public enum AzureStorageBehaviour: int
    {
        /// <summary>
        /// The entity should not be stored.
        /// </summary>
        None = 0,
        /// <summary>
        /// This entity should be stored using blob storage.
        /// </summary>
        Blob = 1,
        /// <summary>
        /// This entity should be stored using table storage.
        /// </summary>
        Table = 2,
        /// <summary>
        /// This entity should be stored using both blob and table storage.
        /// </summary>
        BlobAndTable = 3,
        /// <summary>
        /// This entity should be stored using Azure Queue storage.
        /// </summary>
        Queue = 4,
        /// <summary>
        /// This entity should be stored using all methods.
        /// </summary>
        All = int.MaxValue
    }
}
