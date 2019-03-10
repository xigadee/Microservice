using System;

namespace Xigadee
{
    /// <summary>
    /// This is the default command policy for the persistence class.
    /// </summary>
    public class PersistenceCommandPolicy:CommandPolicy
    {
        /// <summary>
        /// Persistence managers should not be master jobs by default.
        /// </summary>
        public override bool MasterJobEnabled { get { return false; } }
        /// <summary>
        /// This is the default time allowed when making a call to the underlying persistence layer.
        /// </summary>
        public TimeSpan? DefaultTimeout { get; set; }
        /// <summary>
        /// The resource consumer 
        /// </summary>
        public IResourceConsumer ResourceConsumer { get; set; }
        /// <summary>
        /// Specifies the persistence retry policy
        /// </summary>
        public PersistenceRetryPolicy PersistenceRetryPolicy { get; set; }
        /// <summary>
        /// This is the resource profile for the persistence manager.
        /// </summary>
        public ResourceProfile ResourceProfile { get; set; }
    }
}
