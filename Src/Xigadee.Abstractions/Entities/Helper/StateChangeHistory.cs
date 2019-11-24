using System;
using System.Collections.Generic;
using System.Text;

namespace Xigadee
{
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
