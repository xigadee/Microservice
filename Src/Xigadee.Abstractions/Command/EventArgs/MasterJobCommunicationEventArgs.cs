using System.Diagnostics;

namespace Xigadee
{
    /// <summary>
    /// This class contains the change information for the command.
    /// </summary>
    [DebuggerDisplay("{Debug()}")]
    public class MasterJobCommunicationEventArgs: MasterJobEventArgsBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MasterJobCommunicationEventArgs"/> class.
        /// </summary>
        /// <param name="direction">The direction.</param>
        /// <param name="state">The state.</param>
        /// <param name="action">The action.</param>
        /// <param name="iteration">The current iteration.</param>
        /// <param name="originatorId">The originator identifier.</param>
        public MasterJobCommunicationEventArgs(MasterJobCommunicationDirection direction
            , MasterJobState state, string action, long iteration, string originatorId = null)
            :base(iteration)
        {
            State = state;
            Action = action;
            Direction = direction;
            OriginatorId = originatorId;
        }
        /// <summary>
        /// Gets the message originator identifier.
        /// </summary>
        public string OriginatorId { get; }
        /// <summary>
        /// Gets the direction.
        /// </summary>
        public MasterJobCommunicationDirection Direction { get; }
        /// <summary>
        /// The previous state.
        /// </summary>
        public MasterJobState State { get; }
        /// <summary>
        /// The new master job state.
        /// </summary>
        public string Action { get; }

        /// <summary>
        /// Shows a debug string for the event.
        /// </summary>
        public string Debug()
        {
            return $"Message {Direction} - {ServiceName}/{CommandName}: {State} --> {Action} @ {TimeStamp} - {Iteration} [{OriginatorId??ServiceName}]\r\n";
        }
    }
}
