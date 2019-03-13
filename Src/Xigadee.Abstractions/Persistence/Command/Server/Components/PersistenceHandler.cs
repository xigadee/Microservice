using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;

namespace Xigadee
{
    /// <summary>
    /// This handler holds and tracks commands to the persistence agent.
    /// </summary>
    public class PersistenceHandler: CommandHandler<PersistenceHandler.ResponseStatistics>
    {
        private ConcurrentDictionary<int, ResponseCodeStatistics> mResponses = new ConcurrentDictionary<int, ResponseCodeStatistics>();

        #region Record<KT, ET>(PersistenceRequestHolder<KT, ET> holder)
        /// <summary>
        /// Records the specified holder response.
        /// </summary>
        /// <typeparam name="KT">The key type.</typeparam>
        /// <typeparam name="ET">The entity type.</typeparam>
        /// <param name="holder">The request holder.</param>
        public virtual void Record<KT, ET>(PersistenceRequestHolder<KT, ET> holder)
        {
            int responseCode = holder.Rs?.ResponseCode ?? 0;

            var stats = mResponses.GetOrAdd(responseCode, new ResponseCodeStatistics(responseCode));
            stats.Record(holder.Extent, holder.Rs);
        }
        #endregion

        #region StatisticsRecalculate(ResponseStatistics stats)
        /// <summary>
        /// This method recalculates the statistics for the command handler.
        /// </summary>
        /// <param name="stats">The stats to recalculate.</param>
        protected override void StatisticsRecalculate(ResponseStatistics stats)
        {
            base.StatisticsRecalculate(stats);

            stats.Responses = mResponses.Values.ToArray();
        } 
        #endregion

        #region Class -> ResponseStatistics
        /// <summary>
        /// This is the handler statistics class.
        /// </summary>
        /// <seealso cref="Xigadee.CommandHandlerStatistics" />
        public class ResponseStatistics: CommandHandlerStatistics
        {
            #region Name
            /// <summary>
            /// The override makes sure that name comes first in the output.
            /// </summary>
            public override string Name
            {
                get
                {
                    return base.Name;
                }

                set
                {
                    base.Name = value;
                }
            }
            #endregion
            /// <summary>
            /// Gets or sets the responses statistics.
            /// </summary>
            public ResponseCodeStatistics[] Responses { get; set; }
        }
        #endregion

        /// <summary>
        /// This is the specific command statistics holder for a specific status code..
        /// </summary>
        /// <seealso cref="Xigadee.MessagingStatistics" />
        public class ResponseCodeStatistics: MessagingStatistics
        {
            private long mRetries = 0;
            private long mHitCount = 0;
            private long mCacheHits = 0;

            /// <summary>
            /// Initializes a new instance of the <see cref="ResponseCodeStatistics"/> class.
            /// </summary>
            /// <param name="status">The status code.</param>
            public ResponseCodeStatistics(int status)
            {
                Status = status;
            }
            /// <summary>
            /// Gets the status code.
            /// </summary>
            public int Status { get; }
            /// <summary>
            /// Gets the retries.
            /// </summary>
            public long Retries { get { return mRetries; } }
            /// <summary>
            /// Gets the cache hits.
            /// </summary>
            public long CacheHits { get { return mRetries; } }
            /// <summary>
            /// Records the specified extent.
            /// </summary>
            /// <typeparam name="KT">The key type.</typeparam>
            /// <typeparam name="ET">The entity type.</typeparam>
            /// <param name="extent">The timespan extent.</param>
            /// <param name="rs">The response holder..</param>
            public void Record<KT, ET>(TimeSpan? extent, PersistenceRepositoryHolder<KT, ET> rs)
            {
                if (extent.HasValue)
                {
                    ActiveIncrement();
                    ActiveDecrement(extent.Value);
                }

                if (rs?.IsRetry ?? false)
                {
                    Interlocked.Increment(ref mRetries);
                }

                if (rs?.IsCacheHit ?? false)
                {
                    Interlocked.Increment(ref mCacheHits);
                }

                Interlocked.Add(ref mRetries, rs?.Retry ?? 0);
            }
        }

    }
}
