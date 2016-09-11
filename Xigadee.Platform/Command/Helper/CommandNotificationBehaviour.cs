using System;
using System.Threading;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This enum defines how command registration is notified to the controlling container.
    /// </summary>
    public enum CommandNotificationBehaviour
    {
        /// <summary>
        /// Commands will be registered on startup
        /// </summary>
        OnStartUp,
        /// <summary>
        /// Commands will be registered when 
        /// </summary>
        OnRegistration,
        /// <summary>
        /// Commands will be automatically registered if the command has started.
        /// </summary>
        OnRegistrationIfStarted,
        /// <summary>
        /// Commands will not be automatically registers.
        /// </summary>
        Manual
    }
}
