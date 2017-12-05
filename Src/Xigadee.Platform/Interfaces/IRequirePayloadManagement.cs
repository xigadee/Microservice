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
    }
}
