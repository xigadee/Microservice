using System;

namespace Xigadee
{
    /// <summary>
    /// This abstract class is used to implement circuit breaker functionality.
    /// </summary>
    public abstract class CircuitBreakerBase
    {
    }

    [Flags()]
    public enum CircuitBreakerState:int
    {
        Open = 1,
        Closed = 2,
        HalfOpen = 3
    }
}
