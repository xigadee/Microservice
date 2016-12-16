using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This exception is thrown when a registered encryption handler is not found.
    /// </summary>
    public class ChannelEncryptionIdNotResolvedException:Exception
    {
        /// <summary>
        /// This is he default constructor.
        /// </summary>
        /// <param name="channel">The channel.</param>
        public ChannelEncryptionIdNotResolvedException(Channel channel) 
            :base($"Channel {channel.Id} encryption handler {channel.EncryptionHandlerId} was not found in the security collection")
        {

        }
    }
}
