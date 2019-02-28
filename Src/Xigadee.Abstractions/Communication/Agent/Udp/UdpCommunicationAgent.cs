using System;
using System.Linq;
using System.Collections.Generic;

namespace Xigadee
{
    /// <summary>
    /// This agent is used to handle Udp communication.
    /// </summary>
    /// <seealso cref="Xigadee.CommunicationAgentBase" />
    public class UdpCommunicationAgent: CommunicationAgentBase
    {
        Dictionary<int, UdpConfig> mConfig;

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="UdpCommunicationAgent"/> class.
        /// </summary>
        /// <param name="configs">The UDP endpoint configurations.</param>
        /// <param name="shcIds">The optional ServiceHandlerIdCollection identifiers.</param>
        /// <param name="requestAddress">The optional request address.</param>
        /// <param name="responseAddress">The optional response address.</param>
        /// <param name="requestAddressPriority">The optional request address priority.</param>
        /// <param name="responseAddressPriority">The optional response address priority.</param>
        /// <param name="capabilities">The agent capabilities. The default is bidirectional.</param>
        /// <param name="maxUdpMessagePayloadSize">Maximum size of the UDP message payload.</param>
        public UdpCommunicationAgent((int priority,UdpConfig config)[] configs
            , CommunicationAgentCapabilities capabilities = CommunicationAgentCapabilities.Bidirectional
            , ServiceHandlerIdCollection shcIds = null
            , ServiceMessageHeaderFragment requestAddress = null
            , ServiceMessageHeader responseAddress = null
            , int? requestAddressPriority = null
            , int responseAddressPriority = 1
            , int? maxUdpMessagePayloadSize = UdpConfig.PacketMaxSize
            ) : base(capabilities, shcIds)
        {
            mConfig = configs?.ToDictionary((c) => c.priority, (c) => c.config)
                ?? throw new ArgumentNullException("config", "Udp configuration cannot be null.");

            RequestAddress = (requestAddress, requestAddressPriority);

            ResponseAddress = (responseAddress, responseAddressPriority);

            MaxUdpMessagePayloadSize = maxUdpMessagePayloadSize;
        }
        #endregion

        #region ProtocolId
        /// <summary>
        /// The default is UDP for this agent.
        /// </summary>
        public override string ProtocolId { get; } = "Udp";
        #endregion

        /// <summary>
        /// This is the default address for the incoming Udp message.
        /// </summary>
        protected (ServiceMessageHeaderFragment address, int? priority) RequestAddress { get; }
        /// <summary>
        /// This is the optional response message for the incoming Udp message.
        /// </summary>
        protected (ServiceMessageHeader address, int? priority) ResponseAddress{ get; }
        /// <summary>
        /// The maximum payload message size.
        /// </summary>
        protected int? MaxUdpMessagePayloadSize { get; }

        #region ListenerClientCreate(ListenerPartitionConfig c)
        /// <summary>
        /// This override creates the UDP listener client.
        /// </summary>
        /// <param name="c">The partition configuration.</param>
        /// <returns>Returns the client.</returns>
        protected override IClientHolderV2 ListenerClientCreate(ListenerPartitionConfig c)
        {
            if (!mConfig.ContainsKey(c.Priority))
            {
                var err = $"The Udp Listener client configuration is not defined for priority {c.Priority}";
                Collector?.LogWarning(err);
                throw new ArgumentOutOfRangeException(err);
            }

            return new UdpClientHolder(mConfig[c.Priority]
                , CommunicationAgentCapabilities.Listener
                , RequestAddress
                , ResponseAddress
                , MaxUdpMessagePayloadSize
                );
        } 
        #endregion

        protected override void ListenerClientValidate(IClientHolderV2 client, List<MessageFilterWrapper> newList)
        {
            //throw new NotImplementedException();
        }

        #region SenderCreate(SenderPartitionConfig c)
        /// <summary>
        /// This override creates the UdpClient
        /// </summary>
        /// <param name="c">The partition configuration</param>
        /// <returns>Returns the client.</returns>
        protected override IClientHolderV2 SenderCreate(SenderPartitionConfig c)
        {
            if (!mConfig.ContainsKey(c.Priority))
            {
                var err = $"The Udp sender client configuration is not defined for priority {c.Priority}";
                Collector?.LogWarning(err);
                throw new ArgumentOutOfRangeException(err);
            }

            return new UdpClientHolder(mConfig[c.Priority]
                , CommunicationAgentCapabilities.Sender
                , RequestAddress
                , ResponseAddress
                , MaxUdpMessagePayloadSize
                );
        } 
        #endregion

    }
}
