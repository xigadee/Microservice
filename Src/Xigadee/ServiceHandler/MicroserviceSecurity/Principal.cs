using System.Linq;
using System.Security.Claims;

namespace Xigadee
{
    /// <summary>
    /// This method holds the security information for the current request with the Microservice.
    /// These claims can be set from the remote party.
    /// </summary>
    public class MicroserviceSecurityPrincipal:ClaimsPrincipal
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MicroserviceSecurityPrincipal"/> class.
        /// </summary>
        public MicroserviceSecurityPrincipal()
        {

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="MicroserviceSecurityPrincipal"/> class.
        /// </summary>
        /// <param name="incoming">The incoming token.</param>
        public MicroserviceSecurityPrincipal(JwtToken incoming):base(new MicroserviceSecurityIdentity(incoming))
        {

        }
    }

}
