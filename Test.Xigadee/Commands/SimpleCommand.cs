using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xigadee;

namespace Test.Xigadee
{
    public class SimpleCommand: CommandBase
    {
        public SimpleCommand(CommandPolicy policy = null) : base(policy)
        {
        }

        protected override void CommandsRegister()
        {
            base.CommandsRegister();
        }

        [CommandContract(messageType: "franky", actionType: "johnny1")]
        private void ThisisMeStupid1([CommandIn]Blah item)
        {

        }

        [CommandContract(typeof(IDoSomething1))]
        public void ThisisMeStupid2([CommandIn]Blah item)
        {

        }

        [CommandContract(messageType: "franky", actionType: "johnny3")]
        protected void ThisisMeStupid3([CommandIn]string item)
        {

        }
    }
}
