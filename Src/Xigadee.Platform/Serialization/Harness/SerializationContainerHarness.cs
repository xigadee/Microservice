namespace Xigadee
{
    /// <summary>
    /// This class contains the dependencies for the serialization harness.
    /// </summary>
    /// <seealso cref="Xigadee.ServiceHarnessDependencies" />
    public class SerializationHarnessDependencies: ServiceHarnessDependencies
    {
        /// <summary>
        /// This is set to null as we are the serializer.
        /// </summary>
        public override ServiceHarnessSerializationContainer PayloadSerializer => null;
    }

    /// <summary>
    /// This harness is used for unit testing the serialization container.
    /// </summary>
    public class SerializationContainerHarness: ServiceHarness<SerializationContainer, SerializationHarnessDependencies>
    {
    }
}
