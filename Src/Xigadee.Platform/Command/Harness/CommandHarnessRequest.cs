using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Xigadee
{
    /// <summary>
    /// This class contains a list of requests that have been processed by the command harness.
    /// </summary>
    [DebuggerDisplay("Id={Id} Success={IsSuccess} @{TickCount}")]
    public class CommandHarnessRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandHarnessRequest"/> class.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="referenceId">The reference identifier.</param>
        public CommandHarnessRequest(TransmissionPayload request, string referenceId)
        {
            Id = request?.Id ?? Guid.NewGuid();
            Request = request;
            Responses = new List<TransmissionPayload>();
            ReferenceId = referenceId;
        }

        /// <summary>
        /// Gets the incoming payload identifier.
        /// </summary>
        public Guid Id { get; }
        /// <summary>
        /// Gets the request.
        /// </summary>
        public TransmissionPayload Request { get; }
        /// <summary>
        /// Gets any responses.
        /// </summary>
        public List<TransmissionPayload> Responses { get; }
        /// <summary>
        /// Gets or sets the exception.
        /// </summary>
        public Exception Exception { get; set; }
        /// <summary>
        /// Gets a value indicating whether this request is success.
        /// </summary>
        public bool IsSuccess => Exception == null;
        /// <summary>
        /// Gets the incoming reference identifier.
        /// </summary>
        public string ReferenceId { get; }

        /// <summary>
        /// Gets the tick count.
        /// </summary>
        public int TickCount { get; } = Environment.TickCount;
    }
}
