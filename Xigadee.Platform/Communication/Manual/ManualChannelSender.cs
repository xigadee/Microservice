using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public class ManualChannelSender:MessagingSenderBase<ManualChannelConnection, ManualChannelMessage, ManualChannelClientHolder>
    {
        public event EventHandler<TransmissionPayload> OnProcess;
        
        public override async Task ProcessMessage(TransmissionPayload payload)
        {
            OnProcess?.Invoke(this, payload);
            //return base.ProcessMessage(payload);
        }
    }
}
