namespace Xigadee
{
    /// <summary>
    /// This interface is used to identify the persistence shared service.
    /// </summary>
    /// <seealso cref="Xigadee.IStatisticsBase" />
    public interface IPersistenceSharedService: IStatisticsBase
    {
        /// <summary>
        /// Gets the statistics for the persistence client.
        /// </summary>
        PersistenceClientStatistics Statistics { get; }
    }
}
