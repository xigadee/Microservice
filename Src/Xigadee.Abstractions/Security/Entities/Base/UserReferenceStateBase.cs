using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Xigadee
{
    /// <summary>
    /// This class extends the user reference class with state history support.
    /// </summary>
    public abstract class UserReferenceStateBase : UserReferenceBase
    {
        #region StateChanges ...
        /// <summary>
        /// This is the list of historical state changes.
        /// </summary>
        public virtual List<StateChangeHistory> StateChanges { get; set; } = new List<StateChangeHistory>();
        /// <summary>
        /// This method changes the state to the new state and sets the state change history.
        /// </summary>
        /// <param name="newState">The new state.</param>
        /// <param name="description">The optional description.</param>
        /// <param name="memberName">The caller member name.</param>
        /// <param name="sourceLineNumber">The caller source line number.</param>
        public virtual void StateChange(string newState, string description = null
            , [CallerMemberName] string memberName = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            var sc = new StateChangeHistory();

            sc.StateOld = State;
            sc.StateNew = newState;
            sc.Description = description;

            sc.Caller = $"{memberName}@{sourceLineNumber}";

            State = newState;

            StateChanges.Add(sc);
        }
        #endregion

        #region State
        /// <summary>
        /// Gets or sets a value indicating the stage of the transfer.
        /// </summary>
        public virtual string State { get; set; }
        #endregion
    }

    /// <summary>
    /// This is the shared state change base.
    /// </summary>
    public class StateChangeHistory
    {
        /// <summary>
        /// The original state.
        /// </summary>
        public string StateOld { get; set; }
        /// <summary>
        /// The new state.
        /// </summary>
        public string StateNew { get; set; }
        /// <summary>
        /// The UTC time for the change.
        /// </summary>
        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
        /// <summary>
        /// The optional description.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// This is the caller information.
        /// </summary>
        public string Caller { get; set; }
    }
}
