using System;
using System.Collections.Generic;
using System.Text;

namespace Xigadee
{
    /// <summary>
    /// This exception is thrown when an entity signature does not match the signature passed.
    /// </summary>
    public class SignatureEntityVerificationException:Exception
    {
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public SignatureEntityVerificationException()
        {

        }

    }
}
