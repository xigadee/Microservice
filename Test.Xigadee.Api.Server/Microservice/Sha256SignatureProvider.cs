using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Test.Xigadee.Api.Server
{
    /// <summary>
    /// This class verifies the signature.
    /// </summary>
    public class Sha256SignatureProvider
    {
        private readonly string mSalt;

        public Sha256SignatureProvider(string salt)
        {
            mSalt = salt;
        }

        public string Sign(IList<string> signatureValues)
        {
            return CreateHash(signatureValues);
        }

        public bool VerifySignature(string signature, IList<string> signatureValues)
        {
            //// TODO : Until fully implemented do not verify.
            //return true;

            if (string.IsNullOrEmpty(signature) || signatureValues == null)
                return false;

            return signature.Equals(CreateHash(signatureValues));
        }

        private string CreateHash(IEnumerable<string> signatureValues)
        {
            string data = string.Format("{0}:{1}", mSalt,
                                        string.Join(":", signatureValues.Select(
                                        s => String.IsNullOrEmpty(s) ? string.Empty : s)));

            using (var sha256Provider = new SHA256CryptoServiceProvider())
            {
                byte[] binData = Encoding.UTF8.GetBytes(data.ToUpperInvariant());
                return Convert.ToBase64String(sha256Provider.ComputeHash(binData, 0, binData.Length));
            }
        }
    }
}
