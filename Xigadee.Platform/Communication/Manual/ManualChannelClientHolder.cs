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
        private ConcurrentQueue<TransmissionPayload> mPending = new ConcurrentQueue<TransmissionPayload>();

        public ManualChannelClientHolder()
        {

        }

        public Action<TransmissionPayload> IncomingAction { get; set; }

        /// <summary>
        /// This method injects a payload to be picked up by the polling algorithm.
        /// </summary>
        /// <param name="payload">The payload to inject.</param>
        public void Inject(TransmissionPayload payload)
        {
            mPending.Enqueue(payload);
        }

        public override void MessageComplete(TransmissionPayload payload)
        {
        }

        public override async Task<List<TransmissionPayload>> MessagesPull(int? count, int? wait, string mappingChannel = null)
        {
            var list = new List<TransmissionPayload>();

            int countDown = count ?? 1;

            TransmissionPayload payload;

            Guid? batchId = null;
            if (BoundaryLoggingActive)
                batchId = Collector?.BoundaryBatchPoll(count ?? -1, mPending.Count, mappingChannel ?? Name);

            while (countDown> 0 && mPending.TryDequeue(out payload))
            {
                if (mappingChannel != null)
                    payload.Message.ChannelId = mappingChannel;

                list.Add(payload);

                countDown--;
            }

            return list;
        }

        public override async Task Transmit(TransmissionPayload payload, int retry = 0)
        {
            IncomingAction?.Invoke(payload);
        }
    }
}
