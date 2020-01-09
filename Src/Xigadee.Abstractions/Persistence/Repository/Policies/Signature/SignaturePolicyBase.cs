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
        /// This is the current version identifier that will be appended before the hash. The default value is v1.
        /// </summary>
        public virtual int? SignatureVersion => 1;

        /// <summary>
        /// This property can be overriden and allows an entity to be read without a signature.
        /// This is useful when transitioning signatures in to an existing entity store. 
        /// The default value is false and should be switched off when the migration is completed.
        /// </summary>
        public virtual bool VerificationPassedWithoutSignature { get; set; } = false;
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
                throw new NotSupportedException($"{entity.GetType().Name} is not supported for signature generation.");

            return SignatureFormat(versionId, CalculateInternal(entity, versionId));
        }

        /// <summary>
        /// This method calculates and returns the signature without any additional checks.
        /// </summary>
        /// <param name="entity">The entity to calculate.</param>
        /// <param name="versionId">The version of the hash to calculate.</param>
        /// <returns>The signature as a string.</returns>
        protected abstract string CalculateInternal(object entity, int? versionId = null);

        /// <summary>
        /// This method returns true if the entity type is supported.
        /// </summary>
        /// <param name="entityType">The entity type to verify.</param>
        /// <returns>Returns true if supported.</returns>
        public abstract bool Supports(Type entityType);

        /// <summary>
        /// This method verifies the signature passed with the entity.
        /// </summary>
        /// <param name="entity">The entity to check.</param>
        /// <param name="signature">The verification signature.</param>
        /// <returns>Returns true if verified. If the signature is blank, it will return the VerificationPassedWithoutSignature value.</returns>
        public virtual bool Verify(object entity, string signature)
        {
            //This check is used during transition when an entity begins signing but not all entities have been signed.
            //The default value is false. 
            if (string.IsNullOrWhiteSpace(signature))
                return VerificationPassedWithoutSignature;

            if (!SignatureParse(signature, out var versionId, out var hashPart))
                throw new ArgumentOutOfRangeException("signature", "signature version cannot be parsed.");

            var hash = Calculate(entity, versionId);

            return hash == signature;
        }

        /// <summary>
        /// This method formats the output in to a signature format.
        /// </summary>
        /// <param name="versionId">The supported version.</param>
        /// <param name="hash">The hash part.</param>
        /// <returns>Returns the combined signature.</returns>
        protected virtual string SignatureFormat(int? versionId, string hash)
        {
            if (versionId.HasValue)
                return $"#v{versionId.Value}.{hash}";
            else
                return hash;
        }
        /// <summary>
        /// This method parses the signatue in to the version and hash.
        /// </summary>
        /// <param name="signature">The combined signature.</param>
        /// <param name="versionId">The output version.</param>
        /// <param name="hash">The output hash part.</param>
        /// <returns>Returns true if the signature was parsed and returned correctly.</returns>
        protected virtual bool SignatureParse(string signature, out int? versionId, out string hash)
        {
            versionId = null;
            hash = null;

            if (string.IsNullOrWhiteSpace(signature))
                return false;

            if (signature.StartsWith("#v"))
            {
                int pos = signature.IndexOf('.');
                var ver = signature.Substring(2, pos - 2);

                if (!int.TryParse(ver, out var value))
                    return false;

                versionId = value;
                hash = signature.Substring(pos + 1);
            }
            else
                hash = signature;

            return true;
        }
    }
}
