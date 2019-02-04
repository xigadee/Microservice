using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This agent is used to handle Udp communication.
    /// </summary>
    /// <seealso cref="Xigadee.CommunicationAgentBase" />
    public class UdpCommunicationAgent: CommunicationAgentBase
    {
        UdpConfig mConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="UdpCommunicationAgent"/> class.
        /// </summary>
        /// <param name="config">The UDP endpoint configuration.</param>
        /// <param name="serializerId">The optional serializer identifier.</param>
        /// <param name="compressionId">The optional compression identifier.</param>
        /// <param name="encryptionId">The optional encryption identifier.</param>
        /// <param name="requestAddress">The optional request address.</param>
        /// <param name="responseAddress">The optional response address.</param>
        /// <param name="requestAddressPriority">The optional request address priority.</param>
        /// <param name="responseAddressPriority">The optional response address priority.</param>
        /// <param name="capabilities">The agent capabilities. The default is bidirectional.</param>
        /// <param name="maxUdpMessagePayloadSize">Maximum size of the UDP message payload.</param>
        public UdpCommunicationAgent(UdpConfig config
            , CommunicationAgentCapabilities capabilities = CommunicationAgentCapabilities.Bidirectional
            , SerializationHandlerId serializerId = null
            , CompressionHandlerId compressionId = null
            , EncryptionHandlerId encryptionId = null
            , ServiceMessageHeaderFragment requestAddress = null
            , ServiceMessageHeader responseAddress = null
            , int? requestAddressPriority = null
            , int responseAddressPriority = 1
            , int? maxUdpMessagePayloadSize = UdpHelper.PacketMaxSize
            ):base(capabilities, serializerId, compressionId, encryptionId)
        {
            mConfig = config ?? throw new ArgumentNullException("config", "Udp configuration cannot be null.");
        }


        protected override void ListenerClientsStart()
        {
            throw new NotImplementedException();
        }

        protected override void ListenerClientsStop()
        {
            throw new NotImplementedException();
        }

        protected override void ListenerClientValidate(ClientHolder client, List<MessageFilterWrapper> newList)
        {
            throw new NotImplementedException();
        }

    }
}
