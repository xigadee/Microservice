using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Xigadee
{
    /// <summary>
    /// This class wraps both AES encryption and a SHA512 hash around the signature.
    /// </summary>
    public class AesSha512SignaturePolicyWrapper: AesWrapperSignaturePolicyWrapper
    {
        /// <summary>
        /// This is the default constructor. It will create a default provider with a new key and new IV.
        /// </summary>
        internal AesSha512SignaturePolicyWrapper():base()
        {
        }

        public AesSha512SignaturePolicyWrapper(AesCryptoServiceProvider provider):base(provider)
        {
        }

        public AesSha512SignaturePolicyWrapper(byte[] key, byte[] iv, CipherMode mode = CipherMode.CBC, int keySize = 128, int blockSize = 128)
            :base(key, iv, mode, keySize, blockSize)
        {
        }

        /// <summary>
        /// This override inserts the SHA512 wrapper between the child policy.
        /// </summary>
        /// <param name="childPolicy">The child policy.</param>
        public override void RegisterChildPolicy(ISignaturePolicy childPolicy)
        {
            var sha512Policy = new Sha512SignaturePolicyWrapper();

            sha512Policy.RegisterChildPolicy(childPolicy);

            base.RegisterChildPolicy(sha512Policy);
        }
    }
}
