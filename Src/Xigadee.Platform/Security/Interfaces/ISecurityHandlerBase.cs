namespace Xigadee
{
    /// <summary>
    /// This interface is used by classes that provide authentication for incoming and outgoing messages.
    /// </summary>
    public interface ISecurityHandlerBase
    {
        /// <summary>
        /// This is the friendly name for the handler.
        /// </summary>
        string Name { get; }
    }
}
