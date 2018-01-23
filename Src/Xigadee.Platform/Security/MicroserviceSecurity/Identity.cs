using System.Linq;
using System.Security.Claims;

namespace Xigadee
{
    /// <summary>
    /// This class holds the identity of the party specified in the token.
    /// </summary>
    /// <seealso cref="System.Security.Claims.ClaimsIdentity" />
    public class MicroserviceSecurityIdentity: ClaimsIdentity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MicroserviceSecurityIdentity"/> class.
        /// </summary>
        /// <param name="incoming">The incoming token that holds the claims.</param>
        public MicroserviceSecurityIdentity(JwtToken incoming)
        {
            incoming.Claims
                .Where((c) => c.Value is string)
                .ForEach((c) => AddClaim(new Claim(c.Key, c.Value as string)));
        }
    }
}
