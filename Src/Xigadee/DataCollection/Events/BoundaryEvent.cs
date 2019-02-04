using System;
using System.Diagnostics;

namespace Xigadee
{
    /// <summary>
    /// This is the event class for logging message boundary transitions.
    /// </summary>
    [DebuggerDisplay("{Type} ({ChannelId}|{ChannelPriority}) {Direction} [{Id}]")]
    public class BoundaryEvent: EventBase
    {
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        public BoundaryEventType Type { get; set; }
        /// <summary>
        /// Gets or sets the subtype.
        /// </summary>
        public string Subtype { get; set; }
        /// <summary>
        /// Gets or sets the direction, incoming or outgoing.
        /// </summary>
        public ChannelDirection Direction { get; set; }
        /// <summary>
        /// Gets or sets the trace identifier.
        /// </summary>
        public Guid? Id { get; set; }
        /// <summary>
        /// Gets or sets the exception.
        /// </summary>
        public virtual Exception Ex { get; set; }
        /// <summary>
        /// Gets or sets the payload.
        /// </summary>
        public TransmissionPayload Payload { get; set; }
        /// <summary>
        /// Gets or sets the batch identifier.
        /// </summary>
        public virtual Guid? BatchId { get; set; }
        /// <summary>
        /// Gets or sets the requested timestamp.
        /// </summary>
        public int Requested { get; set; }
        /// <summary>
        /// Gets or sets the actual timestamp.
        /// </summary>
        public int Actual { get; set; }
        /// <summary>
        /// Gets or sets the channel identifier.
        /// </summary>
        public virtual string ChannelId { get; set; }
        /// <summary>
        /// Gets or sets the channel priority.
        /// </summary>
        public virtual int ChannelPriority { get; set; }
    }
}
