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
        Type _signatureMaker;

        public SignaturePolicyNull(Type signatureMaker)
        {
            _signatureMaker = signatureMaker;

        }

        public int? SignatureVersion => null;

        public bool ReadPermittedWithoutSignature => false;

        public string Calculate(object entity, int? versionid = null)
        {
            return "";
        }

        public void RegisterChildPolicy(ISignaturePolicy childPolicy)
        {
        }

        public bool Supports(Type entityType) => true;


        public bool Verify(object entity, string signature)
        {
            return true;
        }
    }

}
