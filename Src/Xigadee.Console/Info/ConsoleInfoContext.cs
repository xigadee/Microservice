using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Xigadee
{
    /// <summary>
    /// This context holds the logging information for the console.
    /// </summary>
    public class ConsoleInfoContext
    {
        /// <summary>
        /// The internal count.
        /// </summary>
        protected long mCount = 0;

        /// <summary>
        /// This is the number of info messages that have been logged.
        /// </summary>
        public long Count { get { return mCount; } }

        /// <summary>
        /// Adds the specified information.
        /// </summary>
        /// <param name="info">The information logging message.</param>
        public void Add(ErrorInfo info)
        {
            info.LoggingId = Interlocked.Increment(ref mCount) -1;
            InfoMessages.Add(info);
            InfoCurrent = info.LoggingId;
        }
        /// <summary>
        /// Adds the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="type">The logging type. The default value is 'Info'</param>
        public void Add(string message, LoggingLevel type = LoggingLevel.Info)
        {
            Add(new ErrorInfo { Message = message, Type = type });
        }

        /// <summary>
        /// Gets the info messages for the specified number of entries.
        /// </summary>
        /// <param name="count">The number of records needed.</param>
        /// <returns>Returns an enumeration.</returns>
        public IEnumerable<ErrorInfo> GetCurrent(int count)
        {
            if (InfoCurrent < (count-1))
                InfoCurrent = count-1;

            var result =  InfoMessages
                .OrderByDescending((i) => i.LoggingId)
                .Where((c) => c.LoggingId <= InfoCurrent)
                .Take(count)
                .ToList();

            return result;
        }
        /// <summary>
        /// This is the list of info messages.
        /// </summary>
        public ConcurrentBag<ErrorInfo> InfoMessages { get; } = new ConcurrentBag<ErrorInfo>();

        /// <summary>
        /// Decrements the current position.
        /// </summary>
        /// <returns>Returns false if the last position has been reached.</returns>
        public bool InfoDecrement()
        {
            if (InfoCurrent == 0)
                return false;

            InfoCurrent--;

            return true;
        }
        /// <summary>
        /// Increments the current position.
        /// </summary>
        /// <returns>Returns false if the maximum position has been reached.</returns>
        public bool InfoIncrement()
        {
            if (InfoCurrent == Count - 1)
                return false;

            InfoCurrent++;

            return true;
        }
        /// <summary>
        /// Gets or sets the current position.
        /// </summary>
        public long InfoCurrent { get; set; }

        /// <summary>
        /// Specifies the default behaviour on whether the display should refresh when an item is added.
        /// </summary>
        public bool Refresh { get; set; }
    }
}
