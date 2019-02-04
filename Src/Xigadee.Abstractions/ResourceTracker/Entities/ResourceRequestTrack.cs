using System;
using System.Diagnostics;
using System.Threading;

namespace Xigadee
{
    /// <summary>
    /// This class holds a record of messages that have signalled a slow down or an exception.
    /// </summary>
    [DebuggerDisplay("{Debug}")]
    public class ResourceRequestTrack
    {
        #region Declarations
        private readonly int mStart;
        private int mRetryCount = 0;
        private long mRetryTime = 0;
        private string mGroup;
        #endregion

        #region Constructor
        /// <summary>
        /// This is the default constructor
        /// </summary>
        /// <param name="id">Trace Id</param>
        /// <param name="group">The group.</param>
        public ResourceRequestTrack(Guid id, string group)
        {
            mStart = Environment.TickCount;
            Id = id;
            mGroup = group;
        } 
        #endregion

        #region ProfileId
        /// <summary>
        /// This is the incoming profile id from the calling party.
        /// </summary>
        public Guid ProfileId { get; set; } 
        #endregion
        #region Id
        /// <summary>
        /// This is the traceid of the payload that signalled a throttle request.
        /// </summary>
        public Guid Id { get; private set; }
        #endregion

        #region Active
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
        #endregion

        #region RetryCount
        /// <summary>
        /// This is the count of retries attempted on the resource
        /// </summary>
        public int RetryCount { get { return mRetryCount; } } 
        #endregion

        #region Debug
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
        #endregion

        #region RetrySignal(int delta, ResourceRetryReason reason)
        /// <summary>
        /// This method signals a retry on the resource
        /// </summary>
        /// <param name="delta">The retry time.</param>
        /// <param name="reason">The last retry reason.</param>
        public void RetrySignal(int delta, ResourceRetryReason reason)
        {
            Interlocked.Increment(ref mRetryCount);
            Interlocked.Add(ref mRetryTime, (long)delta);
        } 
        #endregion
    }
}
