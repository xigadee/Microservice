using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Xigadee
{
    /// <summary>
    /// This is the holder for the resource statistics.
    /// </summary>
    [DebuggerDisplay("ResourceStatistics: {Name}={RateLimitAdjustmentPercentage}")]
    public class ResourceStatistics: MessagingStatistics
    {
        #region Declarations
        /// <summary>
        /// This concurrent dictionary holds the current list of active messages and their ratelimit request count.
        /// </summary>
        protected readonly ConcurrentDictionary<Guid, ResourceRequestTrack> mActive;

        protected readonly double mRateLimitCutoutPercentage; 

        protected readonly string mName;

        protected StatsCounter mJobs;
        #endregion
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public ResourceStatistics()
        {
            mActive = new ConcurrentDictionary<Guid, ResourceRequestTrack>();
            mRateLimitCutoutPercentage = 1D;
        } 

        #endregion

        #region RateLimitCutoutPercentage
        /// <summary>
        /// This is the retry ratio at which point the incoming messahes will be stopped completely.
        /// </summary>
        public double RateLimitCutoutPercentage
        {
            get
            {
                return mRateLimitCutoutPercentage;
            }
        } 
        #endregion

        #region RetrySum
        /// <summary>
        /// This is the current rate limit summation across the active payload requests.
        /// If rate limiting is not supported the value will be null.
        /// </summary>
        public int RetrySum
        {
            get
            {
                return mActive.Values.Select(r => r.RetryCount).Sum();
            }
        }
        #endregion
        #region RetryRatio
        /// <summary>
        /// This is the current rate limit summation across the active payload requests.
        /// If rate limiting is not supported the value will be null.
        /// </summary>
        public double RetryRatio
        {
            get
            {
                int count = mActive.Count;

                return count == 0 ? 0 : (double)RetrySum / (double)count;
            }
        }
        #endregion
        #region RateLimitAdjustmentPercentage
        /// <summary>
        /// This is the current rate limit summation across the active payload requests.
        /// If rate limiting is not supported the value will be null.
        /// </summary>
        public double RateLimitAdjustmentPercentage
        {
            get
            {
                double ratio = RetryRatio / mRateLimitCutoutPercentage;

                //Ok, we have exceeded the cut off threshold. Stop everything.
                if (ratio >= 1)
                    return 0;

                if (ratio <= 0)
                    return 1;

                //Adjust the percentage so that as the ratio reduces the adjustment decreases.
                return 1 - ratio;
            }
        }
        #endregion

        /// <summary>
        /// This is a list of the currently active requests.
        /// </summary>
        public string[] Active
        {
            get
            {
                try
                {
                    return mActive.Values.Select((v) => v.Debug).ToArray();
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        internal Guid Start(string name, string group, Guid profileId)
        {
            ActiveIncrement();
            var id = Guid.NewGuid();
            var item = mActive.GetOrAdd(id, new ResourceRequestTrack(id, group) { ProfileId = profileId } );
            return item.Id;
        }

        internal void End(string name, Guid profileId, int start, ResourceRequestResult result)
        {
            int delta = ActiveDecrement(start);
            ResourceRequestTrack outValue;
            if (!mActive.TryRemove(profileId, out outValue))
                return;

            Complete(outValue, TimeSpan.FromMilliseconds(delta));
        }

        internal void Retry(string name, Guid profileId, int start, ResourceRetryReason reason)
        {
            ErrorIncrement();
            int delta = ConversionHelper.CalculateDelta(Environment.TickCount, start);
            ResourceRequestTrack outValue;
            if (!mActive.TryGetValue(profileId, out outValue))
                return;

            outValue.RetrySignal(delta, reason);
        }

        private void Complete(ResourceRequestTrack outValue, TimeSpan delta)
        {
            
        }
    }
}
