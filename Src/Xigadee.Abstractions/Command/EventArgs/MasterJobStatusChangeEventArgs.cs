using System.Diagnostics;

namespace Xigadee
{
    /// <summary>
    /// This class contains the change information for the command.
    /// </summary>
    [DebuggerDisplay("{Debug()}")]
    public class MasterJobStatusChangeEventArgs : MasterJobEventArgsBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MasterJobStatusChangeEventArgs"/> class.
        /// </summary>
        /// <param name="state">The current state.</param>
        /// <param name="iteration">The current iteration.</param>
        public MasterJobStatusChangeEventArgs(MasterJobState state, long iteration):base(iteration)
        {
            State = state;
        }
        /// <summary>
        /// The previous state.
        /// </summary>
        public MasterJobState State { get; }
    }
}
