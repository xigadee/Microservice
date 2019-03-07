using System;
using System.Diagnostics;
namespace Xigadee
{
    /// <summary>
    /// This class holds information for a command destination when the channel is not known.
    /// </summary>
    [DebuggerDisplay("({MessageType}|{ActionType})")]
    public class ServiceMessageHeaderFragment
    {
        /// <summary>
        /// This constructor takes the message and action type parameters.
        /// </summary>
        /// <param name="messageType">The message type.</param>
        /// <param name="actionType">The action type.</param>
        public ServiceMessageHeaderFragment(string messageType = null, string actionType = null)
        {
            MessageType = NullCheck(messageType);
            ActionType = NullCheck(actionType);
        }

        /// <summary>
        /// The message type.
        /// </summary>
        public string MessageType { get; }

        /// <summary>
        /// The action type.
        /// </summary>
        public string ActionType { get; }

        #region NullCheck(string incoming)
        /// <summary>
        /// This method ensures that whitespace or an empty string passed as an incoming parameter is converted to null.
        /// </summary>
        /// <param name="incoming">The incoming string.</param>
        /// <returns>The outgoing string, or null if this string is just whitespace or is an empty string.</returns>
        protected string NullCheck(string incoming)
        {
            if (string.IsNullOrWhiteSpace(incoming))
                return null;

            return incoming.Trim();
        }
        #endregion

        #region IsMatch(ServiceMessageHeaderFragment other)
        /// <summary>
        /// This method matches two headers. It expands on equal and matches based on a partial key match.
        /// </summary>
        /// <param name="other">The other header to compare.</param>
        /// <returns>Returns true if it is a match.</returns>
        public bool IsMatch(ServiceMessageHeaderFragment other)
        {
            if (MessageType == null)
                return true;

            if (!string.Equals(MessageType, other.MessageType, StringComparison.InvariantCultureIgnoreCase))
                return false;

            if (ActionType == null)
                return true;

            return string.Equals(ActionType, other.ActionType, StringComparison.InvariantCultureIgnoreCase);
        }
        #endregion

        #region FromKey(string key)
        /// <summary>
        /// This method parses the key in to its constituent parts and returns a new ServiceMessageHeader struct. 
        /// </summary>
        /// <param name="key">The three part key as string.</param>
        /// <exception cref="System.FormatException">This exception is thrown if the key passed does not contain three forward slashes in the format of channelId/messageType/actionType</exception>
        /// <returns>A new ServiceMessageHeader object.</returns>
        public static ServiceMessageHeaderFragment FromKey(string key)
        {
            string[] keys = key.Split('/');
            if (keys.Length != 2)
                throw new FormatException($"The key '{key}' is not of the format messageType/actionType");

            return new ServiceMessageHeaderFragment(keys[0], keys[1]);
        }
        #endregion

        #region Implicit conversion from (string,string)
        /// <summary>
        /// Implicitly converts three strings in to a ServiceMessageHeader.
        /// </summary>
        /// <param name="t">The value tuple.</param>
        public static implicit operator ServiceMessageHeaderFragment(ValueTuple<string, string> t)
        {
            return new ServiceMessageHeaderFragment(t.Item1, t.Item2);
        }
        #endregion
    }
}
