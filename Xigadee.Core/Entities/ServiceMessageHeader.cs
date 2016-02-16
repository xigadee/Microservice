#region using
using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Text;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This is the service message header identifier.
    /// </summary>
    [DebuggerDisplay("Header={ChannelId}({MessageType}|{ActionType})")]
    public struct ServiceMessageHeader:IEquatable<ServiceMessageHeader>
    {
        private string mChannelId, mMessageType, mActionType;

        public ServiceMessageHeader(string ChannelId, string MessageType = null, string ActionType = null)
        {
            mChannelId = ChannelId;
            mMessageType = MessageType;
            mActionType = ActionType;
        }

        public bool IsPartialKey { get { return mActionType == null || mMessageType == null || mChannelId == null; } }

        /// <summary>
        /// The channel id
        /// </summary>
        public string ChannelId { get { return mChannelId; } }
        /// <summary>
        /// The message channelId.
        /// </summary>
        public string MessageType { get { return mMessageType; } }
        /// <summary>
        /// The action channelId.
        /// </summary>
        public string ActionType { get { return mActionType; } }

        public string ToPartialKey()
        {
            StringBuilder sb = new StringBuilder();

            Action<string, string, string> action =
                (channelId, messageType, actionType) =>
            {
                if (string.IsNullOrEmpty(channelId))
                    return;
                sb.Append(channelId.ToLowerInvariant());

                if (string.IsNullOrEmpty(messageType))
                    return;
                sb.Append('/');
                sb.Append(messageType.ToLowerInvariant());

                if (string.IsNullOrEmpty(actionType))
                    return;
                sb.Append('/');
                sb.Append(actionType.ToLowerInvariant());
            };

            action(mChannelId, mMessageType, mActionType);

            return sb.ToString();
        }

        public string ToKey()
        {
            return ToKey(ChannelId, MessageType, ActionType);
        }

        #region ToKey...
        /// <summary>
        /// This extracts a service message request in to a single key request.
        /// </summary>
        /// <param name="message">The service message.</param>
        /// <returns>Returns the key.</returns>
        public static string ToKey(ServiceMessage message)
        {
            return ToKey(message.ChannelId, message.MessageType, message.ActionType);
        }

        /// <summary>
        /// This method returns a two-part command request in to a single request.
        /// </summary>
        /// <param name="messageType">The command channelId.</param>
        /// <param name="actionType">The command action.</param>
        /// <returns>Returns the key.</returns>
        public static string ToKey(string channelId, string messageType, string actionType)
        {
            channelId = (channelId ?? "").Trim().ToLowerInvariant();
            messageType = (messageType ?? "").Trim().ToLowerInvariant();
            actionType = (actionType ?? "").Trim().ToLowerInvariant();

            return string.Format("{0}/{1}/{2}", channelId, messageType, actionType);
        }
        #endregion

        public static ServiceMessageHeader ToServiceMessageHeader(string key)
        {
            string[] keys = key.Split('/');
            if (keys.Length != 3)
                throw new FormatException(string.Format("The key '{0}' is not of the format type/messageType/actionType", key));

            return new ServiceMessageHeader(keys[0], keys[1], keys[2]);
        }


        public bool Equals(ServiceMessageHeader other)
        {
            return ToKey() == other.ToKey();
        }

        public override bool Equals(object obj)
        {
            if (obj is ServiceMessageHeader)
                return Equals((ServiceMessageHeader)obj);
            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var result = ToKey().GetHashCode();
                return result;
            }
        }
    }
}
