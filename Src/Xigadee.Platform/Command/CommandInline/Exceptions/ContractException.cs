using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{

    /// <summary>
    /// This exception occurs when the channel information is not passed to the AddCommand extension.
    /// This can occur if you pass a ServiceMessageHeaderFragment, without passing the channel reference.
    /// </summary>
    public class ChannelIncomingMissingException: Exception
    {
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public ChannelIncomingMissingException() 
        {
        }
    }

    /// <summary>
    /// This exception occurs when the channel specified in the contract does not match the channel being attached to.
    /// </summary>
    public class InvalidPipelineChannelContractException : ContractException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidPipelineChannelContractException"/> class.
        /// </summary>
        /// <param name="contract">The contract type.</param>
        /// <param name="actualChannelId">The actual channel identifier.</param>
        /// <param name="expectedChannelId">The expected channel identifier.</param>
        public InvalidPipelineChannelContractException(Type contract, string actualChannelId, string expectedChannelId) : base(contract)
        {
            ActualChannelId = actualChannelId;
            ExpectedChannelId = expectedChannelId;
        }
        /// <summary>
        /// Gets the actual channel identifier.
        /// </summary>
        public string ActualChannelId { get; }
        /// <summary>
        /// Gets the expected channel identifier.
        /// </summary>
        public string ExpectedChannelId { get; }
    }
}
