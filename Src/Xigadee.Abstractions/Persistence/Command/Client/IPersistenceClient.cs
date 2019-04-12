namespace Xigadee
{
    /// <summary>
    /// This interface is implemented by the generic persistence client.
    /// </summary>
    public interface IPersistenceClientCommand: ICommand, IPersistenceMessageInitiator
    {
        /// <summary>
        /// This is the entity type.
        /// </summary>
        string EntityType { get; }
        /// <summary>
        /// This is the default routing behavior.
        /// </summary>
        ProcessOptions? RoutingDefault { get; set; }
    }
}