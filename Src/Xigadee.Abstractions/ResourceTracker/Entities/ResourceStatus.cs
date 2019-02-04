namespace Xigadee
{
    /// <summary>
    /// This class contains a brief summary the resource status
    /// </summary>
    public class ResourceStatus
    {
        /// <summary>
        /// This is the name of the resource.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// This is the current circuit breaker state.
        /// </summary>
        public CircuitBreakerState State { get; set; } = CircuitBreakerState.Closed;
        /// <summary>
        /// This is the time to the next pool retry if the circuit breaker is open.
        /// </summary>
        public int? RetryInSeconds { get; set; }
        /// <summary>
        /// This is the percentage of requests that should pass through, if the circuit breaker is in a half-open state.
        /// </summary>
        public int FilterPercentage { get; set; } = 100;
    }
}
