using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Xigadee
{
    /// <summary>
    /// This method hashes the root signature and signs it with a symetric key.
    /// </summary>
    public class AesWrapperSignaturePolicyWrapper : SignaturePolicyBase
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

        public AesWrapperSignaturePolicyWrapper(AesCryptoServiceProvider provider)
        {
            Provider = provider;
        }

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



        public override bool Verify(object entity, string signature)
        {
            //Calculate the child signature
            byte[] hash = CreateHash(entity);

            //Encrypt the root using the symetric key
            var dec = Provider.CreateDecryptor();

            var encBody = Convert.FromBase64String(signature);

            var result = dec.TransformFinalBlock(encBody, 0, encBody.Length);

            return ByteArrayCompare(hash, result);
        }

        static bool ByteArrayCompare(ReadOnlySpan<byte> a1, ReadOnlySpan<byte> a2)
        {
            return a1.SequenceEqual(a2);
        }
    }
}
