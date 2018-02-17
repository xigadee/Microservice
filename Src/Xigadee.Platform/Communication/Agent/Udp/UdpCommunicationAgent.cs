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
        /// <param name="config">The configuration.</param>
        /// <param name="capabilities">The capabilities.</param>
        public UdpCommunicationAgent(UdpConfig config
            , SerializationHandlerId serializerId = null
            , CompressionHandlerId compressionId = null
            , EncryptionHandlerId encryptionId = null
            , ServiceMessageHeaderFragment requestAddress = null
            , ServiceMessageHeader responseAddress = null
            , int? requestAddressPriority = null
            , int responseAddressPriority = 1
            , CommunicationAgentCapabilities capabilities = CommunicationAgentCapabilities.Bidirectional
            , int? maxUdpMessagePayloadSize = UdpHelper.PacketMaxSize
            )
        {
            mConfig = config;
            Capabilities = capabilities;
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

        public override Task SenderTransmit(TransmissionPayload message)
        {
            throw new NotImplementedException();
        }

        protected override ClientHolder SenderClientResolve(int priority)
        {
            throw new NotImplementedException();
        }
    }
}
