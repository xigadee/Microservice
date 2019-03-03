using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This is the default holder for a manual fabric connection.
    /// </summary>
    public class ManualFabricConnection:FabricConnectionBase<ManualFabricMessage>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ManualFabricConnection"/> class.
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <param name="channel">The channel.</param>
        /// <param name="subscription">The subscription.</param>
        /// <exception cref="ArgumentNullException">
        /// channel - Channel identifier cannot be null
        /// or in subscription mode,
        /// subscription - Subscription identifier cannot be null.
        /// </exception>
        public ManualFabricConnection(ManualFabricConnectionMode mode, string channel, string subscription = null)
        {
            Mode = mode;
            Channel = channel ?? throw new ArgumentNullException("channel", "Channel identifier cannot be null");
            if (Mode == ManualFabricConnectionMode.Subscription)
                Subscription = subscription ?? throw new ArgumentNullException("subscription", "Subscription identifier cannot be null.");
        }
        /// <summary>
        /// Gets the connection mode.
        /// </summary>
        public ManualFabricConnectionMode Mode { get; }
        /// <summary>
        /// Gets the client channel identifier.
        /// </summary>
        public string Channel { get; }
        /// <summary>
        /// Gets the subscription identifier if this is in subscription mode..
        /// </summary>
        public string Subscription { get; }

        /// <summary>
        /// Enqueues the specified message on to the fabric.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <exception cref="ArgumentException">Thrown if the connection is not enabled to Transmit.</exception>
        public void Enqueue(ManualFabricMessage message)
        {
            if (!CanEnqueue)
                throw new ArgumentException($"Transmit is not enabled");

            Transmit(this, message);
        }
        /// <summary>
        /// Gets a value indicating whether this instance can enqueue.
        /// </summary>
        public bool CanEnqueue { get { return Transmit != null; } }

        /// <summary>
        /// Dequeues the specified attempt.
        /// </summary>
        /// <param name="attempt">The attempt.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        public IEnumerable<ManualFabricMessage> Dequeue(int? attempt = null)
        {
            if (!CanDequeue)
                throw new ArgumentException($"Receive is not enabled");

            return Receive(this, attempt);
        }
        /// <summary>
        /// Gets a value indicating whether this instance can enqueue.
        /// </summary>
        public bool CanDequeue { get { return Receive != null; } }

        internal Action<ManualFabricConnection, ManualFabricMessage> Transmit { get; set; }

        internal Func<ManualFabricConnection, int?, IEnumerable<ManualFabricMessage>> Receive { get; set; }
    }
}
