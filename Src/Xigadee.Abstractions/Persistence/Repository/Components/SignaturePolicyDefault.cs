using System;
using System.Collections.Generic;
using System.Text;

namespace Xigadee
{
    /// <summary>
    /// This is the base signature policy class for an entity.
    /// </summary>
    public class SignaturePolicyDefault : ISignaturePolicy
    {
        Type _signatureMaker;

        public SignaturePolicyDefault(Type signatureMaker)
        {
            _signatureMaker = signatureMaker;

        }

        public string Calculate(object entity)
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
