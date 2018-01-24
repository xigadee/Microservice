namespace Xigadee
{
    /// <summary>
    /// This interface defines the Microservice definition.
    /// </summary>
    public interface IMicroservice: IService
    {
        MicroserviceId Id { get; }

        ServiceStatus Status { get; }

        IMicroserviceEvents Events { get; }

        IMicroserviceSecurity Security { get; }

        IMicroserviceCommunication Communication { get; }

        IMicroserviceDispatch Dispatch { get; }

        IMicroserviceCommand Commands { get; }

        IMicroservicePolicy Policies { get; }

        IMicroserviceDataCollection DataCollection { get; }

        IMicroserviceSerialization Serialization { get; }

        IMicroserviceResourceMonitor ResourceMonitor { get; }
    }
}