using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This attribute can be set against a method to register it for automatic registration as a remote command.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class CommandMethodAttribute:Attribute
    {
        public CommandMethodAttribute(string messageType, string actionType, string channelId = null)
        {
            ChannelId = channelId;
            MessageType = messageType;
            ActionType = actionType;
        }

        public string ChannelId { get; protected set; }

        public string MessageType { get; protected set; }

        public string ActionType { get; protected set; }
    }
}
