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

        /// <summary>
        /// This action is used to "transmit" a message to the event.
        /// </summary>
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
                batchId = Collector?.BoundaryBatchPoll(count ?? -1, mPending.Count, mappingChannel ?? ChannelId, Priority);

            while (countDown> 0 && mPending.TryDequeue(out payload))
            {
                if (mappingChannel != null)
                    payload.Message.ChannelId = mappingChannel;

                //Get the boundary logger to log the metadata.
                if (BoundaryLoggingActive)
                    Collector?.BoundaryLog(ChannelDirection.Incoming, payload, ChannelId, Priority, batchId: batchId);

                list.Add(payload);

                countDown--;
            }

            return list;
        }

        /// <summary>
        /// This method invokes the event to simulate transmission of the payload.
        /// </summary>
        /// <param name="payload">The payload to transmit.</param>
        /// <param name="retry">The retry count.</param>
        /// <returns></returns>
        public override async Task Transmit(TransmissionPayload payload, int retry = 0)
        {
            bool tryAgain = false;
            bool fail = true;
            try
            {
                LastTickCount = Environment.TickCount;

                if (retry > MaxRetries)
                    throw new RetryExceededTransmissionException();

                IncomingAction?.Invoke(payload);

                if (BoundaryLoggingActive)
                    Collector?.BoundaryLog(ChannelDirection.Outgoing, payload, ChannelId, Priority);

                fail = false;
            }
            catch (Exception ex)
            {
                LogException("Unhandled Exception (Transmit)", ex);
                if (BoundaryLoggingActive)
                    Collector?.BoundaryLog(ChannelDirection.Outgoing, payload, ChannelId, Priority, ex);
                throw;
            }
            finally
            {
                if (fail)
                    StatisticsInternal.ExceptionHitIncrement();
            }

            if (tryAgain)
                await Transmit(payload, ++retry);
        }
    }
}
