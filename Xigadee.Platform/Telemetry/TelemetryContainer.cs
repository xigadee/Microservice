#region using
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

#endregion
namespace Xigadee
{
    /// <summary>
    /// This collection contains the Telemetry modules.
    /// </summary>
    public class TelemetryContainer : CollectionContainerBase<ITelemetry>
    {
        #region Class -> MessageCounterHolder
        /// <summary>
        /// The message counter holder.
        /// </summary>
        [DebuggerDisplay("[{Debug}]/[DebugReset]")]
        private class MessageCounterHolder
        {
            private string mKey;

            private long mCount;
            private long mTotalDelta;

            private long mSuccesses;
            private long mExceptions;

            public DateTime Start { get; private set; }

            public string Key { get; private set; }

            public long SuccessfulMessages { get { return mSuccesses; } }
            
            public long FailedMessages { get { return mExceptions; } }

            public TimeSpan AverageDelta
            {
                get
                {
                    return TimeSpan.FromTicks(mCount == 0 ? 0 : mTotalDelta / mCount);
                }
            }

            public MessageCounterHolder(string key)
            {
                Key = key;
                Start = DateTime.UtcNow;
            }

            public void Log(int delta, bool isSuccess, bool isException)
            {
                Interlocked.Increment(ref mCount);
                Interlocked.Add(ref mTotalDelta, delta);

                if (isSuccess)
                {
                    Interlocked.Increment(ref mSuccesses);
                }

                if (isException)
                {
                    Interlocked.Increment(ref mExceptions);
                }
            }
        } 

        #endregion
        
        #region Declarations
        /// <summary>
        /// This holds a collection of counters used to identify performance bottlenecks.
        /// </summary>
        ConcurrentDictionary<string, MessageCounterHolder> mStatusTracker;

        #endregion

        #region Constructor
        public TelemetryContainer(IEnumerable<ITelemetry> telemetry)
            : base(telemetry)
        {
            mStatusTracker = new ConcurrentDictionary<string, MessageCounterHolder>();
        } 
        #endregion

        public void Log(string key, int delta, bool isSuccess)
        {
            var holder = mStatusTracker.GetOrAdd(key, (k) => new MessageCounterHolder(k));

            holder.Log(delta, isSuccess, !isSuccess);
        }

        #region FlushLogs()
        /// <summary>
        /// This method flushes log information out to the telemetries and resets the status tracker
        /// </summary>
        public async Task Flush()
        {
            try
            {
                var existingStatusTracker = Interlocked.Exchange(ref mStatusTracker, new ConcurrentDictionary<string, MessageCounterHolder>());
                var messageCounters = existingStatusTracker.Values.ToList();
                Items.ForEach(t => messageCounters.ForEach(mc => LogTelemetry(t, mc)));            
                existingStatusTracker.Clear();
            }
            catch (Exception ex)
            {

            }

        }
        #endregion

        private static void LogTelemetry(ITelemetry telemetry, MessageCounterHolder messageCounterHolder)
        {
            telemetry.TrackMetric(string.Format("{0}-AverageDelta", messageCounterHolder.Key), messageCounterHolder.AverageDelta.TotalMilliseconds);
            telemetry.TrackMetric(string.Format("{0}-SuccessfulMessages", messageCounterHolder.Key), messageCounterHolder.SuccessfulMessages);
            telemetry.TrackMetric(string.Format("{0}-FailedMessages", messageCounterHolder.Key), messageCounterHolder.FailedMessages);            
        }
    }
}
