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
    public class Sha256SignatureHelper
    {
        private readonly string mSalt;

        public Sha256SignatureHelper(string salt)
        {
            mSalt = salt??string.Empty;
        }

        public string Sign(params string[] signatureValues)
        {
            return CreateHash(signatureValues);
        }

        public bool VerifySignature(string signature, params string[] signatureValues)
        {
            if (string.IsNullOrEmpty(signature))
                throw new ArgumentOutOfRangeException("signature", "signature cannot be null or empty");

            if (signatureValues == null)
                return false;

            //Case sensitive match
            return signature.Equals(CreateHash(signatureValues), StringComparison.Ordinal);
        }

        private string CreateHash(IEnumerable<string> signatureValues)
        {
            string data = string.Format("{0}:{1}", mSalt, string.Join(":", signatureValues.Select(s => String.IsNullOrEmpty(s) ? string.Empty : s)));

            using (var sha256Provider = new SHA256CryptoServiceProvider())
            {
                byte[] binData = Encoding.UTF8.GetBytes(data);

                byte[] hash = sha256Provider.ComputeHash(binData, 0, binData.Length);

                return Convert.ToBase64String(hash);
            }
        }
    }
}
