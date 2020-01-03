using System;

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
        /// This method registers a child signature policy.
        /// </summary>
        /// <param name="childPolicy">The child policy to register.</param>
        public void RegisterChildPolicy(ISignaturePolicy childPolicy) => _childPolicy = childPolicy;

        /// <summary>
        /// This method calculates and returns the signature.
        /// </summary>
        /// <param name="entity">The entity to calculate.</param>
        /// <returns>The signature as a string.</returns>
        public abstract string Calculate(object entity);

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
        /// <returns>Returns true if verified.</returns>
        public abstract bool Verify(object entity, string signature);

    }
}
