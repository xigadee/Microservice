using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This exception is thrown when a request is sent to a command that is not recognised.
    /// </summary>
    public class CommandNotSupportedException:Exception
    {
        /// <summary>
        /// This exception is thrown when a request is sent to a command that is not recognised.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="commandType">The command type.</param>
        public CommandNotSupportedException(Guid id, ServiceMessageHeader header, Type commandType):base($"This command is not supported: '{header}' in {commandType.Name}")
        {
            Id = id;
            Header = header;
            Command = commandType.Name;
        }
        /// <summary>
        /// This is the payload id.
        /// </summary>
        public Guid Id { get; }
        /// <summary>
        /// This is the incoming message header.
        /// </summary>
        public ServiceMessageHeader Header { get; }
        /// <summary>
        /// This is the command name that raised the error.
        /// </summary>
        public string Command { get; }
    }
}
