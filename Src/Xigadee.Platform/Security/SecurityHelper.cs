using System;
using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Xigadee
{
    /// <summary>
    /// This static class contains a useful set of security functions.
    /// </summary>
    public static class SecurityHelper
    {
        /// <summary>
        /// Calculates the authentication parameter for DocumentDb requests..
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="verb">The verb.</param>
        /// <param name="resourceType">Type of the resource.</param>
        /// <param name="resourceId">The resource identifier.</param>
        /// <param name="xmsdate">The xmsdate.</param>
        /// <param name="date">The date.</param>
        /// <param name="idBased">if set to <c>true</c> [identifier based].</param>
        /// <returns>The authentication as a URL encoded string.</returns>
        public static string CalculateAuth(string key, string verb, string resourceType, string resourceId, string xmsdate, string date = "", bool idBased = false)
        {
            string data = string.Format(CultureInfo.InvariantCulture,
                "{0}\n{1}\n{2}\n{3}\n{4}\n"
                , verb.ToLowerInvariant()
                , resourceType.ToLowerInvariant()
                , idBased ? resourceId : resourceId.ToLowerInvariant()
                , xmsdate.ToLowerInvariant()
                , date.ToLowerInvariant());

            string signature = HMACSignature(key, data);

            string auth = string.Format("type=master&ver=1.0&sig={0}", signature);

            return WebUtility.UrlEncode(auth);
        }

        #region HMACSignature ...
        /// <summary>
        /// Returns a HMACSHA256 of the data using the key provided.
        /// </summary>
        /// <param name="key">The key as a string.</param>
        /// <param name="data">The data to hash.</param>
        /// <returns>Returns a Base64 encoded HMAC SHA256 hash.</returns>
        public static string HMACSignature(string key, string data)
        {
            return HMACSignature(Encoding.UTF8.GetBytes(key), data);
        }

        /// <summary>
        /// Returns a HMACSHA256 of the data using the key provided.
        /// </summary>
        /// <param name="key">The key as a byte array.</param>
        /// <param name="data">The data to hash.</param>
        /// <returns>Returns a Base64 encoded HMAC SHA256 hash.</returns>
        public static string HMACSignature(byte[] key, string data)
        {
            string result;
            using (var hmac = new HMACSHA256(key))
            {
                byte[] hash = Encoding.UTF8.GetBytes(data);
                byte[] sha256Hash = hmac.ComputeHash(hash);

                result = Convert.ToBase64String(sha256Hash);
            }

            return result;
        }
        #endregion

        #region CalculateSasToken
        static readonly DateTime sSASOrigin = new DateTime(1970, 1, 1, 0, 0, 0, 0);

        /// <summary>
        /// Calculates the SAS token for the parameters passed.
        /// </summary>
        /// <param name="uri">The URI of the resource to secure.</param>
        /// <param name="keyName">Name of the key.</param>
        /// <param name="key">The key.</param>
        /// <param name="lifetimeInS">The lifetime in seconds for the token. The default is 20 minutes.</param>
        /// <returns>Returns the token as a string.</returns>
        /// <remarks>See http://msdn.microsoft.com/en-us/library/windowsazure/dn170477.aspx for more details.</remarks>
        public static string CalculateSasToken(string uri, string keyName, string key, int lifetimeInS = 1200)
        {
            TimeSpan diff = DateTime.UtcNow - sSASOrigin;
            uint tokenExpiry = Convert.ToUInt32(diff.TotalSeconds) + Convert.ToUInt32(lifetimeInS);

            string sig = WebUtility.UrlEncode(uri) + "\n" + tokenExpiry;

            string signature = HMACSignature(key, sig);

            string token = String.Format(CultureInfo.InvariantCulture
                , "SharedAccessSignature sr={0}&sig={1}&se={2}&skn={3}"
                , WebUtility.UrlEncode(uri)
                , WebUtility.UrlEncode(signature)
                , tokenExpiry
                , keyName);

            return token;
        } 
        #endregion
    }
}
