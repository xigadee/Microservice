#region using
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#endregion
namespace Xigadee
{
    /// <summary>
    /// This class holds the specific statistics for the Queue Tracker.
    /// </summary>
    public class QueueTrackerStatistics: MessagingStatistics
    {

        /// <summary>
        /// Gets or sets the queue priority level.
        /// </summary>
        public int Level { get; set; }
        
        /// <summary>
        /// This is the high water mark for the queue.
        /// </summary>
        public int MaxWaiting { get; protected set; } = -1;

        /// <summary>
        /// This is the time stamp when the high water mark was reached for the queue.
        /// </summary>
        public DateTime? MaxWaitingTimeStamp { get; protected set; }

        #region Waiting
        /// <summary>
        /// This is the number of messages currently in the queue.
        /// </summary>
        public int Waiting
        {
            get;set;
        }
        #endregion

        /// <summary>
        /// This method sets the high water mark for the queue statistics.
        /// </summary>
        /// <param name="count">The current count.</param>
        public void WaitingSet(int count)
        {
            if (count == 0 || count <= MaxWaiting)
                return;

            MaxWaiting = count;
            MaxWaitingTimeStamp = DateTime.UtcNow;
        }

        /// <summary>
        /// This is the message logged for simple loggers.
        /// </summary>
        public override string Message
        {
            get
            {
                return $"{Level}>{Waiting}/{MaxWaiting}";
            }
            set
            {
            }
        }
    }
}
