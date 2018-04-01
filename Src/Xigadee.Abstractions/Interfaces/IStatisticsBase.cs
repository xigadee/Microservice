namespace Xigadee
{
    /// <summary>
    /// This interface is used to expose the generic statistics methods across multiple components
    /// that allow the system wide statistics tree to be generated.
    /// </summary>
    public interface IStatisticsBase
    {
        /// <summary>
        /// Gets the component statistics.
        /// </summary>
        /// <returns>The statistics class.</returns>
        StatusBase StatisticsGet();
    }
}