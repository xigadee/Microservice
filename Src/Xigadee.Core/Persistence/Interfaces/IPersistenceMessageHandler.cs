namespace Xigadee
{
    /// <summary>
    /// This interface is used to consistently reference the persistence handler irrespective  
    /// of the underlying implementation.
    /// </summary>
    public interface IPersistenceMessageHandler: IStatisticsBase
    {
        string ChannelId { get; set; }

        string EntityType { get; }

    }
}