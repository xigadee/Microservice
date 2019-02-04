using System;

namespace Xigadee
{
    /// <summary>
    /// This class contains the change information for the command.
    /// </summary>
    public class CommandChangeEventArgs: CommandEventArgsBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandChangeEventArgs"/> class.
        /// </summary>
        /// <param name="isRemoval">if set to <c>true</c> [is removal].</param>
        /// <param name="key">The key.</param>
        /// <param name="isMasterJob">if set to <c>true</c> [is master job].</param>
        public CommandChangeEventArgs(bool isRemoval, MessageFilterWrapper key, bool isMasterJob)
        {
            IsRemoval = isRemoval;
            Key = key;
            IsMasterJob  = isMasterJob;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is a removal.
        /// </summary>
        public bool IsRemoval { get; }
        /// <summary>
        /// Gets a value indicating whether this instance is master job.
        /// </summary>
        public bool IsMasterJob { get; }
        /// <summary>
        /// Gets the key.
        /// </summary>
        public MessageFilterWrapper Key { get; }
    }
}
