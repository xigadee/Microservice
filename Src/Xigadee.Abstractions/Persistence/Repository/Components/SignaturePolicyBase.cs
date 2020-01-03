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
        /// <returns>The signature as a string.</returns>
        public virtual string Calculate(object entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            if (!Supports(entity.GetType()))
                throw new NotSupportedException();

            return CalculateInternal(entity);
        }

        /// <summary>
        /// This method calculates and returns the signature without any additional checks.
        /// </summary>
        /// <param name="entity">The entity to calculate.</param>
        /// <returns>The signature as a string.</returns>
        protected abstract string CalculateInternal(object entity);


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

    }
}
