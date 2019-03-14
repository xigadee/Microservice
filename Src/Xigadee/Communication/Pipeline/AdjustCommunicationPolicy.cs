using System;

namespace Xigadee
{
    public static partial class CorePipelineExtensions
    {
        /// <summary>
        /// This extension method changes the default Microservice communication listener policy to balance load between multiple listener clients.
        /// This is the default algorithm.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <returns>Returns the pipeline</returns>
        public static P AdjustCommunicationPolicyForMultipleListenerClients<P>(this P pipeline) where P : IPipeline
        {
            return pipeline.AdjustCommunicationPolicyPollAlgorthm(new MultipleClientPollSlotAllocationAlgorithm());
        }

        /// <summary>
        /// This extension method changes the default Microservice communication listener policy to support a single client.
        /// This is useful for debugging code as it is much faster than having to juggle multiple client priority.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <returns>Returns the pipeline</returns>
        public static P AdjustCommunicationPolicyForSingleListenerClient<P>(this P pipeline) where P : IPipeline
        {
            return pipeline.AdjustCommunicationPolicyPollAlgorthm(new SingleClientPollSlotAllocationAlgorithm());
        }

        /// <summary>
        /// This extension method changes the default Microservice communication listener poll algorithm.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="algorithm">The algorithm to set.</param>
        /// <returns>Returns the pipeline</returns>
        public static P AdjustCommunicationPolicyPollAlgorthm<P>(this P pipeline, IListenerClientPollAlgorithm algorithm) where P : IPipeline
        {
            if (algorithm == null)
                throw new ArgumentNullException("algorithm");

            return pipeline.AdjustPolicyCommunication((p, c) => p.ListenerClientPollAlgorithm = algorithm);
        }
    }
}
