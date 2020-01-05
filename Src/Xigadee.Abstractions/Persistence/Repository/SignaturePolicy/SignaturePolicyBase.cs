using System;
using System.Security.Cryptography;
using System.Text;

namespace Xigadee
{
    /// <summary>
    /// This is the abstract base class that supports signature calculation.
    /// </summary>
    public abstract class SignaturePolicyBase : ISignaturePolicy
    {
        /// <summary>
        /// This is the child policy.
        /// </summary>
        protected ISignaturePolicy _childPolicy = null;

        /// <summary>
        /// This is the current version identifier that will be appended before the hash. The default value is v1.
        /// </summary>
        public virtual int? SignatureVersion => 1;

        /// <summary>
        /// Specifies whether the policy is active.
        /// </summary>
        protected virtual ISignaturePolicy Validate() => _childPolicy ?? throw new ArgumentNullException("_childPolicy", "Child Policy has not been set.");


        /// <summary>
        /// This method registers a child signature policy.
        /// </summary>
        /// <param name="childPolicy">The child policy to register.</param>
        public void RegisterChildPolicy(ISignaturePolicy childPolicy) => _childPolicy = childPolicy;

        /// <summary>
        /// This method calculates and returns the signature.
        /// </summary>
        /// <param name="entity">The entity to calculate.</param>
        /// <param name="versionId">The version of the hash to calculate. For normal operation this should be left null, but in
        /// cases when moving from say a v1 to a v2 hash, especially when adding new properties to the hash, you may need to upgrade the 
        /// hash when updating an entity.</param>
        /// <returns>The signature as a string.</returns>
        public virtual string Calculate(object entity, int? versionId = null)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            versionId = versionId ?? SignatureVersion;


            if (!Supports(entity.GetType()))
                throw new NotSupportedException();

            if (versionId.HasValue)
                return $"v{versionId.Value}.{CalculateInternal(entity, versionId)}";
            else
                return CalculateInternal(entity, versionId);
        }

        /// <summary>
        /// This method calculates and returns the signature without any additional checks.
        /// </summary>
        /// <param name="entity">The entity to calculate.</param>
        /// <returns>The signature as a string.</returns>
        protected abstract string CalculateInternal(object entity, int? versionId = null);

        /// <summary>
        /// This method returns true if the entity type is supported.
        /// </summary>
        /// <param name="entityType">The entity type to verify.</param>
        /// <returns>Returns true if supported.</returns>
        public virtual bool Supports(Type entityType) => Validate().Supports(entityType);

        /// <summary>
        /// This method verifies the signature passed with the entity.
        /// </summary>
        /// <param name="entity">The entity to check.</param>
        /// <param name="signature">The verification signature.</param>
        /// <returns>Returns true if verified.</returns>
        public virtual bool Verify(object entity, string signature) => Calculate(entity) == signature;

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
