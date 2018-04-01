using System;

namespace Xigadee
{
    /// <summary>
    /// This enumeration specifies the direction of the message.
    /// </summary>
    [Flags]
    public enum ChannelDirection:int
    {
        /// <summary>
        /// The message has been received from an external source.
        /// </summary>
        Incoming = 1,
        /// <summary>
        /// The message has just been transmitted to an external source.
        /// </summary>
        Outgoing = 2
    }
}
