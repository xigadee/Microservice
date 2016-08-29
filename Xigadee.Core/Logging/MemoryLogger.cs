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
    /// This class is used to hold the event at the specific logging level.
    /// </summary>
    public class LogEventLevelHolder:IEnumerable<LogEvent>
    {
        /// <summary>
        /// This event is raised when the service start begins
        /// </summary>
        public event EventHandler<LogEvent> OnLogEvent;

        #region Declarations
        /// <summary>
        /// This is the queue that holds the data in memory.
        /// </summary>
        ConcurrentQueue<LogEvent> mQueue = new ConcurrentQueue<LogEvent>();

        int mCapacity;
        long mLogEvents = 0;
        long mLogEventsExpired = 0;

        #endregion
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="level">The logging level for the container.</param>
        /// <param name="capacity">The maximum capacity for the container.</param>
        public LogEventLevelHolder(LoggingLevel level, int capacity = 2000)
        {
            Level = level;
            mCapacity = capacity;
        } 
        #endregion

        /// <summary>
        /// This is the level currently being recorded by this class.
        /// </summary>
        public LoggingLevel Level { get; }
        /// <summary>
        /// This is teh number of events currently in the collection.
        /// </summary>
        public long Count { get { return mLogEvents; } }
        /// <summary>
        /// This is the number of events that have expired from the collection.
        /// </summary>
        public long CountExpired { get { return mLogEventsExpired; } }
        /// <summary>
        /// This is the total number of events that have passed through the collection.
        /// </summary>
        public long CountTotal { get { return mLogEvents+mLogEventsExpired; } }

        #region Log(LogEvent logEvent)
        /// <summary>
        /// This method logs the event.
        /// </summary>
        /// <param name="logEvent">The event to log.</param>
        public async Task Log(LogEvent logEvent)
        {
            mQueue.Enqueue(logEvent);

            Interlocked.Increment(ref mLogEvents);

            try
            {
                OnLogEvent?.Invoke(Level, logEvent);
            }
            catch (Exception) { }

            while (mQueue.Count > mCapacity)
            {
                LogEvent oldEvent;
                if (!mQueue.TryDequeue(out oldEvent))
                    break;

                Interlocked.Increment(ref mLogEventsExpired);
            }
        }
        #endregion

        public IEnumerator<LogEvent> GetEnumerator()
        {
            return mQueue?.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return mQueue?.GetEnumerator();
        }
    }

    /// <summary>
    /// This logger can be used for diagnotic purposes, and will hold a set of logger messages in memory, based on the 
    /// size parameter passed through in the constructor.
    /// </summary>
    public class MemoryLogger: ServiceBase<LoggingStatistics>, ILogger, IServiceOriginator
    {
        #region Declarations
        /// <summary>
        /// This is the queue that holds the data in memory.
        /// </summary>
        Dictionary<LoggingLevel, LogEventLevelHolder> mHolders; 

        Func<LoggingLevel, int> mCapacityCalculator;

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
            mCapacityCalculator = capacityCalculator;
        }

        /// <summary>
        /// The service originator.
        /// </summary>
        public string OriginatorId
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
            //Create a dictionary for each specific level.
            mHolders = 
                Enum.GetValues(typeof(LoggingLevel))
                    .Cast<LoggingLevel>()
                    .ToDictionary((l) => l, (l) => new LogEventLevelHolder(l, mCapacityCalculator(l)));
        }

        protected override void StopInternal()
        {
            mHolders.Clear();
            mHolders = null;
        }

        /// <summary>
        /// This method returns the holder.
        /// If the logger has not yet started, then this method will return null.
        /// </summary>
        /// <param name="level">The logging level requested.</param>
        /// <returns>Returns the logging level container.</returns>
        public LogEventLevelHolder Holder(LoggingLevel level)
        {
            return mHolders?[level];
        }
    }
}
