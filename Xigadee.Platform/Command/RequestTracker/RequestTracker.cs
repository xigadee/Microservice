#region using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks; 
#endregion
namespace Xigadee
{
    /// <summary>
    /// This base class holds the process requests.
    /// </summary>
    public class RequestTracker
    {
        public RequestTracker()
        {
            UTCStart = DateTime.UtcNow;
            TTL = TimeSpan.FromMinutes(5);
        }

        public string Id { get; set; }

        public TransmissionPayload Payload { get; set; }

        public DateTime UTCStart { get; set; }

        public TimeSpan TTL { get; set; }

        public object State { get; set; }

        public bool HasExpired
        {
            get
            {
                return DateTime.UtcNow > UTCStart.Add(TTL);
            }
        }
    }
}
