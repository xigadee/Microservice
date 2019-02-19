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
        UdpConfig mConfig;

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
            mConfig = config ?? throw new ArgumentNullException("config", "Udp configuration cannot be null.");
        } 
        #endregion


        protected override void ListenerClientsStart()
        {
            //throw new NotImplementedException();
        }

        protected override void ListenerClientsStop()
        {
            //throw new NotImplementedException();
        }

        protected override void ListenerClientValidate(IClientHolder client, List<MessageFilterWrapper> newList)
        {
            //throw new NotImplementedException();
        }

    }
}
