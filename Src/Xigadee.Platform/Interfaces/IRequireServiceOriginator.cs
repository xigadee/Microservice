namespace Xigadee
{
    /// <summary>
    /// This interface is implemented by services that require information about the service they are running under.
    /// </summary>
    public interface IRequireServiceOriginator
    {
        /// <summary>
        /// The originator Id for the service.
        /// </summary>
        MicroserviceId OriginatorId { get; set; }
    }
}
