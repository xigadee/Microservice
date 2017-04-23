using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This exception is thrown when an encryption handler id not found in the collection.
    /// </summary>
    public class EncryptionHandlerNotResolvedException: Exception
    {
        /// <summary>
        /// This is he default constructor.
        /// </summary>
        /// <param name="channelId">The channel id.</param>
        /// <param name="id">The encryption handler identifier.</param>
        public EncryptionHandlerNotResolvedException(string channelId, string id)
            : base($"Encryption handler '{id}' for channel '{channelId}' was not found in the security collection")
        {

        }

        /// <summary>
        /// This is he default constructor.
        /// </summary>
        /// <param name="id">The encryption handler identifier.</param>
        public EncryptionHandlerNotResolvedException(string id)
            : base($"Encryption handler '{id}' was not found in the security collection")
        {

        }
    }
}
