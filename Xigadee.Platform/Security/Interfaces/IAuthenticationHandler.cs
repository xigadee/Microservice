using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This interface is used by classes that provide authentication for incoming and outgoing messages.
    /// </summary>
    public interface IAuthenticationHandler: IServiceOriginator
    {
        /// <summary>
        /// This method signs the outgoing payload.
        /// </summary>
        /// <param name="payload">The payload to sign.</param>
        void Sign(TransmissionPayload payload);
        /// <summary>
        /// This method verifies the incoming payload and sets the ClaimsPrincipal on the payload.
        /// </summary>
        /// <param name="payload">The payload to verify.</param>
        void Verify(TransmissionPayload payload);
    }
}
