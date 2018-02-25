using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;
using System.Collections;

namespace Xigadee
{
    /// <summary>
    /// This class is used to hold the memory event at the specific logging level.
    /// </summary>
    public class MemoryLogEventLevelHolder: IEnumerable<LogEvent>
    {
        #region Events
        /// <summary>
        /// This event is raised when the service start begins
        /// </summary>
        public event EventHandler<LogEvent> OnLogEvent; 
        #endregion
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
        public MemoryLogEventLevelHolder(LoggingLevel level, int capacity = 2000)
        {
            Level = level;
            mCapacity = capacity;
        }
        #endregion

        #region Log(LogEvent logEvent)
        /// <summary>
        /// This method logs the event.
        /// </summary>
        /// <param name="logEvent">The event to log.</param>
        public Task Log(LogEvent logEvent)
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

            return Task.CompletedTask;
        }
        #endregion        

        /// <summary>
        /// This is the level currently being recorded by this class.
        /// </summary>
        public LoggingLevel Level { get; }
        /// <summary>
        /// This is the number of events currently in the collection.
        /// </summary>
        public long Count => mLogEvents;
        /// <summary>
        /// This is the number of events that have expired from the collection.
        /// </summary>
        public long CountExpired => mLogEventsExpired;
        /// <summary>
        /// This is the total number of events that have passed through the collection.
        /// </summary>
        public long CountTotal => mLogEvents + mLogEventsExpired;
        /// <summary>
        /// Returns an enumerator that iterates through the queue events collection.
        /// </summary>
        /// <returns>
        /// An enumerator that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<LogEvent> GetEnumerator() => mQueue?.GetEnumerator();
        /// <summary>
        /// Returns an enumerator that iterates through a event collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator() => mQueue?.GetEnumerator();

    }
}
