using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This interface is used to validate incoming messages and to set their SecurityPrincipal value.
    /// </summary>
    public interface ISecurityService: ISecurityCommunication, ISecurityEncryption
    {
    }

    /// <summary>
    /// These method provide validates and decryption/encryption support when supported for incoming and outgoing messages.
    /// </summary>
    public interface ISecurityCommunication
    {
        /// <summary>
        /// This method verifies the security for the incoming message based on the channel it arrived on.
        /// </summary>
        /// <param name="channel">The channel the payload originated on.</param>
        /// <param name="payloadIn">The payload in.</param>
        void Verify(Channel channel, TransmissionPayload payloadIn);
        /// <summary>
        /// This method secures an outgoing message using the security defined for the channel.
        /// </summary>
        /// <param name="channel">The outgoing channel.</param>
        /// <param name="payloadOut">The outgoing payload.</param>
        void Secure(Channel channel, TransmissionPayload payloadOut);
    }

    public interface ISecurityEncryption
    {
        byte[] Encrypt(EncryptionHandlerId id, byte[] input);

        byte[] Decrypt(EncryptionHandlerId id, byte[] input);

        bool EncryptionValidate(EncryptionHandlerId id, bool throwErrors = true);
    }

    /// <summary>
    /// This interface is implemented by components that require access to the security service.
    /// </summary>
    public interface IRequireSecurityService
    {
        /// <summary>
        /// This method provides a method to the security service.
        /// </summary>
        ISecurityService Security { get; set; }
    }
}
