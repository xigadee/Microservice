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
