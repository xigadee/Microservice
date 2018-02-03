namespace Xigadee
{
    /// <summary>
    /// This interface is used by classes that are provide services through the use of system handlers.
    /// </summary>
    public interface IServiceHandler
    {
        /// <summary>
        /// Gets the handler unique identifier.
        /// </summary>
        string Id { get; }
        /// <summary>
        /// This is the friendly name for the handler.
        /// </summary>
        string Name { get; }
    }
}
