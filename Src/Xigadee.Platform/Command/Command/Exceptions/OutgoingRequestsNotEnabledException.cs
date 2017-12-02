namespace Xigadee
{
    /// <summary>
    /// This exception is thrown when a command attempts to make an outgoing request, but this 
    /// capability has not been enabled in the policy settings.
    /// </summary>
    public class OutgoingRequestsNotEnabledException:CommandException
    {
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public OutgoingRequestsNotEnabledException():base("Outgoing requests are not enabled. Please set the command policy 'OutgoingRequestsEnabled' to true.")
        {

        }
    }
}
