using System;
using System.Runtime.CompilerServices;

namespace Xigadee
{
    /// <summary>
    /// This is an extension helper class for the Property Bag.
    /// </summary>
    public static class StateChangeHelper
    {
        /// <summary>
        /// This method changes the state to the new state and sets the state change history.
        /// </summary>
        /// <param name="entity">The entity to apply the state change.</param>
        /// <param name="newState">The new state.</param>
        /// <param name="description">The optional description.</param>
        /// <param name="memberName">The caller member name.</param>
        /// <param name="sourceLineNumber">The caller source line number.</param>
        public static void StateChange(this IEntityState entity, string newState, string description = null
            , [CallerMemberName] string memberName = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            var sc = new StateChangeHistory();

            sc.StateOld = entity.State;
            sc.StateNew = newState;
            sc.Description = description;

            sc.Caller = $"{memberName}@{sourceLineNumber}";

            entity.State = newState;

            entity.StateChanges.Add(sc);
        }
    }
}
