namespace Xigadee
{
    /// <summary>
    /// This interface is implemented by a generic persistence message initiator.
    /// </summary>
    /// <seealso cref="Xigadee.IStatisticsBase" />
    public interface IPersistenceMessageInitiator: IStatisticsBase
    {
        /// <summary>
        /// Gets the statistics.
        /// </summary>
        PersistenceClientStatistics Statistics { get; }
    }
}
