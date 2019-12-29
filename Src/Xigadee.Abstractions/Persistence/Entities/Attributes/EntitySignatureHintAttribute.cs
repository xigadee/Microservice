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
        }

        /// <summary>
        /// This is the class used to calculate the signature.
        /// </summary>
        public Type SignatureClass { get; }

    }
}
