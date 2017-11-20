using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Xigadee
{
    public class UdpClientHolder : ClientHolder<UdpClient, ServiceMessage>
    {
        public UdpClientHolder()
        {

        }

        public override async Task<List<TransmissionPayload>> MessagesPull(int? count, int? wait, string mappingChannel = null)
        {
            List<TransmissionPayload> batch = null;

            //Guid? batchId = null;
            //try
            //{
            //    var intBatch = (await MessageReceive(count, wait))?.ToList() ?? new List<M>();
            //    if (BoundaryLoggingActive)
            //        batchId = Collector?.BoundaryBatchPoll(count ?? -1, intBatch.Count, mappingChannel ?? ChannelId, Priority);

            //    batch = intBatch.Select(m => TransmissionPayloadUnpack(m, Priority, mappingChannel, batchId)).ToList();
            //}
            //catch (MessagingException dex)
            //{
            //    //OK, something has gone wrong with the Azure fabric.
            //    LogException("Messaging Exception (Pull)", dex);
            //    //Let's reinitialise the client
            //    if (ClientReset == null)
            //        throw;

            //    ClientReset(dex);
            //    batch = batch ?? new List<TransmissionPayload>();
            //}
            //catch (TimeoutException tex)
            //{
            //    LogException("MessagesPull (Timeout)", tex);
            //    batch = batch ?? new List<TransmissionPayload>();
            //}

            LastTickCount = Environment.TickCount;

            return batch ?? new List<TransmissionPayload>();
        }

        public override Task Transmit(TransmissionPayload payload, int retry = 0)
        {
            throw new NotImplementedException();
        }

    }
}
