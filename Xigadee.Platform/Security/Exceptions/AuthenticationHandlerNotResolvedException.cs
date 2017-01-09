using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This exception is thrown when an encryption handler id not found in the collection.
    /// </summary>
    public class AuthenticationHandlerNotResolvedException: Exception
    {
        /// <summary>
        /// This is he default constructor.
        /// </summary>
        /// <param name="channelId">The channel id.</param>
        /// <param name="id">The encryption handler identifier.</param>
        public AuthenticationHandlerNotResolvedException(string channelId, string id)
            : base($"Autheentication handler '{id}' for channel '{channelId}' was not found in the security collection")
        {

        }
    }
}
