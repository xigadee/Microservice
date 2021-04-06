using System;
using System.Linq;
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

        /// <summary>
        /// This method checks whether a state change has previously existed.
        /// </summary>
        /// <param name="sm">The state machine entity.</param>
        /// <param name="state">The state to check for.</param>
        /// <returns>Returns true if the change exists.</returns>
        public static bool StateChangeExists(this IEntityState sm, string state)
            => sm.State == state || (sm.StateChanges?.Exists(sc => sc.StateNew == state) ?? false);
        /// <summary>
        /// This extension method returns the last state change.
        /// </summary>
        /// <param name="sm">The state machine entity.</param>
        /// <param name="condition">The optional condition used to select a set of states. If this is null or not supplied, the last state change based on date is returned</param>
        /// <returns>Returns the last state change or null if there are no changes, or none match the condition.</returns>
        public static StateChangeHistory LastStateChange(this IEntityState sm, Func<StateChangeHistory, bool> condition = null)
            => sm.StateChanges?.OrderBy(s => s.TimeStamp).LastOrDefault(s => condition?.Invoke(s) ?? true);

    }
}
