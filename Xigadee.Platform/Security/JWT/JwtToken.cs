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
    /// </summary>
    public class JwtToken
    {
        #region Registered claims
        public const string HeaderIssuer = "iss";
        public const string HeaderSubject = "sub";
        public const string HeaderAudience = "aud";
        public const string HeaderExpirationTime = "exp";
        public const string HeaderNotBefore = "nbf";
        public const string HeaderIssuedAt = "iat";
        public const string HeaderJWTID = "jti"; 
        #endregion

        #region Declarations
        /// <summary>
        /// The underlying 
        /// </summary>
        protected JwtRoot mRoot = null;
        #endregion
        #region Constructor
        /// <summary>
        /// This is the default constructor for creating a new token.
        /// </summary>
        public JwtToken()
        {
            Header = new JOSEHeader();
            //Set the default settings.
            Header.Algorithm = JWTHashAlgorithm.HS256;
            Header.Type = "JWT";

            //Set the empty claims.
            Claims = new ClaimsSet();
        }
        /// <summary>
        /// This constructor should be used to load and incoming token.
        /// </summary>
        /// <param name="token">The compact serialized token.</param>
        /// <param name="secret">The secret to validate the token.</param>
        /// <param name="validateToken">A boolean value that indicates when token should be validated with the secret.</param>
        public JwtToken(string token, byte[] secret, bool validateToken = true)
        {
            mRoot = new JwtRoot(token);

            Header = new JOSEHeader(mRoot.JoseHeader);
            if (!string.Equals(Header.Type,"JWT", StringComparison.InvariantCultureIgnoreCase))
                throw new InvalidJwtTokenStructureException("The JOSE Header type is not JWT");

            //Check that the algorithm is supported.
            var algo = Header.Algorithm;

            Claims = new ClaimsSet(JwtRoot.UTF8ToJSONConvert(mRoot.Raw[1]));

            if (validateToken && !JwtValidateIncoming(mRoot, algo, secret))
                throw new InvalidJwtSignatureException();
        } 
        #endregion

        /// <summary>
        /// This is the JOSE Header collection.
        /// </summary>
        public JOSEHeader Header { get; set; }
        /// <summary>
        /// This is the Jwt Cliams set.
        /// </summary>
        public ClaimsSet Claims { get; set; }

        /// <summary>
        /// This method validates teh 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ValidateIncoming(byte[] key, JWTHashAlgorithm? algo = null)
        {
            return JwtValidateIncoming(mRoot, algo ?? Header.Algorithm, key);
        }

        private bool JwtValidateIncoming(JwtRoot root, JWTHashAlgorithm algo, byte[] key)
        {
            if (root == null)
                throw new ArgumentOutOfRangeException("Incoming token not set.");

            if (key == null)
                throw new ArgumentNullException("key", $"{nameof(JwtValidateIncoming)} - key cannot be null");

            string b64joseHeader = JwtRoot.SafeBase64UrlEncode(root.Raw[0]);
            string b64jwtClaimsSet = JwtRoot.SafeBase64UrlEncode(root.Raw[1]);

            var signed = JwtRoot.CalculateAuthSignature(algo, key, b64joseHeader, b64jwtClaimsSet);
            var original = JwtRoot.SafeBase64UrlEncode(root.Raw[2]);
            return original == signed;
        }

        /// <summary>
        /// This method signs the token and returns it using JWS Compact Serialization notation.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Returns a compact serialization key.</returns>
        public string ToString(byte[] key)
        {
            JWTHashAlgorithm algo = Header.Algorithm;

            string b64joseHeader = JwtRoot.SafeBase64UrlEncode(Encoding.UTF8.GetBytes(Header.ToString()));
            string b64jwtClaimsSet = JwtRoot.SafeBase64UrlEncode(Encoding.UTF8.GetBytes(Claims.ToString()));

            string signature = JwtRoot.CalculateAuthSignature(algo, key, b64joseHeader, b64jwtClaimsSet);

            return $"{b64joseHeader}.{b64jwtClaimsSet}.{signature}";
        }
    }
}
