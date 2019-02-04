namespace Xigadee
{
    /// <summary>
    /// This interface is implemented by Rate Limiters.
    /// </summary>
    public interface IResourceRequestRateLimiter: IResourceBase
    {
        /// <summary>
        /// This is the rate that the request process should adjust as a ratio 1 is the default without limiting.
        /// 0 is when the circuit breaker is active.
        /// </summary>
        double RateLimitAdjustmentPercentage { get; }
    }
}
