using System;

namespace Xigadee
{
    /// <summary>
    /// The contract attribute can be applied to IMessageContract to describe their specific routing data.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
    public class ContractAttribute : Attribute
    {
        /// <summary>
        /// This is the default constructor used to create an attribute which decorates a message contract.
        /// </summary>
        /// <param name="channelId">The channelId.</param>
        /// <param name="messageType">The message type.</param>
        /// <param name="messageAction">The action type.</param>
        public ContractAttribute(string channelId, string messageType, string messageAction)
        {
            Header = new ServiceMessageHeader(channelId, messageType, messageAction);
        }
        /// <summary>
        /// The converted header.
        /// </summary>
        public ServiceMessageHeader Header { get; }

        /// <summary>
        /// The channelId
        /// </summary>
        public string ChannelId { get { return Header.ChannelId; } }
        /// <summary>
        /// The message type.
        /// </summary>
        public string MessageType { get { return Header.MessageType; } }
        /// <summary>
        /// The action type.
        /// </summary>
        public string ActionType { get { return Header.ActionType; } }

    }
}
