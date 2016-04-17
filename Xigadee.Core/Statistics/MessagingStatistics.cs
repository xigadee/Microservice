#region using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This is the base statistics class for commands that process messages.
    /// </summary>
    public class MessagingStatistics:StatusBase
    {
        #region Declarations
        private StatsContainer mStatsDefault;
        #endregion
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public MessagingStatistics():base()
        {
            mStatsDefault = new StatsContainer();
        }
        #endregion

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

        #region ErrorIncrement()
        /// <summary>
        /// This method is used to increment the active and total record count.
        /// </summary>
        public virtual void ErrorIncrement()
        {
            mStatsDefault.ErrorIncrement();
        }
        #endregion
        #region ActiveIncrement()
        /// <summary>
        /// This method is used to increment the active and total record count.
        /// </summary>
        public virtual int ActiveIncrement()
        {
            return mStatsDefault.ActiveIncrement();
        }
        #endregion
        #region ActiveDecrement(long delta)
        /// <summary>
        /// This method is used to decrement the active count and submits the processing time.
        /// </summary>
        /// <param name="delta">The processing time in milliseconds.</param>
        public virtual int ActiveDecrement(int start)
        {
            return mStatsDefault.ActiveDecrement(start);
        }
        #endregion
        #region ActiveDecrement(TimeSpan delta)
        /// <summary>
        /// This method is used to decrement the active count and submits the processing time.
        /// </summary>
        /// <param name="delta">The processing time in milliseconds.</param>
        public virtual int ActiveDecrement(TimeSpan extent)
        {
            return mStatsDefault.ActiveDecrement(extent);
        }
        #endregion

        #region Message
        /// <summary>
        /// This is the message logged for simple loggers.
        /// </summary>
        public override string Message
        {
            get
            {
                return mStatsDefault.ToString();
            }
            set
            {
                base.Message = value;
            }
        }
        #endregion
        #region Batches
        /// <summary>
        /// This is the message logged for simple loggers.
        /// </summary>
        public virtual string[] Batches
        {
            get
            {
                return mStatsDefault.Batches;
            }
        }

        /// <summary>
        /// This is the slowest batch.
        /// </summary>
        public string BatchSlow { get { return mStatsDefault.BatchSlow; } }
        /// <summary>
        /// This is the fastest batch.
        /// </summary>
        public string BatchFast { get { return mStatsDefault.BatchFast; } }
        #endregion
    }
}
