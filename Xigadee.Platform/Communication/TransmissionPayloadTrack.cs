using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This class holds a record of messages that have signalled a slow down.
    /// </summary>
    [DebuggerDisplay("{Debug}")]
    public class TranmissionPayloadTrack
    {
        private readonly int mStart;
        private int mRateLimitSignalCount = 0;

        public TranmissionPayloadTrack(Guid id)
        {
            mStart = Environment.TickCount;
            Id = id;
        }

        /// <summary>
        /// This is the traceid of the payload that signalled a throttle request.
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// This is the throttle time expiry.
        /// </summary>
        public TimeSpan? Active
        {
            get
            {
                return TimeSpan.FromMilliseconds(ConversionHelper.CalculateDelta(Environment.TickCount, mStart));
            }
        }

        public int RateLimitSignalCount {get{ return mRateLimitSignalCount; } }
        /// <summary>
        /// This is the debug string for logging.
        /// </summary>
        public string Debug
        {
            get
            {
                return string.Format("{0} - {1} Hits={2}", Id, Active, mRateLimitSignalCount);
            }
        }

        public void RateLimitSignal()
        {
            Interlocked.Increment(ref mRateLimitSignalCount);
        }
    }
}
