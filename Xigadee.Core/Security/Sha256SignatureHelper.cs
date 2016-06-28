using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public abstract class SignatureHelper<H>
        where H: HashAlgorithm
    {
        #region Declarations
        protected readonly string mSalt;
        #endregion
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="salt">The case sensitive string that serves as the salt value for the signature.</param>
        public SignatureHelper(string salt)
        {
            mSalt = salt ?? string.Empty;
        }
        #endregion

        /// <summary>
        /// This method creates a hash from the incoming values against the salt.
        /// </summary>
        /// <param name="signatureValues">The set of string values to hash with the salt.</param>
        /// <returns>Returns the base64 encoded hash.</returns>
        public string Sign(params string[] signatureValues)
        {
            byte[] hash = CreateHash(signatureValues);
            return Convert.ToBase64String(hash);
        }

        /// <summary>
        /// This method verifies an existing signature against the set of string parameters passed.
        /// </summary>
        /// <param name="base64signature">The existing base64 encoded signature.</param>
        /// <param name="signatureValues">The parameter collection.</param>
        /// <returns>Returns true if the computed hash matches the one passed.</returns>
        public bool VerifySignature(string base64signature, params string[] signatureValues)
        {
            if (string.IsNullOrEmpty(base64signature))
                throw new ArgumentOutOfRangeException("signature", "signature cannot be null or empty");

            byte[] comparand = Convert.FromBase64String(base64signature);
            byte[] hash = CreateHash(signatureValues);

            return StructuralComparisons.StructuralEqualityComparer.Equals(comparand, hash);
        }

        /// <summary>
        /// This private method creates a base64 hash from the salt and the parameters passed.
        /// </summary>
        /// <param name="signatureValues">The signature values to hash.</param>
        /// <returns>Returns a byte array.</returns>
        private byte[] CreateHash(IEnumerable<string> signatureValues)
        {
            string data = string.Format("{0}:{1}", mSalt, string.Join(":", signatureValues.Select(s => String.IsNullOrEmpty(s) ? string.Empty : s)));

            using (H provider = CreateProvider())
            {
                byte[] binData = Encoding.UTF8.GetBytes(data);

                byte[] hash = provider.ComputeHash(binData, 0, binData.Length);

                return hash;
            }
        }

        /// <summary>
        /// This override is used to create the specific provider.
        /// </summary>
        /// <returns></returns>
        protected abstract H CreateProvider();
    }

    /// <summary>
    /// This class verifies the signature.
    /// </summary>
    public class Sha512SignatureHelper: SignatureHelper<SHA512CryptoServiceProvider>
    {
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="salt">The case sensitive string that serves as the salt value for the signature.</param>
        public Sha512SignatureHelper(string salt) : base(salt)
        {
        }
        #endregion

        protected override SHA512CryptoServiceProvider CreateProvider()
        {
            return new SHA512CryptoServiceProvider();
        }
    }

    /// <summary>
    /// This class verifies the signature.
    /// </summary>
    public class Sha256SignatureHelper: SignatureHelper<SHA256CryptoServiceProvider>
    {
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="salt">The case sensitive string that serves as the salt value for the signature.</param>
        public Sha256SignatureHelper(string salt) : base(salt)
        {
        }
        #endregion

        protected override SHA256CryptoServiceProvider CreateProvider()
        {
            return new SHA256CryptoServiceProvider();
        }
    }
}
