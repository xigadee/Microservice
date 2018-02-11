using System;
using System.Collections.Generic;
using System.Text;

namespace Xigadee
{
    /// <summary>
    /// This exception is used during start up when validating the messaging setting
    /// </summary>
    public class ClientsUndefinedMessagingException: MessagingException
    {
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="message">The error message.</param>
        public ClientsUndefinedMessagingException(string message) : base(message)
        {
        }

    }
}
