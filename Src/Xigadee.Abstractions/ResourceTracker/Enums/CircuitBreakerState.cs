using System;

namespace Xigadee
{
    /// <summary>
    /// This enumeration contains the state of a resource currently being managed.
    /// </summary>
    [Flags()]
    public enum CircuitBreakerState: int
    {
        /// <summary>
        /// Any requests fail immediately as the service has reached the threshold failure.
        /// </summary>
        Open = 1,
        /// <summary>
        /// All is good. The service is operating as normal.
        /// </summary>
        Closed = 2,
        /// <summary>
        /// The service is tentatively accepting requests on a ratio. If the calls succeed the circuit breaker will close, 
        /// otherwise the breaker will reopen. This is used during a retry state after the breaker has opened. It is used to ensure 
        /// we do not inundate the underlying service with too many calls when we restart.
        /// </summary>
        HalfOpen = 3
    }
}
