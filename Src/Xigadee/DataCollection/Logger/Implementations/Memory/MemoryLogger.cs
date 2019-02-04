using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace Xigadee
{
    /// <summary>
    /// This logger can be used for diagnostic purposes, and will hold a set of logger messages in memory, based on the 
    /// size parameter passed through in the constructor.
    /// </summary>
    [Obsolete]
    public class MemoryLogger: ServiceBase<LoggingStatistics>, IXigadeeLogger, IRequireServiceOriginator
    {
        #region Declarations
        /// <summary>
        /// This is the queue that holds the data in memory.
        /// </summary>
        Dictionary<LoggingLevel, MemoryLogEventLevelHolder> mHolders; 

        long mLogEvents = 0;

        long mLogEventsExpired = 0;
        #endregion
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryLogger"/> class.
        /// </summary>
        public MemoryLogger() : this((l) => 2000)
        {
        } 
        #endregion

        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="capacityCalculator"></param>
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
        public MicroserviceId OriginatorId{get; set;}

        /// <summary>
        /// This method asynchronously logs an event.
        /// </summary>
        /// <param name="logEvent">The event to log.</param>
        /// <returns>
        /// This is an async task.
        /// </returns>
        public async Task Log(LogEvent logEvent)
        {
            await mHolders[logEvent.Level].Log(logEvent);

            Interlocked.Increment(ref mLogEvents);
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

        #region Unused start/stop code.
        /// <summary>
        /// This method starts the service.
        /// </summary>
        protected override void StartInternal()
        {
        }
        /// <summary>
        /// This method stops the service. 
        /// </summary>
        protected override void StopInternal()
        {
        } 
        #endregion

    }
}
