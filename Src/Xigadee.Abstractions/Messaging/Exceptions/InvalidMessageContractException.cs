using System;

namespace Xigadee
{
    /// <summary>
    /// This exception is thrown when a message contract does not resolve.
    /// </summary>
    public class InvalidMessageContractException: ContractException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidMessageContractException"/> class.
        /// </summary>
        /// <param name="contract">The contract type.</param>
        public InvalidMessageContractException(Type contract) : base(contract)
        {

        }
    }
}
