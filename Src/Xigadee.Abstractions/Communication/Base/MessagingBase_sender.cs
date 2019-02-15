using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
namespace Xigadee
{
    public abstract partial class MessagingBase<C, M, H>
    {

        public IEnumerable<ClientHolder> SenderClients => throw new NotImplementedException();

        public List<SenderPartitionConfig> SenderPriorityPartitions { get; set; }

        public void ListenerCommandsActiveChange(List<MessageFilterWrapper> supported)
        {
            throw new NotImplementedException();
        }


        public Task SenderTransmit(TransmissionPayload message)
        {
            throw new NotImplementedException();
        }
    }
}
