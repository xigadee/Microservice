namespace Xigadee
{
    /// <summary>
    /// This interface is for components that require payload serialization and deserialization.
    /// </summary>
    public interface IRequireServiceHandlers
    {
        /// <summary>
        /// This is the system wide service handlers.
        /// </summary>
        IServiceHandlerContainer ServiceHandlers { get; set; }
    }
}
