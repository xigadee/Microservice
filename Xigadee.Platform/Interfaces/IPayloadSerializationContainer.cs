using System;
namespace Xigadee
{
    /// <summary>
    /// This interface is used to expose the serialization container to applications 
    /// that require access to it.
    /// </summary>
    public interface IPayloadSerializationContainer
    {
        object PayloadDeserialize(ServiceMessage message);

        P PayloadDeserialize<P>(TransmissionPayload payload);

        byte[] PayloadSerialize(object payload);
    }

    /// <summary>
    /// This interface is for components that require payload serialization and deserialization.
    /// </summary>
    public interface IPayloadSerializerConsumer
    {
        IPayloadSerializationContainer PayloadSerializer { get; set; }
    }
}
