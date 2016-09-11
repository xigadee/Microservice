using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This class contains the change information for the command.
    /// </summary>
    public class CommandChange
    {
        public CommandChange(bool isRemoval, MessageFilterWrapper key, bool requiresQuorum = false)
        {
            IsRemoval = isRemoval;
            Key = key;
            RequiresQuorum = requiresQuorum;
        }

        public bool IsRemoval { get; protected set; }

        public bool RequiresQuorum { get; protected set; }

        public MessageFilterWrapper Key { get; protected set; }
    }
}
