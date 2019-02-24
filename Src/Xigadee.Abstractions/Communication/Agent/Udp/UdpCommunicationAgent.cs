using System;
using System.Collections.Generic;

namespace Xigadee
{
    /// <summary>
    /// This agent is used to handle Udp communication.
    /// </summary>
    /// <seealso cref="Xigadee.CommunicationAgentBase" />
    public class UdpCommunicationAgent: CommunicationAgentBase
    {
        (int priority, UdpConfig config)[] mConfig;

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="UdpCommunicationAgent"/> class.
        /// </summary>
        /// <param name="config">The UDP endpoint configuration.</param>
        /// <param name="shcIds">The optional ServiceHandlerIdCollection identifiers.</param>
        /// <param name="requestAddress">The optional request address.</param>
        /// <param name="responseAddress">The optional response address.</param>
        /// <param name="requestAddressPriority">The optional request address priority.</param>
        /// <param name="responseAddressPriority">The optional response address priority.</param>
        /// <param name="capabilities">The agent capabilities. The default is bidirectional.</param>
        /// <param name="maxUdpMessagePayloadSize">Maximum size of the UDP message payload.</param>
        public UdpCommunicationAgent(UdpConfig config
            , CommunicationAgentCapabilities capabilities = CommunicationAgentCapabilities.Bidirectional
            , ServiceHandlerIdCollection shcIds = null
            , ServiceMessageHeaderFragment requestAddress = null
            , ServiceMessageHeader responseAddress = null
            , int? requestAddressPriority = null
            , int responseAddressPriority = 1
            , int? maxUdpMessagePayloadSize = UdpConfig.PacketMaxSize
            ) : base(capabilities, shcIds)
        {
            if (config == null)
                throw new ArgumentNullException("config", "Udp configuration cannot be null.");

            mConfig = new[] { (requestAddressPriority ?? 1, config) };
        }
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
            if (configs == null)
                throw new ArgumentNullException("config", "Udp configuration cannot be null.");

            mConfig = configs;
        }
        #endregion

        #region ProtocolId
        /// <summary>
        /// The default is UDP for this agent.
        /// </summary>
        public override string ProtocolId { get; } = "Udp"; 
        #endregion

        protected override void ListenerClientsStart()
        {
            mConfig.ForEach((c) =>
                {
                    var client = new UdpClientHolder(c.config, CommunicationAgentCapabilities.Listener);
                    client.Priority = c.priority;
                    mListenerClients.AddOrUpdate(c.priority, client, (i, ct) => client);
                    client.Start();
                });
        }

        protected override void ListenerClientsStop()
        {
            //mListenerClients.ForEach((c) => c.Value.
        }

        protected override void ListenerClientValidate(IClientHolder client, List<MessageFilterWrapper> newList)
        {
            //throw new NotImplementedException();
        }

        public override void SenderStart()
        {
            mConfig.ForEach((c) =>
            {
                var client = new UdpClientHolder(c.config, CommunicationAgentCapabilities.Sender);
                client.Priority = c.priority;
                mSenderClients.AddOrUpdate(c.priority, client, (i, ct) => client);
                client.Start();
            });
        }

        public override void SenderStop()
        {
            base.SenderStop();
        }
    }
}
