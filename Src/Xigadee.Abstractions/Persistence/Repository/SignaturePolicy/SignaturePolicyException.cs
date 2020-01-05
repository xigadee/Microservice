using System;
using System.Collections.Generic;
using System.Text;

namespace Xigadee
{ 
    /// <summary>
    /// This exception is throw when resolving the Entity signature support class.
    /// </summary>
    public class SignaturePolicyException:Exception
    {
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="entityType">The entity type.</param>
        /// <param name="signatureType">The signature type</param>
        public SignaturePolicyException(Type entityType, Type signatureType)
        {
            EntityType = entityType;
            SignatureClassType = signatureType;
        }
        /// <summary>
        /// This is the entity type that the exception is generated for.
        /// </summary>
        public Type EntityType { get; }
        /// <summary>
        /// This is the signature class type that the exception was generated for.
        /// </summary>
        public Type SignatureClassType { get; }
    }
}
