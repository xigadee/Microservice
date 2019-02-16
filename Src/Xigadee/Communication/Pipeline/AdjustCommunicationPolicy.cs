namespace Xigadee
{
    public static partial class CorePipelineExtensions
    {
        /// <summary>
        /// This extension method changes the default Microservice communication listener policy to balance load between multiple listener clients.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <returns>Returns the pipeline</returns>
        public static P AdjustCommunicationPolicyForMultipleListenerClients<P>(this P pipeline) where P : IPipeline
        {
            return pipeline.AdjustPolicyCommunication((p, c) => p.ListenerClientPollAlgorithm = new MultipleClientPollSlotAllocationAlgorithm());
        }

        /// <summary>
        /// This extension method changes the default Microservice communication listener policy to support a single client.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <returns>Returns the pipeline</returns>
        public static P AdjustCommunicationPolicyForSingleListenerClient<P>(this P pipeline) where P : IPipeline
        {
            return pipeline.AdjustPolicyCommunication((p, c) => p.ListenerClientPollAlgorithm = new SingleClientPollSlotAllocationAlgorithm());
        }
    }
}
