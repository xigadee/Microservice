namespace Xigadee
{
    /// <summary>
    /// This exception is used during start up when validating the messaging setting
    /// </summary>
    public class CommunicationAgentStartupException:MessagingException
    {
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="property">The property that has caused the error.</param>
        /// <param name="message">The error message.</param>
        public CommunicationAgentStartupException(string property, string message):base(message)
        {
            Property = property;
        }
        /// <summary>
        /// This is the property that has caused the error.
        /// </summary>
        public string Property { get; }
    }

}
