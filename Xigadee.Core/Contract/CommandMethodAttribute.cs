using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This attribute can be set against a command method to register it for automatic registration as a remote command.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class CommandContractAttribute:Attribute
    {
        public CommandContractAttribute(string messageType, string actionType, string channelId = null)
        {
            ChannelId = channelId;
            MessageType = messageType;
            ActionType = actionType;
            Header = new Xigadee.ServiceMessageHeader(channelId, messageType, actionType);
        }

        public string ChannelId { get; protected set; }

        public string MessageType { get; protected set; }

        public string ActionType { get; protected set; }

        /// <summary>
        /// The converted header.
        /// </summary>
        public ServiceMessageHeader Header { get; protected set; }
    }
}
