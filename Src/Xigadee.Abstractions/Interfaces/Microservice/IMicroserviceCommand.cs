using System;
using System.Collections.Generic;

namespace Xigadee
{
    /// <summary>
    /// This interface defines the publicly accessible CommandContainer support.
    /// </summary>
    public interface IMicroserviceCommand: IEnumerable<ICommand>
    {
        /// <summary>
        /// Registers the specified command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>Returns the command passed in the request.</returns>
        ICommand Register(ICommand command);
        /// <summary>
        /// Gets the shared services collection.
        /// </summary>
        ISharedService SharedServices { get; }

    }
}
