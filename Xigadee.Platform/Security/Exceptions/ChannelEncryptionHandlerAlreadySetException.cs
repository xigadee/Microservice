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
    public class ChannelEncryptionHandlerAlreadySetException: Exception
    {
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="channelId">The channel identifier.</param>
        public ChannelEncryptionHandlerAlreadySetException(string channelId)
            : base($"Encryption handler {channelId} was not found in the security collection")
        {

        }
    }
}
