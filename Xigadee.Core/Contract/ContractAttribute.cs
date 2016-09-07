#region using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion
namespace Xigadee
{
    /// <summary>
    /// The contract attribute can be applied to IMessageContract to describe their specific routing data.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
    public class ContractAttribute : Attribute
    {
        private readonly ServiceMessageHeader mHeader;

        /// <summary>
        /// This is the default
        /// </summary>
        /// <param name="channelId">The channelId.</param>
        /// <param name="messageType">The message type.</param>
        /// <param name="messageAction">The action type.</param>
        public ContractAttribute(string channelId, string messageType, string messageAction)
        {
            mHeader = new ServiceMessageHeader(channelId, messageType, messageAction);
        }

        /// <summary>
        /// The channelId
        /// </summary>
        public string ChannelId { get { return mHeader.ChannelId; } }
        /// <summary>
        /// The message type.
        /// </summary>
        public string MessageType { get { return mHeader.MessageType; } }
        /// <summary>
        /// The action type.
        /// </summary>
        public string ActionType { get { return mHeader.ActionType; } }
        /// <summary>
        /// The converted header.
        /// </summary>
        public ServiceMessageHeader Header { get { return mHeader; } }
    }
}
