using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This enumeration specifies the direction of the message.
    /// </summary>
    public enum ChannelDirection
    {
        /// <summary>
        /// The message has been received from an external source.
        /// </summary>
        Incoming,
        /// <summary>
        /// The message has just been transmitted to an external source.
        /// </summary>
        Outgoing
    }
}
