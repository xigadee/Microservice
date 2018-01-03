using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This class holds the Udp client and associated logic.
    /// </summary>
    public class UdpClientHolder : ClientHolder<UdpHelper, SerializationHolder>
    {
        /// <summary>
        /// Gets or sets the type of the binary content. This is used for deserialization.
        /// </summary>
        public string ContentType { get; set; }
        /// <summary>
        /// Gets or sets any specific encoding used for the binary payload, i.e. GZIP
        /// </summary>
        public string ContentEncoding { get; set; }

        #region MessagesPull(int? count, int? wait, string mappingChannel = null)
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
            int errorCount = 0;
            //Guid? batchId = null;

            if (wait.HasValue)
                timeOut = Environment.TickCount + wait.Value;

            List<TransmissionPayload> batch = new List<TransmissionPayload>();

            //if (BoundaryLoggingActive)
            //    batchId = Collector?.BoundaryBatchPoll(count ?? -1, intBatch.Count, mappingChannel ?? ChannelId, Priority);

            try
            {
                while (Client.Available
                    && countMax > 0
                    && (!timeOut.HasValue || timeOut.Value > Environment.TickCount)
                    )
                {
                    try
                    {
                        var result = await Client.ReceiveAsync();

                        var holder = (SerializationHolder)result.Buffer;
                        holder.Metadata = result.RemoteEndPoint;
                        holder.ContentType = ContentType;
                        holder.ContentEncoding = ContentEncoding;

                        //Unpack the message in the holder.
                        var sm = MessageUnpack(holder);

                        batch.Add(new TransmissionPayload(sm));
                    }
                    catch (Exception ex)
                    {
                        Collector?.LogException("UdpClientHolder/MessagesPull deserialization error.", ex);
                        errorCount++;
                    }

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
        #endregion

        #region Transmit(TransmissionPayload payload, int retry = 0)
        /// <summary>
        /// This method is used to Transmit the payload. You should override this method to insert your own transmission logic.
        /// </summary>
        /// <param name="payload">The payload to transmit.</param>
        /// <param name="retry">This parameter specifies the number of retries that should be attempted if transmission fails. By default this value is 0.</param>
        public override async Task Transmit(TransmissionPayload payload, int retry = 0)
        {
            try
            {
                var holder = MessagePack(payload);

                if (holder.Blob != null)
                {
                    var result = await Client.SendAsync(holder.Blob, holder.Blob.Length);
                }

            }
            catch (Exception ex)
            {
                Collector?.LogException("UdpClientHolder/Transmit", ex);
            }

        }    
        #endregion
    }
}
