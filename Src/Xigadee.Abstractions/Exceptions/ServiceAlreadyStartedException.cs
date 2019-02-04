using System;

namespace Xigadee
{
    /// <summary>
    /// This exception is thrown if a command receives a start request and is already running.
    /// </summary>
    public class ServiceAlreadyStartedException:Exception
    {
        /// <summary>
        /// Initializes a new instance of the ServiceNotStartedException class.
        /// </summary>
        public ServiceAlreadyStartedException() : base() { }
    }
}
