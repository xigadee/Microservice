using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public class ManualChannelClientHolder: ClientHolder<ManualChannelConnection, ManualChannelMessage>
    {
        private ConcurrentQueue<ServiceMessage> mPending = new ConcurrentQueue<ServiceMessage>();

        public ManualChannelClientHolder()
        {

        }
        public void Inject(ServiceMessage message)
        {
            mPending.Enqueue(message);
        }

        public override void MessageComplete(TransmissionPayload payload)
        {
        }

        public override async Task<List<TransmissionPayload>> MessagesPull(int? count, int? wait, string mappingChannel = null)
        {
            var list = new List<TransmissionPayload>();

            int countDown = count ?? 1;

            ServiceMessage message;

            while (countDown> 0 && mPending.TryDequeue(out message))
            {
                if (mappingChannel != null)
                    message.ChannelId = mappingChannel;

                var payload = new TransmissionPayload(message);

                list.Add(payload);

                countDown--;
            }

            return list;
        }

        public override async Task Transmit(TransmissionPayload payload, int retry = 0)
        {

        }
    }
}
