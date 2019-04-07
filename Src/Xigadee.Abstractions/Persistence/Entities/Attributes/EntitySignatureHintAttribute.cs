using System;
using System.Collections.Generic;
using System.Text;

namespace Xigadee
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class EntitySignatureHintAttribute:Attribute
    {
        public EntitySignatureHintAttribute(Type signatureClass)
        {
            SignatureClass = signatureClass;
        }

        private Type SignatureClass { get; }

    }
}
