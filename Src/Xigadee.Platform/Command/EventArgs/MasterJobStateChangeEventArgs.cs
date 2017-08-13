using System.Diagnostics;

namespace Xigadee
{
    /// <summary>
    /// This class contains the change information for the command.
    /// </summary>
    [DebuggerDisplay("{Debug()}")]
    public class MasterJobStateChangeEventArgs: MasterJobEventArgsBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MasterJobStateChangeEventArgs"/> class.
        /// </summary>
        /// <param name="oldState">The old state.</param>
        /// <param name="newState">The new state.</param>
        /// <param name="iteration">The current iteration.</param>
        public MasterJobStateChangeEventArgs(MasterJobState oldState, MasterJobState newState, long? iteration = null) : base(iteration)
        {
            StateOld = oldState;
            StateNew = newState;
        }
        /// <summary>
        /// The previous state.
        /// </summary>
        public MasterJobState StateOld { get; }
        /// <summary>
        /// The new master job state.
        /// </summary>
        public MasterJobState StateNew { get; }

        /// <summary>
        /// Shows a debug string for the event.
        /// </summary>
        public string Debug()
        {
            return $"State Change - {ServiceId}/{CommandName}: {StateOld} > {StateNew} @ {TimeStamp} - {Iteration}\r\n";
        }
    }
}
