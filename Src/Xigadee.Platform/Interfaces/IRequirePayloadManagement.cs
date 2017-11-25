namespace Xigadee
{
    /// <summary>
    /// This interface is for components that require payload serialization and deserialization.
    /// </summary>
    public interface IRequirePayloadManagement
    {
        /// <summary>
        /// This is the system wide Payload serializer.
        /// </summary>
        IPayloadSerializationContainer PayloadSerializer { get; set; }

        //IObjectRegistry Registry {get;set;}
    }

    /// <summary>
    /// This interface can be used to pass messages between Commands within the same Microservice container without the overhead of serialization.
    /// Objects are registered through this container and then pass using the reference Ids generated.
    /// </summary>
    public interface IObjectRegistry
    {

    }
}
