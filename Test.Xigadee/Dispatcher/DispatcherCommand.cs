using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xigadee;

namespace Test.Xigadee
{
    public class DispatcherCommand:CommandBase
    {

        [CommandContract("friday", "feeling")]
        [return: PayloadOut]
        private string TestCommand([PayloadIn] string inData)
        {
            return "Thanks for all the fish";
        }
    }
}
