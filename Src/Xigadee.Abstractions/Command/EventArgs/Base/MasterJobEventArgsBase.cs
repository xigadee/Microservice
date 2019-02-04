using System;

namespace Xigadee
{
    /// <summary>
    /// This is base class for master job event arguments
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public abstract class MasterJobEventArgsBase: CommandEventArgsBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MasterJobEventArgsBase"/> class.
        /// </summary>
        /// <param name="iteration">The iteration identifier.</param>
        protected MasterJobEventArgsBase(long? iteration = null)
        {
            Iteration = iteration;
        }
        /// <summary>
        /// Gets the iteration.
        /// </summary>
        public long? Iteration { get; }
        /// <summary>
        /// Gets the time stamp.
        /// </summary>
        public DateTime TimeStamp { get; } = DateTime.UtcNow;
    }
}
