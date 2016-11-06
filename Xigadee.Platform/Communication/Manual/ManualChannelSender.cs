using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public class ManualChannelSender:MessagingSenderBase<ManualChannelConnection, ManualChannelMessage, ManualChannelClientHolder>
    {

        public override Task ProcessMessage(TransmissionPayload payload)
        {
            return base.ProcessMessage(payload);
        }
    }
}
