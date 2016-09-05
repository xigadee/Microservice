using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This class holds the command message notification.
    /// </summary>
    public class CommandHolder: IEquatable<CommandHolder>
    {
        public CommandHolder(MessageFilterWrapper message, bool requiresQuorum)
        {
            Message = message;
            RequiresQuorum = requiresQuorum;
        }
        /// <summary>
        /// This is the message filter.
        /// </summary>
        public MessageFilterWrapper Message { get; set; }

        public bool RequiresQuorum { get; set; }

        public bool Equals(CommandHolder other)
        {
            return other.Message == Message;
        }
    }
}
