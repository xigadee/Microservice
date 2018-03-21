using System;
using System.Collections.Generic;

namespace Xigadee
{
    #region CommunicationAgentBase
    /// <summary>
    /// This is the base communication agent class.
    /// </summary>
    public abstract class CommunicationAgentBase: CommunicationAgentBase<CommunicationAgentStatistics>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommunicationAgentBase"/> class.
        /// </summary>
        /// <param name="capabilities">The agent capabilities. The default is bidirectional.</param>
        /// <param name="serializerId">The optional preferred serializer identifier.</param>
        /// <param name="compressionId">The optional preferred compression identifier.</param>
        /// <param name="encryptionId">The optional preferred encryption identifier.</param>
        protected CommunicationAgentBase(
            CommunicationAgentCapabilities capabilities = CommunicationAgentCapabilities.Bidirectional
            , SerializationHandlerId serializerId = null
            , CompressionHandlerId compressionId = null
            , EncryptionHandlerId encryptionId = null):base(capabilities, serializerId, compressionId, encryptionId)
        {
        }
    } 
    #endregion

    /// <summary>
    /// This is the base communication agent class.
    /// </summary>
    /// <typeparam name="S"></typeparam>
    /// <seealso cref="Xigadee.ServiceBase{S}" />
    /// <seealso cref="Xigadee.IListener" />
    /// <seealso cref="Xigadee.ISender" />
    public abstract partial class CommunicationAgentBase<S>: ServiceBase<S>
        where S : CommunicationAgentStatistics, new()
    {
        #region Declarations
        private object syncObject = new object();
        /// <summary>
        /// This is the supported list of message types for the client.
        /// </summary>
        protected List<MessageFilterWrapper> mSupportedMessageTypes = new List<MessageFilterWrapper>();
        #endregion
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="CommunicationAgentBase{S}"/> class.
        /// </summary>
        /// <param name="capabilities">The agent capabilities. The default is bidirectional.</param>
        /// <param name="serializerId">The optional preferred serializer identifier.</param>
        /// <param name="compressionId">The optional preferred compression identifier.</param>
        /// <param name="encryptionId">The optional preferred encryption identifier.</param>
        protected CommunicationAgentBase(
            CommunicationAgentCapabilities capabilities = CommunicationAgentCapabilities.Bidirectional
            , SerializationHandlerId serializerId = null
            , CompressionHandlerId compressionId = null
            , EncryptionHandlerId encryptionId = null)
        {
            Capabilities = capabilities;
            PreferredSerializerId = serializerId;
            PreferredCompressionId = compressionId;
            PreferredEncryptionId = encryptionId;
        } 
        #endregion

        /// <summary>
        /// Gets or sets the preferred serializer identifier.
        /// </summary>
        public SerializationHandlerId PreferredSerializerId { get; protected set; }
        /// <summary>
        /// Gets or sets the preferred compression identifier.
        /// </summary>
        public CompressionHandlerId PreferredCompressionId { get; protected set; }
        /// <summary>
        /// Gets or sets the preferred encryption identifier.
        /// </summary>
        public EncryptionHandlerId PreferredEncryptionId { get; protected set; }

        #region Capabilities/CanListen/CanSend
        /// <summary>
        /// Override this to specify your agents capabilities.
        /// </summary>
        public virtual CommunicationAgentCapabilities Capabilities { get; protected set;}
        /// <summary>
        /// Gets a value indicating whether this agent can listen for message.
        /// </summary>
        public bool CanListen => (Capabilities & CommunicationAgentCapabilities.Listener) > 0;
        /// <summary>
        /// Gets a value indicating whether this agent can send messages.
        /// </summary>
        public bool CanSend => (Capabilities & CommunicationAgentCapabilities.Sender) > 0;
        #endregion

        #region SettingsValidate()
        /// <summary>
        /// This method validates the configuration and settings for the connection.
        /// </summary>
        protected virtual void SettingsValidate()
        {
            if (ChannelId == null)
                throw new CommunicationAgentStartupException("ChannelId", "ChannelId cannot be null");

            ListenerSettingsValidate();

            SenderSettingsValidate();
        }
        #endregion

        #region Start/Stop
        /// <summary>
        /// This method starts the agent. 
        /// </summary>
        protected override void StartInternal()
        {
            SettingsValidate();

            if (CanSend)
            {
                SenderSettingsValidate();
                SenderStart();
            }

            if (CanListen)
            {
                ListenerSettingsValidate();
                ListenerStart();
            }
        }
        /// <summary>
        /// This method stops the agent.
        /// </summary>
        protected override void StopInternal()
        {
            if (CanListen)
                ListenerStop();
            if (CanSend)
                SenderStop();
        } 
        #endregion

        #region SupportsChannel(string channel)
        /// <summary>
        /// This method compares the channel and returns true if it is supported.
        /// </summary>
        /// <param name="channel"></param>
        /// <returns>Returns true if the channel is supported.</returns>
        public bool SenderSupportsChannel(string channel)
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
