using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This is the base contract exception.
    /// </summary>
    public abstract class ContractException : Exception
    {
        protected ContractException(Type contract)
        {
            ContractName = contract.Name;
        }

        public string ContractName { get; }
    }

    /// <summary>
    /// This exception is thrown when a message contract does not resolve.
    /// </summary>
    public class InvalidMessageContractException : ContractException
    {
        public InvalidMessageContractException(Type contract):base(contract)
        {

        }
    }

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
        public InvalidPipelineChannelContractException(Type contract, string actualChannelId, string expectedChannelId) : base(contract)
        {
            ActualChannelId = actualChannelId;
            ExpectedChannelId = expectedChannelId;
        }

        public string ActualChannelId { get; }

        public string ExpectedChannelId { get; }
    }
}
