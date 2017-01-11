using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This exception is thrown when a duplicate encryption handler is added to the collection.
    /// </summary>
    public class EncryptionHandlerAlreadyExistsException: Exception
    {
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        public EncryptionHandlerAlreadyExistsException(string identifier)
            : base($"The Encryption handler '{identifier}' already exists in the security collection.")
        {

        }
    }
}
