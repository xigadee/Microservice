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
        public Func<UdpReceiveResult, object> ConvertIncoming { get; set; }

        public Func<object, UdpReceiveResult> ConvertOutgoing { get; set; }

        

        /// <summary>
        /// This method pulls fabric messages and converts them in to generic payload messages for the Microservice to process.
        /// </summary>
        /// <param name="count">The maximum number of messages to return.</param>
        /// <param name="wait">The maximum wait in milliseconds</param>
        /// <param name="mappingChannel">This is the incoming mapping channel for subscription based client where the subscription maps
        /// to a new incoming channel on the same topic.</param>
        /// <returns>
        /// Returns a list of transmission for processing.
        /// </returns>
        public override async Task<List<TransmissionPayload>> MessagesPull(int? count, int? wait, string mappingChannel = null)
        {
            int? timeOut = null;
            int countMax = count ?? 10;

            if (wait.HasValue)
                timeOut = Environment.TickCount + wait.Value;

            List<TransmissionPayload> batch =  new List<TransmissionPayload>();

            try
            {
                Guid? batchId = null;

                while (Client.Available > 0 
                    && countMax > 0
                    && (!timeOut.HasValue || timeOut.Value>Environment.TickCount)
                    )
                {
                    var result = await Client.ReceiveAsync();
                    //if (BoundaryLoggingActive)
                    //    batchId = Collector?.BoundaryBatchPoll(count ?? -1, intBatch.Count, mappingChannel ?? ChannelId, Priority);

                    var sm = MessageUnpack(result);

                    countMax--;
                }
            }
            catch (Exception ex)
            {
                LogException("Messaging Exception (Pull)", ex);
            }

            LastTickCount = Environment.TickCount;

            return batch;
        }

        /// <summary>
        /// This method is used to Transmit the payload. You should override this method to insert your own transmission logic.
        /// </summary>
        /// <param name="payload">The payload to transmit.</param>
        /// <param name="retry">This parameter specifies the number of retries that should be attempted if transmission fails. By default this value is 0.</param>
        /// <returns></returns>
        public override Task Transmit(TransmissionPayload payload, int retry = 0)
        {
            payload.CompleteSet();
            return Task.FromResult(0);
        }      
    }
}
