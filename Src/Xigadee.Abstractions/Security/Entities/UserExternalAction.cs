using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Xigadee
{
    /// <summary>
    /// This class is used to store an async action for a user account, such as account creation confirmation, and/or 
    /// password reset completion.
    /// </summary>
    public class UserExternalAction: UserReferenceBase
    {
        /// <summary>
        /// Gets or sets the type of the action, i.e. Email, SMS, Push etc.
        /// </summary>
        [EntityPropertyHint("typeaction")]
        public string ActionType { get; set; }

        /// <summary>
        /// This property is used to specify a particular communication template, i.e. Registration confirmation, password reset, etc.
        /// </summary>
        [EntityPropertyHint("typetemplate")]
        public string TemplateType { get; set; }

        /// <summary>
        /// This property holds the current state of the action.
        /// </summary>
        [EntityPropertyHint("state")]
        public string State { get; set; }

        /// <summary>
        /// Gets or sets the option expiry date/time for the action.
        /// </summary>
        public DateTime? DateExpiry { get; set; }

        #region StateChanges ...
        /// <summary>
        /// This is the list of historical state changes.
        /// </summary>
        public List<UserExternalActionStateChange> StateChanges { get; set; } = new List<UserExternalActionStateChange>();
        /// <summary>
        /// This method changes the state to the new state and sets the state change history.
        /// </summary>
        /// <param name="newState">The new state.</param>
        /// <param name="description">The optional description.</param>
        public void StateChange(string newState, string description = null)
        {
            var sc = new UserExternalActionStateChange();

            sc.StateOld = State;
            sc.StateNew = newState;
            sc.Description = description;

            State = newState;

            StateChanges.Add(sc);
        }
        #endregion
    }

    /// <summary>
    /// This helper class holds the state change information.
    /// </summary>
    [DebuggerDisplay("{StateOld}->{StateNew} @ {TimeStamp}: {Description}")]
    public class UserExternalActionStateChange
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
    }
}
