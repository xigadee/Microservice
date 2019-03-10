namespace Xigadee
{
    /// <summary>
    /// This class is used to form the fabric used to communicate between Microservices.
    /// </summary>
    public abstract class CommunicationFabricBase<B>
        where B: ICommunicationFabricBridge
    {
        /// <summary>
        /// Gets the <see cref="ICommunicationFabricBridge"/> for the specified mode.
        /// </summary>
        /// <param name="mode">The communication mode.</param>
        /// <returns>A bridge for the specific communication mode.</returns>
        protected abstract B this[CommunicationFabricMode mode] { get; }

        /// <summary>
        /// Gets the queue agent.
        /// </summary>
        public virtual B Queue => this[CommunicationFabricMode.Queue];
        /// <summary>
        /// Gets the broadcast agent.
        /// </summary>
        public virtual B Broadcast => this[CommunicationFabricMode.Broadcast];
    }
}
