using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

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
        /// <summary>
        /// Sets the authentication.
        /// </summary>
        /// <param name="uSec">The user security object.</param>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        public static void AuthenticationSet(this UserSecurity uSec, string username, string password)
        {
            var result = uSec.CalculateSHA256Hash(username, password);
            uSec.Authentication[result.algo] = result.b64;
        }

        /// <summary>
        /// Calculates the SHA256 hash.
        /// </summary>
        /// <param name="uSec">The user security object.</param>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public static (string algo, string b64) CalculateSHA256Hash(this UserSecurity uSec, string username, string password)
        {
            username = (username ?? "").Trim().ToLowerInvariant();
            var combined = $"{uSec.Id.ToString("N").ToLowerInvariant()}:{username}:{password}";
            byte[] bytes = Encoding.UTF8.GetBytes(combined);

            using (SHA256Managed algo = new SHA256Managed())
            {
                byte[] hash = algo.ComputeHash(bytes);
                return ("SHA256", Convert.ToBase64String(hash));
            }
        }


        /// <summary>
        /// Verifies the incoming credential information.
        /// </summary>
        /// <param name="uSec">The user security object.</param>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns>Returns true if the credentials match the stored hash.</returns>
        public static bool AuthenticationVerify(this UserSecurity uSec, string username, string password)
        {
            var result = uSec.CalculateSHA256Hash(username, password);

            if (uSec.Authentication.TryGetValue(result.algo, out string hash))
                return hash == result.b64;

            return false;
        }

    }
}
