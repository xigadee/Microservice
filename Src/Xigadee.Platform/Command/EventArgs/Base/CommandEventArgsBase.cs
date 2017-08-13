using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This is the base class for command event arguments.
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public abstract class CommandEventArgsBase:EventArgs
    {
        /// <summary>
        /// This is the service name.
        /// </summary>
        public string ServiceId { get; set; }
        /// <summary>
        /// Gets or sets the name of the command.
        /// </summary>
        public string CommandName { get; set; }
    }
}
