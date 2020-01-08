using System;
using System.Collections.Generic;
using System.Text;

namespace Xigadee
{
    /// <summary>
    /// This is the base signature policy class for an entity.
    /// </summary>
    public class SignaturePolicyNull : ISignaturePolicy
    {

        public int? SignatureVersion => null;

        public bool VerificationPassedWithoutSignature { get; } = true;

        public string Calculate(object entity, int? versionid = null)
        {
            return null;
        }

        public bool Supports(Type entityType) => true;


        public bool Verify(object entity, string signature)
        {
            return true;
        }
    }

}
