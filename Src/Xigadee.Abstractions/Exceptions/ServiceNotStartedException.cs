using System;

namespace Xigadee
{
    /// <summary>
    /// This exception is thrown if a service receives input and it is not in a start state.
    /// </summary>
    public class ServiceNotStartedException:Exception
    {
        /// <summary>
        /// Initializes a new instance of the ServiceNotStartedException class.
        /// </summary>
        public ServiceNotStartedException() : base() { }
    }
}
