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
        public CommandChange(bool isRemoval, MessageFilterWrapper key)
        {
            IsRemoval = isRemoval;
            Key = key;
        }

        public bool IsRemoval { get; protected set; }

        public MessageFilterWrapper Key { get; protected set; }
    }
}
