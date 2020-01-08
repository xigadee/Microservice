using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Xigadee
{
    /// <summary>
    /// This method hashes the root signature and signs it with a symetric key.
    /// </summary>
    public class AesWrapperSignaturePolicyWrapper : SignaturePolicyWrapperBase
    {
        #region Static helper constructor
        /// <summary>
        /// Creates a test policy with a fresh key and Iv.
        /// </summary>
        /// <returns>Returns the provider.</returns>
        public static AesWrapperSignaturePolicyWrapper CreateTestPolicy() => new AesWrapperSignaturePolicyWrapper();
        #endregion        
        #region Constructors
        /// <summary>
        /// This is the default constructor. It will create a default provider with a new key and new IV.
        /// </summary>
        internal AesWrapperSignaturePolicyWrapper()
        {
            Provider = new AesCryptoServiceProvider();
            Provider.Mode = CipherMode.CBC;
            Provider.KeySize = 128;
            Provider.BlockSize = 128;
            Provider.GenerateIV();
            Provider.GenerateKey();
        }
        /// <summary>
        /// This constructor accepts the crypt service provider used to encrypt and decrypt the signature.
        /// </summary>
        /// <param name="provider">The crypto service provider.</param>
        public AesWrapperSignaturePolicyWrapper(AesCryptoServiceProvider provider)
        {
            Provider = provider;
        }
        /// <summary>
        /// This constructor accepts the paramters directly used to create a crypt service provider used to encrypt and decrypt the signature.
        /// </summary>
        /// <param name="key">The AES encrpytion key.</param>
        /// <param name="iv">The AES initialization vector.</param>
        /// <param name="mode">The optional mode, the default is CBC.</param>
        /// <param name="keySize">The key size, the default is 128.</param>
        /// <param name="blockSize">The block size, the default is 128.</param>
        public AesWrapperSignaturePolicyWrapper(byte[] key, byte[] iv, CipherMode mode = CipherMode.CBC, int keySize = 128, int blockSize = 128)
        {
            Provider = new AesCryptoServiceProvider();
            Provider.Mode = mode;
            Provider.KeySize = keySize;
            Provider.BlockSize = blockSize;
            Provider.Key = key;
            Provider.IV = iv;
        } 
        #endregion

        /// <summary>
        /// This is the crypto provider for the signature.
        /// </summary>
        public AesCryptoServiceProvider Provider { get; }

        /// <summary>
        /// This method creates the internal hash.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="versionId">The signature version.</param>
        /// <returns>Returns the Base 64 encoded encrypted hash.</returns>
        protected override string CalculateInternal(object entity, int? versionId = null)
        {
            //Calculate the child signature
            byte[] hash = CreateHash(entity);

            //Encrypt the root using the symetric key
            var enc = Provider.CreateEncryptor();

            var result = enc.TransformFinalBlock(hash, 0, hash.Length);

            return Convert.ToBase64String(result);
        }

        private byte[] CreateHash(object entity, int? versionId = null)
        {
            //Calculate the child signature
            var child = _childPolicy.Calculate(entity, versionId);

            var bytes = Encoding.UTF8.GetBytes(child);

            //Hash the root
            // computes the hash of the name space ID concatenated with the name (step 4)
            byte[] hash;
            using (var incrementalHash = IncrementalHash.CreateHash(HashAlgorithmName.SHA512))
            {
                incrementalHash.AppendData(bytes);
                hash = incrementalHash.GetHashAndReset();
            }

            return hash;
        }

        /// <summary>
        /// This method verifies the signature passed with the entity.
        /// </summary>
        /// <param name="entity">The entity to check.</param>
        /// <param name="signature">The verification signature.</param>
        /// <returns>Returns true if verified. If the signature is blank, it will return the VerificationPassedWithoutSignature value.</returns>
        public override bool Verify(object entity, string signature)
        {
            //This check is used during transition when an entity begins signing but not all entities have been signed.
            //The default value is false. 
            if (string.IsNullOrWhiteSpace(signature))
                return VerificationPassedWithoutSignature;

            if (!SignatureParse(signature, out var versionId, out var hashPart))
                throw new ArgumentOutOfRangeException("signature", "signature version cannot be parsed.");

            //Calculate the child signature
            byte[] hash = CreateHash(entity, versionId);

            //Encrypt the root using the symetric key
            var dec = Provider.CreateDecryptor();

            var encBody = Convert.FromBase64String(hashPart);

            var result = dec.TransformFinalBlock(encBody, 0, encBody.Length);

            return ByteArrayCompare(hash, result);
        }

    }
}
