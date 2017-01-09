using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xigadee;

namespace Xigadee
{
    /// <summary>
    /// This class contains and validates the Jwt Token.
    /// This class currently only supports simple HMAC-based verification.
    /// Thanks to http://kjur.github.io/jsjws/tool_jwt.html for verification.
    /// </summary>
    public class JwtToken
    {
        #region Declarations
        /// <summary>
        /// The underlying 
        /// </summary>
        protected JwtRoot mIncoming = null;
        #endregion
        #region Constructor
        /// <summary>
        /// This is the default constructor for creating a new token.
        /// </summary>
        public JwtToken(JwtHashAlgorithm? algo = null)
        {
            Header = new JOSEHeader();
            //Set the default settings.
            Header.SupportedAlgorithm = algo ?? JwtHashAlgorithm.HS256;
            Header.Type = "JWT";

            //Set the empty claims.
            Claims = new JwtClaims();
        }
        /// <summary>
        /// This constructor should be used to load and incoming token.
        /// </summary>
        /// <param name="token">The compact serialized token.</param>
        /// <param name="secret">The secret to validate the token.</param>
        /// <param name="validateSignature">A boolean value that indicates when token should be validated with the secret.</param>
        /// <param name="supportNoneAlgo">This property specifies whether the none algorithm should be supported. As this is a security risk, it has to be explicitly set to accept.</param>
        public JwtToken(string token, byte[] secret, bool validateSignature = true, bool supportNoneAlgo = false)
        {
            mIncoming = new JwtRoot(token);

            Header = new JOSEHeader(mIncoming.JoseHeader);
            if (!string.Equals(Header.Type,"JWT", StringComparison.InvariantCultureIgnoreCase))
                throw new JwtTokenStructureInvalidException("The JWT declaration is not in the JOSE Header");

            //Check that the algorithm is supported.
            var algo = Header.SupportedAlgorithm;
            if (algo == JwtHashAlgorithm.None && !supportNoneAlgo)
                throw new JwtAlgorithmNoneNotAllowedException();

            Claims = new JwtClaims(JwtRoot.UTF8ToJSONConvert(mIncoming.Raw[1]));

            if (validateSignature && !JwtValidateIncoming(mIncoming, algo, secret))
                throw new JwtSignatureInvalidException();
        }
        #endregion

        //Be aware: https://auth0.com/blog/critical-vulnerabilities-in-json-web-token-libraries/

        /// <summary>
        /// This is the JOSE Header collection.
        /// </summary>
        public JOSEHeader Header { get; }
        /// <summary>
        /// This is the Jwt Cliams set.
        /// </summary>
        public JwtClaims Claims { get; }

        /// <summary>
        /// This method validates teh 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ValidateIncoming(byte[] key, JwtHashAlgorithm? algo = null)
        {
            return JwtValidateIncoming(mIncoming, algo ?? Header.SupportedAlgorithm, key);
        }

        private bool JwtValidateIncoming(JwtRoot incoming, JwtHashAlgorithm algo, byte[] key)
        {
            if (incoming == null)
                throw new ArgumentOutOfRangeException("Incoming token not set.");

            if (key == null)
                throw new ArgumentNullException("key", $"{nameof(JwtValidateIncoming)} - key cannot be null");

            string b64joseHeader = JwtHelper.SafeBase64UrlEncode(incoming.Raw[0]);
            string b64jwtClaimsSet = JwtHelper.SafeBase64UrlEncode(incoming.Raw[1]);

            var signed = JwtRoot.CalculateAuthSignature(algo, key, b64joseHeader, b64jwtClaimsSet);
            var original = JwtHelper.SafeBase64UrlEncode(incoming.Raw[2]);
            return original == signed;
        }

        /// <summary>
        /// This method signs the token and returns it using JWS Compact Serialization notation.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Returns a compact serialization key.</returns>
        public string ToString(byte[] key)
        {
            JwtHashAlgorithm algo = Header.SupportedAlgorithm;

            string b64joseHeader = JwtHelper.SafeBase64UrlEncode(Encoding.UTF8.GetBytes(Header.ToString()));
            string b64jwtClaimsSet = JwtHelper.SafeBase64UrlEncode(Encoding.UTF8.GetBytes(Claims.ToString()));

            string signature = JwtRoot.CalculateAuthSignature(algo, key, b64joseHeader, b64jwtClaimsSet);

            return $"{b64joseHeader}.{b64jwtClaimsSet}.{signature}";
        }
    }
}
