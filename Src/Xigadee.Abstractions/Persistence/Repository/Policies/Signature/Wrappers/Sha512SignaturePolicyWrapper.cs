using System;
using System.Security.Cryptography;
using System.Text;

namespace Xigadee
{
    /// <summary>
    /// This method hashes the root signature with an SHA512 hash.
    /// </summary>
    public class Sha512SignaturePolicyWrapper : SignaturePolicyWrapperBase
    {
        /// <summary>
        /// This method creates the internal hash.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="versionId">This is the required version of the hash.</param>
        /// <returns>Returns the Base 64 encoded encrypted hash.</returns>
        protected override string CalculateInternal(object entity, int? versionId = null)
        {
            //Calculate the child signature
            var signature = CalculateSignature(entity, versionId);

            return CreateSHA512HashAsBase64(signature);
        }

        /// <summary>
        /// This method is used to create the raw string signature. The SHA512 function will be applied to this 
        /// string to output the actual hash, that is of a fixed length string.
        /// </summary>
        /// <param name="entity">The entity the string signature should be create</param>
        /// <param name="versionId">This is the required version of the hash.</param>
        /// <returns>Returns the string signature for the entity.</returns>
        protected virtual string CalculateSignature(object entity, int? versionId = null)
            => _childPolicy.Calculate(entity, versionId);

        /// <summary>
        /// This method creates a hash from the incoming string.
        /// </summary>
        /// <param name="signature">The signature string to hash.</param>
        /// <returns>Returns the byte array.</returns>
        protected virtual byte[] CreateSHA512Hash(string signature)
        {
            var bytes = Encoding.UTF8.GetBytes(signature);

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
        /// This method creates a hash from the incoming string.
        /// </summary>
        /// <param name="signature">The signature string to hash.</param>
        /// <returns>Returns the hash as a base64 encoded string.</returns>
        protected virtual string CreateSHA512HashAsBase64(string signature) => Convert.ToBase64String(CreateSHA512Hash(signature));

    }
}
