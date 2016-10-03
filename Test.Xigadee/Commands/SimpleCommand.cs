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


        [CommandContract(messageType: "franky", actionType: "johnny2")]
        private async Task ThisisMeStupid1(TransmissionPayload incoming, List<TransmissionPayload> outgoing)
        {

        }

        [CommandContract(messageType: "franky", actionType: "johnny1")]
        private void ThisisMeStupid2([CommandIn]Blah item)
        {

        }

        [CommandContract(typeof(IDoSomething1))]
        public string ThisisMeStupid3([CommandIn]Blah item)
        {
            return "Hmm";
        }

        [CommandContract(messageType: "franky", actionType: "johnny3")]
        protected void ThisisMeStupid4([CommandIn]string item)
        {

        }
    }
}
