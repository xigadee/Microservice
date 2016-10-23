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


        [CommandContract(messageType: "franky", actionType: "johnny1")]
        private async Task ThisisMeStupid1(TransmissionPayload incoming, List<TransmissionPayload> outgoing)
        {

        }


        [CommandContract(messageType: "franky", actionType: "johnny2")]
        private void ThisisMeStupid2([PayloadIn]Blah item)
        {

        }


        [CommandContract(typeof(IDoSomething1))]
        [return: PayloadOut]
        public string ThisisMeStupid3([PayloadIn]Blah item)
        {
            return "Hmm";
        }

        [CommandContract(messageType: "franky", actionType: "johnny4")]
        protected void ThisisMeStupid4([PayloadIn]string item)
        {

        }

        [CommandContract(messageType: "franky", actionType: "johnny5")]
        [CommandContract(messageType: "franky", actionType: "johnny5b")]
        private void ThisisMeStupid5([PayloadIn]Blah item, [PayloadOut]out string response)
        {
            response = null;
        }

        [CommandContract(messageType: "franky", actionType: "johnny6")]
        [return: PayloadOut]
        private async Task<string> ThisisMeStupid6(TransmissionPayload incoming, List<TransmissionPayload> outgoing)
        {
            return "ff";
        }
    }
}
