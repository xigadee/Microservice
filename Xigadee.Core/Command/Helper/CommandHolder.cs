using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public class CommandHolder: IEquatable<CommandHolder>
    {
        public CommandHolder(MessageFilterWrapper message, bool requiresQuorum)
        {
            Message = message;
            RequiresQuorum = requiresQuorum;
        }

        public MessageFilterWrapper Message { get; set; }

        public bool RequiresQuorum { get; set; }

        public bool Equals(CommandHolder other)
        {
            return other.Message == Message;
        }
    }
}
