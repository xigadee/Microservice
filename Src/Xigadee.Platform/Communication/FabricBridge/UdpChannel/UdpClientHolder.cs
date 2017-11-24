using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This class holds the Udp client and associated logic.
    /// </summary>
    public class UdpClientHolder : ClientHolder<UdpClient, UdpReceiveResult>
    {
        public override async Task<List<TransmissionPayload>> MessagesPull(int? count, int? wait, string mappingChannel = null)
        {
            List<TransmissionPayload> batch =  new List<TransmissionPayload>();

            try
            {
                Guid? batchId = null;

                while (Client.Available > 0)
                {
                    var result = await Client.ReceiveAsync();
                    //if (BoundaryLoggingActive)
                    //    batchId = Collector?.BoundaryBatchPoll(count ?? -1, intBatch.Count, mappingChannel ?? ChannelId, Priority);

                    var sm = MessageUnpack(result);

                }
            }
            catch (Exception ex)
            {
                LogException("Messaging Exception (Pull)", ex);
            }

            LastTickCount = Environment.TickCount;

            return batch;
        }

        public override Task Transmit(TransmissionPayload payload, int retry = 0)
        {
            payload.CompleteSet();
            return Task.FromResult(0);
        }      
    }
}
