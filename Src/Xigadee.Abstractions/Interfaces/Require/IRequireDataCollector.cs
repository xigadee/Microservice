namespace Xigadee
{
    /// <summary>
    /// This interface is implemented by component that require raw access to the data collector.
    /// </summary>
    public interface IRequireDataCollector
    {
        /// <summary>
        /// This is the data collector.
        /// </summary>
        IDataCollection Collector { get; set; }
    }
}
