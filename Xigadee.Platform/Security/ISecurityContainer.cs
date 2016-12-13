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
    public interface ISecurityService: ISecurityCommunication
    {
    }

    /// <summary>
    /// These method provide validates and decryption/encryption support when supported for incoming and outgoing messages.
    /// </summary>
    public interface ISecurityCommunication
    {
        void Verify(TransmissionPayload payloadIn);

        void Secure(TransmissionPayload payloadOut);
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
