using System;

namespace Xigadee
{
    /// <summary>
    /// This is the base signature policy class for an entity.
    /// </summary>
    public class SignaturePolicyNull : ISignaturePolicy
    {
        /// <summary>
        /// No version.
        /// </summary>
        public int? SignatureVersion => null;

        /// <summary>
        /// Always return true.
        /// </summary>
        public bool VerificationPassedWithoutSignature { get; } = true;

        /// <summary>
        /// No calculation is performed.
        /// </summary>
        public string Calculate(object entity, int? versionid = null) => null;

        /// <summary>
        /// This policy does not support signature.
        /// </summary>
        /// <param name="entityType"></param>
        /// <returns></returns>
        public bool Supports(Type entityType) => false;

        /// <summary>
        /// No verification will be done.
        /// </summary>
        public bool Verify(object entity, string signature) => true;
    }
}
