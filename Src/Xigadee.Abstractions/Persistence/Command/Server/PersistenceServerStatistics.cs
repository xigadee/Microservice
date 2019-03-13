using System.Threading;
namespace Xigadee
{
    /// <summary>
    /// This is the root statistics class for a persistence handler.
    /// </summary>
    /// <seealso cref="Xigadee.CommandStatistics" />
    public class PersistenceServerStatistics: CommandStatistics
    {

        #region Name
        /// <summary>
        /// This is the messaging statistics name.
        /// </summary>
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
        #endregion

        #region RetryIncrement()
        private long mRetries;
        /// <summary>
        /// This method is used to increment the retry count.
        /// </summary>
        public virtual void RetryIncrement()
        {
            Interlocked.Increment(ref mRetries);
        }
        #endregion        
        /// <summary>
        /// Gets the retry count.
        /// </summary>
        public long Retries {get { return mRetries; } }

        /// <summary>
        /// Gets or sets the requests in play.
        /// </summary>
        public string[] RequestsInPlay { get; set; }
    }
}
