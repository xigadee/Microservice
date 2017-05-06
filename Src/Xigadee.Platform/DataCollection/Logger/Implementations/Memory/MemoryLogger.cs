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
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Collections;

namespace Xigadee
{
    /// <summary>
    /// This logger can be used for diagnotic purposes, and will hold a set of logger messages in memory, based on the 
    /// size parameter passed through in the constructor.
    /// </summary>
    [Obsolete]
    public class MemoryLogger: ServiceBase<LoggingStatistics>, ILogger, IRequireServiceOriginator
    {
        #region Declarations
        /// <summary>
        /// This is the queue that holds the data in memory.
        /// </summary>
        Dictionary<LoggingLevel, MemoryLogEventLevelHolder> mHolders; 

        long mLogEvents = 0;

        long mLogEventsExpired = 0;
        #endregion

        public MemoryLogger():this((l) => 2000)
        {
        }

        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="capacity"></param>
        public MemoryLogger(Func<LoggingLevel, int> capacityCalculator)
        {
            //Create a dictionary for each specific level.
            mHolders =
                Enum.GetValues(typeof(LoggingLevel))
                    .Cast<LoggingLevel>()
                    .ToDictionary((l) => l, (l) => new MemoryLogEventLevelHolder(l, capacityCalculator(l)));

        }

        /// <summary>
        /// The service originator.
        /// </summary>
        public MicroserviceId OriginatorId
        {
            get; set;
        }

        public async Task Log(LogEvent logEvent)
        {
            await mHolders[logEvent.Level].Log(logEvent);

            Interlocked.Increment(ref mLogEvents);
        }

        protected override void StartInternal()
        {
        }

        protected override void StopInternal()
        {
        }

        /// <summary>
        /// This method returns the holder.
        /// If the logger has not yet started, then this method will return null.
        /// </summary>
        /// <param name="level">The logging level requested.</param>
        /// <returns>Returns the logging level container.</returns>
        public MemoryLogEventLevelHolder Holder(LoggingLevel level)
        {
            return mHolders?[level];
        }
    }
}
