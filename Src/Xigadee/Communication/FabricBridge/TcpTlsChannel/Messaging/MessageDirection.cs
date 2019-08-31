using System;

namespace Xigadee
{
    /// <summary>
    /// This enumeration defines the messaging charteristics.
    /// </summary>
    [Flags]
    public enum MessageDirection
    {
        /// <summary>
        /// This is the default value that the message is set to 
        /// </summary>
        Undefined = 0,
        /// <summary>
        /// Message can be read from.
        /// </summary>
        Read = 1,
        /// <summary>
        /// Message can be written to.
        /// </summary>
        Write = 2,
        /// <summary>
        /// Message supports both reading and writing.
        /// </summary>
        Bidirectional = 3
    }
}
