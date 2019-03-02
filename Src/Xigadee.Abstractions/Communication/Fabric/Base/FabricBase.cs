namespace Xigadee
{
    /// <summary>
    /// This class is used to form the fabric used to communicate between Microservices.
    /// </summary>
    public abstract class FabricBridgeBase<B>
        where B: ICommunicationAgent
    {
        /// <summary>
        /// Gets the <see cref="ICommunicationAgent"/> for the specified mode.
        /// </summary>
        /// <param name="mode">The communication mode.</param>
        /// <returns>A bridge for the specific communication mode.</returns>
        public abstract B this[FabricMode mode] { get; }

        /// <summary>
        /// Gets the queue agent.
        /// </summary>
        public virtual B Queue => this[FabricMode.Queue];
        /// <summary>
        /// Gets the broadcast agent.
        /// </summary>
        public virtual B Broadcast => this[FabricMode.Broadcast];
    }
}
