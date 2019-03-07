using System;
using System.Diagnostics;
using System.Text;
namespace Xigadee
{
    /// <summary>
    /// This is the service message header identifier.
    /// </summary>
    [DebuggerDisplay("Header={ChannelId}({MessageType}|{ActionType}) Key={Key} PKey={ToPartialKey()} Partial={IsPartialKey}")]
    public class ServiceMessageHeader: ServiceMessageHeaderFragment, IEquatable<ServiceMessageHeader>
    {
        /// <summary>
        /// This constructor is used to set the message from its component parts.
        /// </summary>
        /// <param name="channelId">The channel that the command is attached to.</param>
        /// <param name="messageType">The optional message type.</param>
        /// <param name="actionType">The optional action type.</param>
        public ServiceMessageHeader(string channelId, string messageType = null, string actionType = null):base(messageType,actionType)
        {
            ChannelId = NullCheck(channelId);

            IsPartialKey = ActionType == null || MessageType == null || ChannelId == null;
            Key = ToKey(ChannelId, MessageType, ActionType);
        }

        /// <summary>
        /// The channel identifier.
        /// </summary>
        public string ChannelId { get; }

        /// <summary>
        /// This property returns true if part of the key is not set.
        /// </summary>
        public bool IsPartialKey { get; }

        /// <summary>
        /// This is the key reference.
        /// </summary>
        public string Key { get; }

        #region IsMatch(ServiceMessageHeader other)
        /// <summary>
        /// This method matches two headers. It expands on equal and matches based on a partial key match.
        /// </summary>
        /// <param name="other">The other header to compare.</param>
        /// <returns>Returns true if it is a match.</returns>
        public bool IsMatch(ServiceMessageHeader other)
        {
            //If neither is partial then check on equality.
            if (!IsPartialKey)
                return Equals(other);

            if (ChannelId == null)
                return true;

            if (!string.Equals(ChannelId, other.ChannelId, StringComparison.InvariantCultureIgnoreCase))
                return false;

            if (MessageType == null)
                return true;

            if (!string.Equals(MessageType, other.MessageType, StringComparison.InvariantCultureIgnoreCase))
                return false;

            if (ActionType == null)
                return true;

            return string.Equals(ActionType, other.ActionType, StringComparison.InvariantCultureIgnoreCase);
        } 
        #endregion

        #region ToPartialKey()
        /// <summary>
        /// This method returns the partial key with the relevant parts in order by channelId / messageType / actionType
        /// if at any steps these are null, the key stops and returns.
        /// </summary>
        /// <returns>A string containing the partial key.</returns>
        public string ToPartialKey()
        {
            StringBuilder sb = new StringBuilder();

            bool cont = false;
            PartialPart(ChannelId, ref cont, false);
            PartialPart(MessageType, ref cont);
            PartialPart(ActionType, ref cont);

            sb.Append('/');

            return sb.ToString();

            void PartialPart(string item, ref bool skip, bool append = true)
            {
                if (skip) return;

                if (string.IsNullOrEmpty(item))
                    skip = true;
                else
                {
                    if (append) sb.Append('/');
                    sb.Append(item.ToLowerInvariant());
                }
            }
        } 
        #endregion

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
        /// <param name="channelId">The channel identifier.</param>
        /// <param name="messageType">The message type.</param>
        /// <param name="actionType">The action type.</param>
        /// <returns>Returns the key.</returns>
        public static string ToKey(string channelId, string messageType, string actionType)
        {
            channelId = (channelId ?? "").Trim();
            messageType = (messageType ?? "").Trim();
            actionType = (actionType ?? "").Trim();

            return $"{channelId}/{messageType}/{actionType}".ToLowerInvariant();
        }
        #endregion

        #region FromKey(string key)
        /// <summary>
        /// This method parses the key in to its constituent parts and returns a new ServiceMessageHeader struct. 
        /// </summary>
        /// <param name="key">The three part key as string.</param>
        /// <exception cref="System.FormatException">This exception is thrown if the key passed does not contain three forward slashes in the format of channelId/messageType/actionType</exception>
        /// <returns>A new ServiceMessageHeader object.</returns>
        public new static ServiceMessageHeader FromKey(string key)
        {
            string[] keys = key.Split('/');
            if (keys.Length != 3)
                throw new FormatException($"The key '{key}' is not of the format channelId/messageType/actionType");

            return new ServiceMessageHeader(keys[0], keys[1], keys[2]);
        } 
        #endregion

        #region Equals ...
        /// <summary>
        /// This is the equal override that matches other classes by use of their case insensitive keys.
        /// </summary>
        /// <param name="other">The header to match.</param>
        /// <returns>Returns true if the other class matches this class.</returns>
        public bool Equals(ServiceMessageHeader other)
        {
            if (other == null)
                return false;

            return Key == other.Key;
        }

        /// <summary>
        /// This is the equals override.
        /// </summary>
        /// <param name="obj">The incoming object.</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is ServiceMessageHeader)
                return Equals((ServiceMessageHeader)obj);
            return false;
        } 
        #endregion

        #region GetHashCode()
        /// <summary>
        /// This returns the hash code for the key.
        /// </summary>
        /// <returns>Returns the unsigned integer hashcode.</returns>
        public override int GetHashCode()
        {
            return Key.GetHashCode();
        } 
        #endregion

        #region Implicit conversion from (string,string,string)
        /// <summary>
        /// Implicitly converts three strings in to a ServiceMessageHeader.
        /// </summary>
        /// <param name="t">The value tuple.</param>
        public static implicit operator ServiceMessageHeader(ValueTuple<string, string, string> t)
        {
            return new ServiceMessageHeader(t.Item1, t.Item2, t.Item3);
        }
        #endregion
        #region Implicit conversion from (string,ServiceMessageHeaderFragmentg)
        /// <summary>
        /// Implicitly converts three strings in to a ServiceMessageHeader.
        /// </summary>
        /// <param name="t">The value tuple.</param>
        public static implicit operator ServiceMessageHeader(ValueTuple<string, ServiceMessageHeaderFragment> t)
        {
            return new ServiceMessageHeader(t.Item1, t.Item2?.MessageType, t.Item2?.ActionType);
        }
        #endregion
        #region Implicit conversion from string
        /// <summary>
        /// Implicitly converts three strings in to a ServiceMessageHeader.
        /// </summary>
        /// <param name="t">The value tuple.</param>
        public static implicit operator ServiceMessageHeader(string t)
        {
            return FromKey(t);
        }

        #endregion

        #region == overload
        /// <summary>
        /// This is the equals operator override.
        /// </summary>
        /// <param name="a">The first item.</param>
        /// <param name="b">The second item.</param>
        /// <returns>Returns true if they match.</returns>
        public static bool operator ==(ServiceMessageHeader a, ServiceMessageHeader b)
        {
            bool nullEither = object.ReferenceEquals(a, null) ^ object.ReferenceEquals(b, null);

            if (nullEither)
                return false;

            //This checks if both are null, then should be set to true.
            if (!nullEither && object.ReferenceEquals(a, null))
                return true;

            return a.Equals(b);
        }
        #endregion
        #region != overload
        /// <summary>
        /// This is the not equals operator override.
        /// </summary>
        /// <param name="a">The first item.</param>
        /// <param name="b">The second item.</param>
        /// <returns>Returns true if they do not match.</returns>
        public static bool operator !=(ServiceMessageHeader a, ServiceMessageHeader b)
        {
            return !(a == b);
        }
        #endregion

        /// <summary>
        /// This is the default any header.
        /// </summary>
        public static ServiceMessageHeader Any => (null, null, null);
    }
}
