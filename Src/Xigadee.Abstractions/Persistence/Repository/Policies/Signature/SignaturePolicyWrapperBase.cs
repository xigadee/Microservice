using System;
using System.Security.Cryptography;
using System.Text;

namespace Xigadee
{
    /// <summary>
    /// This is the abstract base class that supports signature calculation.
    /// </summary>
    public abstract class SignaturePolicyWrapperBase : SignaturePolicyBase
    {
        /// <summary>
        /// This is the child policy.
        /// </summary>
        protected ISignaturePolicy _childPolicy = null;

        /// <summary>
        /// Specifies whether the policy is active.
        /// </summary>
        protected override ISignaturePolicy Validate() => _childPolicy ?? throw new ArgumentNullException("_childPolicy", "Child Policy has not been set.");

        /// <summary>
        /// This method registers a child signature policy.
        /// </summary>
        /// <param name="childPolicy">The child policy to register.</param>
        public override void RegisterChildPolicy(ISignaturePolicy childPolicy) => _childPolicy = childPolicy;

        /// <summary>
        /// This method returns true if the entity type is supported.
        /// </summary>
        /// <param name="entityType">The entity type to verify.</param>
        /// <returns>Returns true if supported.</returns>
        public override bool Supports(Type entityType) => Validate().Supports(entityType);

    }
}
