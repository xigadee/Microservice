namespace Xigadee
{
    /// <summary>
    /// This interface is used by classes that provide authentication for incoming and outgoing messages.
    /// </summary>
    public interface ISecurityHandlerBase
    {
        /// <summary>
        /// Gets the encryption identifier used to match a handler with a specific id.
        /// </summary>
        string Id { get; }
        /// <summary>
        /// This is the friendly name for the handler.
        /// </summary>
        string Name { get; }
    }
}
