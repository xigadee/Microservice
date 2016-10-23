using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xigadee;

namespace Test.Xigadee
{
    public class SimpleCommand2: CommandBase
    {
        public SimpleCommand2(CommandPolicy policy = null) : base(policy)
        {
        }


        [CommandContract(messageType: "SimpleCommand2", actionType: "johnny1")]
        private async Task ThisisMeStupid1(TransmissionPayload incoming, List<TransmissionPayload> outgoing)
        {

        }



        [CommandContract(messageType: "SimpleCommand2", actionType: "johnny6")]
        [return: PayloadOut]
        private async Task<string> ThisisMeStupid6(TransmissionPayload incoming, List<TransmissionPayload> outgoing)
        {
            return "ff";
        }
    }
}
