using System;
namespace Xigadee
{
    /// <summary>
    /// This base status class is used for logging job time statistics.
    /// </summary>
    public class StatusBase : EventBase
    {
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="name">The name to display in the status message.</param>
        public StatusBase(string name)
        {
            Name = name??GetType().Name;
            Created = DateTime.UtcNow;
        }
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public StatusBase()
        {
            Name = GetType().Name;
            Created = DateTime.UtcNow;
        }
        #endregion

        #region Name
        /// <summary>
        /// This is the service name
        /// </summary>
        public virtual string Name { get; set; }
        #endregion

        #region ComponentId
        /// <summary>
        /// This is the base component id.
        /// </summary>
        public Guid ComponentId { get; set; } 
        #endregion
        #region Created
        /// <summary>
        /// This is the time the service was created.
        /// </summary>
        public DateTime Created { get; set; }
        #endregion

        #region Ex
        /// <summary>
        /// This is the last exception recorded.
        /// </summary>
        public Exception Ex { get; set; } 
        #endregion

        #region Message
        /// <summary>
        /// This is the message logged for simple loggers.
        /// </summary>
        public virtual string Message
        {
            get
            {
                var now = DateTime.UtcNow;
                return string.Format("{0} @ {1} Uptime {2} "
                    , Name
                    , now
                    , now - Created);
            }
            set
            {
            }
        } 
        #endregion
    }
}
