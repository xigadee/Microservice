using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This exception is thrown when a duplicate authentication handler is added to the collection.
    /// </summary>
    public class AuthenticationHandlerInvalidSecretException: Exception
    {
        /// <summary>
        /// This is he default constructor.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        public AuthenticationHandlerInvalidSecretException(Exception ex)
            : base("The Authentication handler has an invalid secret passed through the configuration.", ex)
        {

        }
    }
}
