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

        private void ProcessInvoke(TransmissionPayload payload)
        {
            try
            {
                OnProcess?.Invoke(this, payload);
            }
            catch (Exception ex)
            {
                Collector?.LogException("ManualChannelSender/ProcessInvoke", ex);
            }
        }

        protected override ManualChannelClientHolder ClientCreate(SenderPartitionConfig partition)
        {
            var holder = base.ClientCreate(partition);

            holder.IncomingAction = ProcessInvoke;

            return holder;
        }
    }
}
