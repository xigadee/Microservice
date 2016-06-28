using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This class verifies the signature.
    /// </summary>
    public class Sha512SignatureHelper
    {
        #region Declarations
        private readonly string mSalt;
        #endregion
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="salt">The case sensitive string that serves as the salt value for the signature.</param>
        public Sha512SignatureHelper(string salt)
        {
            mSalt = salt ?? string.Empty;
        } 
        #endregion

        /// <summary>
        /// This method creates a SHA256
        /// </summary>
        /// <param name="signatureValues"></param>
        /// <returns></returns>
        public string Sign(params string[] signatureValues)
        {
            return CreateHash(signatureValues);
        }

        public bool VerifySignature(string base64signature, params string[] signatureValues)
        {
            if (string.IsNullOrEmpty(base64signature))
                throw new ArgumentOutOfRangeException("signature", "signature cannot be null or empty");

            //Case sensitive match
            return base64signature.Equals(CreateHash(signatureValues), StringComparison.Ordinal);
        }

        /// <summary>
        /// This private method creates a base64 hash from the salt and the parameters passed.
        /// </summary>
        /// <param name="signatureValues">The signature values to hash.</param>
        /// <returns>Returns a base 64 encoded string.</returns>
        private string CreateHash(IEnumerable<string> signatureValues)
        {
            string data = string.Format("{0}:{1}", mSalt, string.Join(":", signatureValues.Select(s => String.IsNullOrEmpty(s) ? string.Empty : s)));

            using (var provider = new SHA512CryptoServiceProvider())
            {
                byte[] binData = Encoding.UTF8.GetBytes(data);

                byte[] hash = provider.ComputeHash(binData, 0, binData.Length);

                return Convert.ToBase64String(hash);
            }
        }
    }
}
