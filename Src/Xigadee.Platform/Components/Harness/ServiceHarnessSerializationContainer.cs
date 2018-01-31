namespace Xigadee
{
    /// <summary>
    /// This is the stub serialization container.
    /// </summary>
    public class ServiceHarnessSerializationContainer: ServiceHandlerContainer
    {
        /// <summary>
        /// This override adds the Json serializer by default.
        /// </summary>
        protected override void StartInternal()
        {
            Serialization.Add(new JsonRawSerializer());

            Compression.Add(new PayloadCompressorDeflate());
            Compression.Add(new PayloadCompressorGzip());

            base.StartInternal();
        }
    }
}
