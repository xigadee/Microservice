using System.Collections.Generic;

namespace Xigadee
{
    /// <summary>
    /// This interface is implemented by entities that use a state machine.
    /// </summary>
    public interface IEntityState
    {
        /// <summary>
        /// The current state.
        /// </summary>
        string State { get; set; }
        /// <summary>
        /// The history of state changes.
        /// </summary>
        List<StateChangeHistory> StateChanges { get; set; }
    }
}