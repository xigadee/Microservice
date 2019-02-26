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
        }
        #endregion

        #region ProtocolId
        /// <summary>
        /// The default is UDP for this agent.
        /// </summary>
        public override string ProtocolId { get; } = "Udp";
        #endregion

        protected override void ListenerClientStart(ListenerPartitionConfig c)
        {
            if (mConfig.ContainsKey(c.Priority))
            {
                var client = new UdpClientHolder(mConfig[c.Priority], CommunicationAgentCapabilities.Listener);
                client.Priority = c.Priority;
                mListenerClients.AddOrUpdate(c.Priority, client, (i, ct) => client);
                client.Start();
            };
        }


        protected override void ListenerClientValidate(IClientHolderV2 client, List<MessageFilterWrapper> newList)
        {
            //throw new NotImplementedException();
        }

        public override void SenderStart(SenderPartitionConfig p)
        {
            if (!mConfig.ContainsKey(p.Priority))
                throw new ArgumentOutOfRangeException($"Udp configuration is not defined for partition priority {p.Priority}");

            var config = mConfig[p.Priority];

            var client = new UdpClientHolder(config, CommunicationAgentCapabilities.Sender);
            client.Priority = p.Priority;
            mSenderClients.AddOrUpdate(client.Priority, client, (i, ct) => client);
            client.Start();
        }

        public override void SenderStop(IClientHolderV2 client)
        {
            client.Stop();
        }

        protected override void ListenerClientStop(IClientHolderV2 client)
        {
            client.Stop();
        }
    }
}
