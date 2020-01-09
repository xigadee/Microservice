using System;

namespace Xigadee
{
    /// <summary>
    /// This attribute is used to calculate an entity signature.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class EntitySignatureHintAttribute:Attribute
    {
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="signatureClass">The class used to calculate the signature.</param>
        public EntitySignatureHintAttribute(Type signatureClass)
        {
            SignatureClass = signatureClass ?? throw new ArgumentNullException("signatureClass");
            VerificationPassedWithoutSignature = null;
        }

        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="signatureClass">The class used to calculate the signature.</param>
        /// <param name="verificationPassedWithoutSignature">
        /// Specifies that the verification will be passed if there is not a signature defined. 
        /// This is useful when introducing a new signature policy, but should only be supported until old entities have been updated.
        /// </param>
        public EntitySignatureHintAttribute(Type signatureClass, bool verificationPassedWithoutSignature)
        {
            SignatureClass = signatureClass ?? throw new ArgumentNullException("signatureClass");
            VerificationPassedWithoutSignature = verificationPassedWithoutSignature;
        }

        /// <summary>
        /// This is the class used to calculate the signature.
        /// </summary>
        public Type SignatureClass { get; }

        /// <summary>
        /// Specifies that the verification will be passed if there is not an existing signature defined. 
        /// </summary>
        public bool? VerificationPassedWithoutSignature { get; }
    }
}
