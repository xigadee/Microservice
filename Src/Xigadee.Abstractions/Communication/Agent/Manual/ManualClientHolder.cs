using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public class ManualClientHolder : ClientHolderV2<MessagingServiceStatistics>
    {
        private ConcurrentQueue<TransmissionPayload> mPending = new ConcurrentQueue<TransmissionPayload>();

        /// <summary>
        /// This action is used to "transmit" a message to the event.
        /// </summary>
        public Action<TransmissionPayload> IncomingAction { get; set; }

        public ManualClientHolder()
        {

        }


        /// <summary>
        /// Purges any remaining messages when the service shuts down.
        /// </summary>
        public void Purge()
        {
            TransmissionPayload payload = null;

            while (mPending?.TryDequeue(out payload) ?? false)
            {
                payload.TraceWrite("Purged", "ManualChannelClientHolder/Purge");
                payload.SignalFail();
            }
        }

        /// <summary>
        /// This method injects a payload to be picked up by the polling algorithm.
        /// </summary>
        /// <param name="payload">The payload to inject.</param>
        public void Inject(TransmissionPayload payload)
        {
            try
            {
                mPending.Enqueue(payload);
                payload.TraceWrite("Enqueued", "ManualChannelClientHolder/Inject");
            }
            catch (Exception ex)
            {
                payload.TraceWrite($"Failed: {ex.Message}", "ManualChannelClientHolder/Inject");
            }
        }

        public override Task<List<TransmissionPayload>> MessagesPull(int? count, int? wait, string mappingChannel = null)
        {
            var list = new List<TransmissionPayload>();

            int countDown = count ?? 1;

            TransmissionPayload payload;

            Guid? batchId = null;
            if (BoundaryLoggingActive)
                batchId = Collector?.BoundaryBatchPoll(count ?? -1, mPending.Count, mappingChannel ?? ChannelId, Priority);

            while (countDown > 0 && mPending.TryDequeue(out payload))
            {
                if (mappingChannel != null)
                    payload.Message.ChannelId = mappingChannel;

                //Get the boundary logger to log the metadata.
                if (BoundaryLoggingActive)
                    Collector?.BoundaryLog(ChannelDirection.Incoming, payload, ChannelId, Priority, batchId: batchId);

                list.Add(payload);
                payload.TraceWrite("MessagesPull", "ManualChannelClientHolder");

                countDown--;
            }

            return Task.FromResult(list);
        }

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
