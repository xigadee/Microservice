using System;

namespace Xigadee
{
    [Flags()]
    public enum CircuitBreakerState: int
    {
        Open = 1,
        Closed = 2,
        HalfOpen = 3
    }
}
