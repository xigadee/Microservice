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
    }

    /// <summary>
    /// This is the default signature policy class for a default entity..
    /// </summary>
    /// <typeparam name="E">The entity type E.</typeparam>
    public class SignaturePolicy<E>
    {
        public SignaturePolicy(Type signatureMaker)
        {

        }
    }
}
