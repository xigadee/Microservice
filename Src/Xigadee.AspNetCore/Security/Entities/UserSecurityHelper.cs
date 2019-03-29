using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;

namespace Xigadee
{
    /// <summary>
    /// This class provides extension methods to the UserSecurity entity.
    /// </summary>
    public static class UserSecurityHelper
    {
        /// <summary>
        /// Determines whether the user has any Client SSL certificate restrictions set in their account.
        /// </summary>
        /// <param name="uSec">The user security object.</param>
        /// <returns> Returns <c>true</c> if restrictions have been set otherwise, <c>false</c>.</returns>
        public static bool HasClientCertificateRestrictions(this UserSecurity uSec)
        {
            return uSec.Certificates.Any();
        }
        /// <summary>
        /// Validates the certificate restriction for the user.
        /// </summary>
        /// <param name="uSec">The user security object.</param>
        /// <param name="hash">The certificate hash.</param>
        /// <returns>Returns true if the certificate is successfully validated.</returns>
        public static bool ValidateCertificateRestriction(this UserSecurity uSec, string hash)
        {
            return uSec.Certificates.Values.Contains(hash);
        }

        /// <summary>
        /// Determines whether the user has any IP address restrictions set in their account.
        /// </summary>
        /// <param name="uSec">The user security object.</param>
        /// <returns> Returns <c>true</c> if restrictions have been set otherwise, <c>false</c>.</returns>
        public static bool HasIpRestrictions(this UserSecurity uSec)
        {
            return uSec.IPAddresses.Any();
        }
        /// <summary>
        /// Validates the IP address restriction.
        /// </summary>
        /// <param name="uSec">The user security object.</param>
        /// <param name="address">The IP address.</param>
        /// <returns>Returns true if the address is successfully validated.</returns>
        public static bool ValidateIpRestriction(this UserSecurity uSec, IPAddress address)
        {
            return uSec.IPAddresses.Select((s) => IPAddress.Parse(s.Value)).Contains(address);
        }
    }
}
