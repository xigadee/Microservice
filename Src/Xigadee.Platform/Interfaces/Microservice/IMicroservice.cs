namespace Xigadee
{
    /// <summary>
    /// This interface defines the Microservice definition.
    /// </summary>
    public interface IMicroservice: IService
    {
        /// <summary>
        /// Gets the Microservice identifiers.
        /// </summary>
        MicroserviceId Id { get; }
        /// <summary>
        /// Gets the current service status.
        /// </summary>
        ServiceStatus Status { get; }
        /// <summary>
        /// Gets the events service.
        /// </summary>
        IMicroserviceEvents Events { get; }
        /// <summary>
        /// Gets the security service.
        /// </summary>
        IMicroserviceSecurity Security { get; }
        /// <summary>
        /// Gets the communication service.
        /// </summary>
        IMicroserviceCommunication Communication { get; }
        /// <summary>
        /// Gets the dispatcher service.
        /// </summary>
        IMicroserviceDispatch Dispatch { get; }
        /// <summary>
        /// Gets the commands collection service.
        /// </summary>
        IMicroserviceCommand Commands { get; }
        /// <summary>
        /// Gets the service configuration policies.
        /// </summary>
        IMicroservicePolicy Policies { get; }
        /// <summary>
        /// Gets the data collection service.
        /// </summary>
        IMicroserviceDataCollection DataCollection { get; }
        /// <summary>
        /// Gets the serialization service.
        /// </summary>
        IMicroserviceSerialization Serialization { get; }
        /// <summary>
        /// Gets the resource monitor service.
        /// </summary>
        IMicroserviceResourceMonitor ResourceMonitor { get; }
    }
}