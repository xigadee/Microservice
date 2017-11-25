namespace Xigadee
{
    /// <summary>
    /// This interface is used to consistently reference the persistence handler irrespective  
    /// of the underlying implementation.
    /// </summary>
    public interface IPersistenceMessageHandler: IStatisticsBase
    {
        /// <summary>
        /// Gets or sets the channel identifier.
        /// </summary>
        string ChannelId { get; set; }
        /// <summary>
        /// Gets the type of the entity.
        /// </summary>
        /// <value>
        /// The type of the entity.
        /// </value>
        string EntityType { get; }

    }
}