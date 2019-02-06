using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Xigadee
{
    /// <summary>
    /// This is the base class used to hold a messaging client.
    /// </summary>
    /// <typeparam name="C">The client type.</typeparam>
    /// <typeparam name="M">The client message type.</typeparam>
    public abstract class ClientHolder<C, M>: ClientHolder
        where C: class
    {
        /// <summary>
        /// This is the internal client
        /// </summary>
        public C Client { get; set; }
        /// <summary>
        /// This function creates the client when requested.
        /// </summary>
        public Func<C> ClientCreate { get; set; }
        /// <summary>
        /// This method is used to signal completion of a specific message.
        /// </summary>
        public Action<M, bool> MessageSignal { get; set; }
        /// <summary>
        /// This function receives message batches.
        /// </summary>
        public Func<int?, int?, Task<IEnumerable<M>>> MessageReceive { get; set; }
        /// <summary>
        /// This method transmits the message over the specific fabric
        /// </summary>
        public Func<M, Task> MessageTransmit { get; set; }
        /// <summary>
        /// This function unpacks the message from the specific fabric format to the generic ServiceMessage format for transmission.
        /// </summary>
        public Func<M, ServiceMessage> MessageUnpack { get; set; }
        /// <summary>
        /// This function packs the message in to the correct format for transmission
        /// </summary>
        public Func<TransmissionPayload, M> MessagePack { get; set; }
        /// <summary>
        /// This method signals using the MessageSignal function if this is not null.
        /// </summary>
        /// <param name="message">The message to signal.</param>
        /// <param name="signal">The signal flag.</param>
        /// <param name="id">The optional id.</param>
        public virtual void MessageSignalInternal(M message, bool signal, Guid id)
        {
            MessageSignal?.Invoke(message, signal);
        }

        /// <summary>
        /// This method creates a wrapper around the message which links back through the TransmissionPayload
        /// </summary>
        /// <param name="message">The internal message.</param>
        /// <param name="serviceMessage">The service message to process.</param>
        /// <returns>Returns a Transmission payload, with the internal signalling pointing back to the message signal function.</returns>
        protected virtual TransmissionPayload PayloadRegisterAndCreate(M message, ServiceMessage serviceMessage)
        {
            var payload = new TransmissionPayload(serviceMessage
                , release: (b, i) => MessageSignalInternal(message, b, i))
            {
                MaxProcessingTime = MessageMaxProcessingTime,
                Options = ProcessOptions.RouteInternal,
                Source = Name,

            };

            return payload;
        }

    }
}
