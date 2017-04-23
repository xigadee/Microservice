using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public class CommandHolderException:Exception
    {
        public CommandHolderException(string message):base(message)
        {

        }
    }
}
