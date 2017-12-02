using System;

namespace Xigadee
{
    /// <summary>
    /// This event class is fired when a CommandHarnessRequest object is generated.
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class CommandHarnessEventArgs:EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandHarnessEventArgs"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="ev">The request.</param>
        public CommandHarnessEventArgs(long id, CommandHarnessTraffic ev)
        {
            Id = id;
            Event = ev;
        }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        public long Id { get; }

        /// <summary>
        /// Gets the request.
        /// </summary>
        public CommandHarnessTraffic Event { get; }
    }
}
