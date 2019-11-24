using System.Collections.Generic;

namespace Xigadee
{
    /// <summary>
    /// This class extends the user reference class with state history support.
    /// </summary>
    public abstract class UserReferenceStateBase : UserReferenceBase, IEntityState
    {
        /// <summary>
        /// This is the list of historical state changes.
        /// </summary>
        public virtual List<StateChangeHistory> StateChanges { get; set; } = new List<StateChangeHistory>();
        /// <summary>
        /// Gets or sets a value indicating the stage of the transfer.
        /// </summary>
        public virtual string State { get; set; }
    }
}
