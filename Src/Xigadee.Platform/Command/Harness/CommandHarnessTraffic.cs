using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Xigadee
{
    /// <summary>
    /// This class contains a list of requests that have been processed by the command harness.
    /// </summary>
    [DebuggerDisplay("{Direction}:{Tracker.Type}={Id} Success={IsSuccess} @{UTCCreated}")]
    public class CommandHarnessTraffic
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandHarnessTraffic"/> class.
        /// </summary>
        /// <param name="direction">The request direction: incoming or outgoing.</param>
        /// <param name="tracker">The request.</param>
        /// <param name="referenceId">The reference identifier.</param>
        /// <param name="originatorId">Initial identifier.</param>
        public CommandHarnessTraffic(CommandHarnessTrafficDirection direction
            , TaskTracker tracker
            , string referenceId
            , long? originatorId = null)
        {
            Direction = direction;
            Id = tracker?.Id ?? Guid.NewGuid();
            Tracker = tracker;
            Responses = new List<TransmissionPayload>();
            ReferenceId = referenceId;
            OriginatorId = originatorId;
        }

        /// <summary>
        /// Gets the direction of the request.
        /// </summary>
        public CommandHarnessTrafficDirection Direction { get; }
        /// <summary>
        /// Gets the initial reference.
        /// </summary>
        public long? OriginatorId { get; }
        /// <summary>
        /// Gets the incoming payload identifier.
        /// </summary>
        public Guid Id { get; }
        /// <summary>
        /// Gets the request.
        /// </summary>
        public TaskTracker Tracker { get; }
        /// <summary>
        /// Gets any responses returned by the command as part of the standard request/response call methods.
        /// </summary>
        public List<TransmissionPayload> Responses { get; }
        /// <summary>
        /// Gets or sets any uncaught exception raised during the request.
        /// </summary>
        public Exception Exception { get; set; }
        /// <summary>
        /// Gets a value indicating whether this request is a success.
        /// </summary>
        public bool IsSuccess => Exception == null;
        /// <summary>
        /// Gets the incoming reference identifier.
        /// </summary>
        public string ReferenceId { get; }

        /// <summary>
        /// Gets the tick count.
        /// </summary>
        public DateTime UTCCreated { get; } = DateTime.UtcNow;
    }
}
