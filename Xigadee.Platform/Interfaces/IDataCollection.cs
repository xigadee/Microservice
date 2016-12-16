using System.Security.Claims;

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
        /// <param name="claims">The optional claims of the calling party. If not set explicity, then this
        /// will be populated from the current thread. If you don't want this then pass an empty claims object.</param>
        void Write(EventBase eventData, DataCollectionSupport support, bool sync = false, ClaimsPrincipal claims = null);
    }
}