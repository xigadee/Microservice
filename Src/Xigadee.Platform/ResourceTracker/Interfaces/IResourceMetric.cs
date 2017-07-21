namespace Xigadee
{
    /// <summary>
    /// This interface contains the core values that are used to measure a resource.
    /// </summary>
    public interface IResourceMetric//: IResourceBase
    {
        string[] Active { get; }

        double RateLimitAdjustmentPercentage { get; }

        double RateLimitCutoutPercentage { get; }

        double RetryRatio { get; }

        int RetrySum { get; }

        string Name { get; }
    }
}