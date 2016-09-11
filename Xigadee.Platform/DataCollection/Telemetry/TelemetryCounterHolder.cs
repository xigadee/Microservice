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
    /// The message counter holder.
    /// </summary>
    [DebuggerDisplay("[{Debug}]/[DebugReset]")]
    public class TelemetryCounterHolder
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

        public TelemetryCounterHolder(string key)
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
}
