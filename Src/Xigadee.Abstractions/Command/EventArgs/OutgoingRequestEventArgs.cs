using System;

namespace Xigadee
{
    /// <summary>
    /// This class contains the change information for the command.
    /// </summary>
    public class OutgoingRequestEventArgs: CommandEventArgsBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OutgoingRequestEventArgs"/> class.
        /// </summary>
        /// <param name="request">The request.</param>
        public OutgoingRequestEventArgs(OutgoingRequest request)
        {
            Request = request;
        }
        /// <summary>
        /// Gets the outgoing request.
        /// </summary>
        public OutgoingRequest Request { get; }
    }
}
