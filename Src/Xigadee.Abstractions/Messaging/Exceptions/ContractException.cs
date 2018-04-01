using System;

namespace Xigadee
{
    /// <summary>
    /// This is the base contract exception.
    /// </summary>
    public abstract class ContractException: Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContractException"/> class.
        /// </summary>
        /// <param name="contract">The contract.</param>
        protected ContractException(Type contract)
        {
            ContractName = contract.Name;
        }
        /// <summary>
        /// Gets the name of the contract.
        /// </summary>
        public string ContractName { get; }
    }
}
