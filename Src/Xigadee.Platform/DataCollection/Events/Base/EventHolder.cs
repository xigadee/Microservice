using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// The event holder holds the event during processing.
    /// </summary>
    public class EventHolder
    {
        /// <summary>
        /// This constructor sets the data type for the event data holder.
        /// </summary>
        /// <param name="dataType">The data type.</param>
        /// <param name="claims">This </param>
        public EventHolder(DataCollectionSupport dataType, ClaimsPrincipal claims)
        {
            DataType = dataType;
            Claims = claims;
        }
        /// <summary>
        /// This property specifies whether the event can be queued for logging (false) or should be logged immediately.
        /// The default is to log async.
        /// </summary>
        public bool Sync { get; set; }
        /// <summary>
        /// The time stamp.
        /// </summary>
        public int Timestamp { get; set; }
        /// <summary>
        /// The queued data.
        /// </summary>
        public EventBase Data { get; set; }

        /// <summary>
        /// This is the datatype for the event.
        /// </summary>
        public DataCollectionSupport DataType { get; }
        /// <summary>
        /// This is the identify of the calling party based on the claims passed with the request,
        /// </summary>
        public ClaimsPrincipal Claims {get;}
    }
}
