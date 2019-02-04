using System;

namespace Xigadee
{
    /// <summary>
    /// This class is used to signal generically entity change information from the persistence store.
    /// </summary>
    public class EntityChangeEventArgs: EventArgs
    {
        /// <summary>
        /// This is the entity action, i.e. Create, Update, Delete etc.
        /// </summary>
        public string ActionType { get; set; }
        /// <summary>
        /// This is the entity type name used to route to the persistence command
        /// </summary>
        public string EntityType { get; set; }
        /// <summary>
        /// This is the entity key as a string
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// This is the version id of the entity if it is available
        /// </summary>
        public string VersionId { get; set; }
        /// <summary>
        /// This is the key type, i.e. string, int, etc.
        /// </summary>
        public string KeyType { get; set; }
        /// <summary>
        /// This is the id of the incoming request
        /// </summary>
        public Guid TraceId { get; set; }
        /// <summary>
        /// This is the originating message id.
        /// </summary>
        public string OriginatorKey { get; set; }
        /// <summary>
        /// This is the originating process id.
        /// </summary>
        public string ProcessCorrelationKey { get; set; }

    }

}
