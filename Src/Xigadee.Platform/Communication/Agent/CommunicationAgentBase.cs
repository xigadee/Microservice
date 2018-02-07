using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="S"></typeparam>
    /// <seealso cref="Xigadee.ServiceBase{S}" />
    /// <seealso cref="Xigadee.IListener" />
    /// <seealso cref="Xigadee.ISender" />
    public abstract partial class CommunicationAgentBase<S>: ServiceBase<S>, IListener, ISender
        where S : StatusBase, new()
    {
        #region Declarations
        private object syncObject = new object();
        /// <summary>
        /// This is the supported list of message types for the client.
        /// </summary>
        protected List<MessageFilterWrapper> mSupportedMessageTypes = new List<MessageFilterWrapper>();
        #endregion

        protected CommunicationAgentBase()
        {

        }

        public CommunicationAgentCapabilities Capabilities { get; protected set; }

        protected override void StartInternal()
        {
            throw new NotImplementedException();
        }

        protected override void StopInternal()
        {
            throw new NotImplementedException();
        }

        public List<ListenerPartitionConfig> PriorityPartitions { get; set; }

        List<SenderPartitionConfig> ISender.PriorityPartitions { get; set; }

        #region SupportsChannel(string channel)
        /// <summary>
        /// This method compares the channel and returns true if it is supported.
        /// </summary>
        /// <param name="channel"></param>
        /// <returns>Returns true if the channel is supported.</returns>
        public bool SupportsChannel(string channel)
        {
            return string.Equals(channel, ChannelId, StringComparison.InvariantCultureIgnoreCase);
        }
        #endregion

        #region BoundaryLoggingActive
        /// <summary>
        /// This property specifies whether the boundary logger is active.
        /// </summary>
        public bool? BoundaryLoggingActive { get; set; }
        #endregion
        #region ChannelId
        /// <summary>
        /// This is the ChannelId for the messaging service
        /// </summary>
        public string ChannelId
        {
            get;
            set;
        }
        #endregion

        #region LogExceptionLocation(string method)
        /// <summary>
        /// This helper method provides a class name and method name for debugging exceptions. 
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="ex">The exception.</param>
        /// <returns>A combination string.</returns>
        protected void LogExceptionLocation(string method, Exception ex)
        {
            Collector?.LogException($"{GetType().Name}/{method}", ex);
        }
        #endregion

        #region ServiceHandlers
        /// <summary>
        /// This is the system wide service handlers.
        /// </summary>
        public IServiceHandlers ServiceHandlers { get; set; }
        #endregion
        #region OriginatorId
        /// <summary>
        /// The originator Id for the service.
        /// </summary>
        public MicroserviceId OriginatorId { get; set; }
        #endregion
        #region Collector
        /// <summary>
        /// This is the data collector.
        /// </summary>
        public IDataCollection Collector { get; set; }
        #endregion
        #region SharedServices
        /// <summary>
        /// The shared service container.
        /// </summary>
        public ISharedService SharedServices { get; set; } 
        #endregion
    }
}
