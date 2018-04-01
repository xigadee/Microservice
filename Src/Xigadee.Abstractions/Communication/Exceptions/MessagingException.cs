#region using
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#endregion
namespace Xigadee
{
    /// <summary>
    /// This is the base abstract exception for messaging exceptions
    /// </summary>
    public abstract class MessagingException: Exception
    {
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="message">The message to pass.</param>
        protected MessagingException(string message) : base(message) { }
    }
}
