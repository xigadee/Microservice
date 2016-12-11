#region Copyright
// Copyright Hitachi Consulting
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;

namespace Xigadee
{
    /// <summary>
    /// This handler holds and tracks commands to the persistence agent.
    /// </summary>
    public class PersistenceHandler:CommandHandler<PersistenceHandlerStatistics>
    {
        private ConcurrentDictionary<int, PersistenceResponseStatisticsHolder> mResponses = new ConcurrentDictionary<int, PersistenceResponseStatisticsHolder>();

        public virtual void Record<KT, ET>(PersistenceRequestHolder<KT, ET> holder)
        {
            int responseCode = holder.Rs?.ResponseCode ?? 0;

            var stats = mResponses.GetOrAdd(responseCode, new PersistenceResponseStatisticsHolder(responseCode));
            stats.Record(holder.Extent, holder.Rs);
        }

        protected override void StatisticsRecalculate(PersistenceHandlerStatistics stats)
        {
            base.StatisticsRecalculate(stats);

            stats.Responses = mResponses.Values.ToArray();
        }
    }

    public class PersistenceHandlerStatistics: CommandHandlerStatistics
    {
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

        public PersistenceResponseStatisticsHolder[] Responses { get; set; }
    }

    public class PersistenceResponseStatisticsHolder: MessagingStatistics
    {
        private long mRetries = 0;
        private long mHitCount = 0;
        private long mCacheHits = 0;

        public PersistenceResponseStatisticsHolder(int status)
        {
            Status = status;
        }

        public int Status { get; }

        public long Retries { get { return mRetries; } }

        public long CacheHits { get { return mRetries; } }

        public void Record<KT, ET>(TimeSpan? extent, PersistenceRepositoryHolder<KT, ET> rs)
        {
            if (extent.HasValue)
            {
                ActiveIncrement();
                ActiveDecrement(extent.Value);
            }

            if (rs.IsRetry)
            {
                Interlocked.Increment(ref mRetries);
            }

            if (rs.IsCached)
            {
                Interlocked.Increment(ref mCacheHits);
            }

            Interlocked.Add(ref mRetries, rs?.Retry??0);
        }
    }
}
