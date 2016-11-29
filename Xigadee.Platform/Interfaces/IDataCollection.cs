namespace Xigadee
{
    /// <summary>
    /// This interface is exposed by the data collection to give generic access to all types of 
    /// logging through a single interface.
    /// </summary>
    public interface IDataCollection
    {
        /// <summary>
        /// This method writes the data 
        /// </summary>
        /// <param name="eventData">The event data.</param>
        /// <param name="support">The event type.</param>
        /// <param name="sync">A boolean value that specifies whether the request should be process syncronously (true).</param>
        void Write(EventBase eventData, DataCollectionSupport support, bool sync = false);
    }
}