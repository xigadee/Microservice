using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Xigadee
{
    /// <summary>
    /// This class holds a record of messages that have signalled a slow down or an exception.
    /// </summary>
    [DebuggerDisplay("{Debug}")]
    public class ResourceRequestTrack
    {
        private readonly int mStart;
        private int mRetryCount = 0;
        private long mRetryTime = 0;
        private string mGroup;

        public ResourceRequestTrack(Guid id, string group)
        {
            mStart = Environment.TickCount;
            Id = id;
            mGroup = group;
        }

        /// <summary>
        /// This is the incoming profile id from the calling party.
        /// </summary>
        public Guid ProfileId { get; set; }

        #region Id
        /// <summary>
        /// This is the traceid of the payload that signalled a throttle request.
        /// </summary>
        public Guid Id { get; private set; }
        #endregion

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

        public int RetryCount { get { return mRetryCount; } }

        /// <summary>
        /// This is the debug string for logging.
        /// </summary>
        public string Debug
        {
            get
            {
                return string.Format("[{0}]/{1} Retries={2}/{3} {4} - {5}", mGroup, ProfileId, mRetryTime, mRetryCount, Active, Id);
            }
        }

        public void RetrySignal(int delta, ResourceRetryReason reason)
        {
            Interlocked.Increment(ref mRetryCount);
            Interlocked.Add(ref mRetryTime, (long)delta);
        }
    }
}
