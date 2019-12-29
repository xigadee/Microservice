using System;
using System.Collections.Generic;
using System.Text;

namespace Xigadee
{
    /// <summary>
    /// This is the base signature policy class for an entity.
    /// </summary>
    public class SignaturePolicy
    {
        Type _signatureMaker;

        public SignaturePolicy(Type signatureMaker)
        {
            _signatureMaker = signatureMaker;

            
        }

        public string Calculate(object entity)
        {
            return "";
        }

        public bool Verify(object entity, string signature)
        {
            return true;
        }
    }

}
