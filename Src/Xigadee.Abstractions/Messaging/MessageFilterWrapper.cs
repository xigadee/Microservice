#region using
using System;
using System.Diagnostics;
#endregion
namespace Xigadee
{
    /// <summary>
    /// The MessageFilterWrapper class allows for extended filter functionality for the messaging bus.
    /// </summary>
    [DebuggerDisplay("{Header.Key}|{ClientId}")]
    public class MessageFilterWrapper: IEquatable<MessageFilterWrapper>
    {
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="header">THe actual service message header.</param>
        /// <param name="clientId">This is the optional client id.</param>
        public MessageFilterWrapper(ServiceMessageHeader header, string clientId = null)
        {
            Header = header;
            ClientId = clientId;
        }

        /// <summary>
        /// This is the message header.
        /// </summary>
        public ServiceMessageHeader Header { get; }

        /// <summary>
        /// This is the specific client id for the message.
        /// </summary>
        public string ClientId { get; }

        #region GetHashCode()
        /// <summary>
        /// This is the Hashcode override.
        /// </summary>
        /// <returns>This is the calculated hashcode.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var result = 0;

                result = (result * 397) ^ SafeHashCode(ClientId);

                result = (result * 397) ^ SafeHashCode(Header.Key);

                return result;
            }
        }
        #endregion
        #region SafeHashCode(object item)
        /// <summary>
        /// This helper method gets the Hashcode for the item or returns 0 if the object is null.
        /// </summary>
        /// <param name="item">The item to get the hashcode for.</param>
        /// <returns>The hashcode or 0.</returns>
        private int SafeHashCode(object item)
        {
            if (item == null)
                return 0;

            return item.GetHashCode();
        }
        #endregion

        #region Equals...
        /// <summary>
        /// This is the base Equals method.
        /// </summary>
        /// <param name="other">The other object to compare.</param>
        /// <returns>Returns true if the object is a wrapper and the parameters match.</returns>
        public override bool Equals(object other)
        {
            if (other == null || !(other is MessageFilterWrapper))
                return false;

            return Equals((MessageFilterWrapper)other);
        }

        /// <summary>
        /// This method compares the two wrappers.
        /// </summary>
        /// <param name="other">The other wrapper to compare against this one.</param>
        /// <returns>Returns true if there is equality.</returns>
        public bool Equals(MessageFilterWrapper other)
        {
            if (other == null)
                return false;

            return ClientId == other.ClientId
                && Header.Equals(other.Header);
        }
        #endregion

        #region Implicit conversion from ServiceMessageHeader with null client
        /// <summary>
        /// Implicitly converts three strings in to a ServiceMessageHeader.
        /// </summary>
        /// <param name="t">The value tuple.</param>
        public static implicit operator MessageFilterWrapper(ServiceMessageHeader t)
        {
            return new MessageFilterWrapper(t);
        }
        #endregion
        #region Implicit conversion from (ServiceMessageHeaderFragmentg, string)
        /// <summary>
        /// Implicitly converts three strings in to a ServiceMessageHeader.
        /// </summary>
        /// <param name="t">The value tuple.</param>
        public static implicit operator MessageFilterWrapper(ValueTuple<ServiceMessageHeader, string> t)
        {
            return new MessageFilterWrapper(t.Item1, t.Item2);
        }
        #endregion
    }
}
