namespace Xigadee
{
    /// <summary>
    /// This interface is implemented by components that need to access the shared service container for their usage.
    /// </summary>
    public interface IRequireSharedServices
    {
        /// <summary>
        /// The shared service container.
        /// </summary>
        ISharedService SharedServices { get; set; }
    }
}
