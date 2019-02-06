using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xigadee;

namespace Xigadee
{
    /// <summary>
    /// This class is used to mock the standard data collection for the holder.
    /// </summary>
    public class ServiceHarnessDataCollection : IDataCollection
    {
        /// <summary>
        /// This is a collection of the events generated.
        /// </summary>
        public ConcurrentQueue<EventHolder> Events = new ConcurrentQueue<EventHolder>();
        /// <summary>
        /// This method writes the events to the queue.
        /// </summary>
        /// <param name="eventData">The event data.</param>
        /// <param name="support">The collection support type.</param>
        /// <param name="sync">The sync flag. Ignored for this usage.</param>
        /// <param name="claims">The current claims.</param>
        public void Write(EventBase eventData, DataCollectionSupport support, bool sync = false, ClaimsPrincipal claims = null)
        {
            Events.Enqueue(new EventHolder(support, claims) { Data = eventData, Sync = sync, Timestamp = Environment.TickCount });
        }
    }
}
