namespace Xigadee
{
    /// <summary>
    /// This interface is for components that require payload serialization and deserialization.
    /// </summary>
    public interface IRequirePayloadSerialization
    {
        /// <summary>
        /// This is the system wide Payload serializer.
        /// </summary>
        IPayloadSerializationContainer PayloadSerializer { get; set; }
    }
}
