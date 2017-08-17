using System.Reflection;

namespace Xigadee
{
    /// <summary>
    /// This is a shared abstract class to hold method invocation shared logic.
    /// </summary>
    public abstract class CommandSignatureBase
    {
        /// <summary>
        /// Initialises the specified command.
        /// </summary>
        /// <param name="command">The command to process.</param>
        /// <param name="method">The method for the signature.</param>
        /// <param name="throwSignatureException">This property specifies whether the system should throw exceptions if a signature is invalid.</param>
        /// <exception cref="CommandContractSignatureException">Thrown if throwSignatureException is set to true and validate fails.</exception>
        public virtual void Initialise(ICommand command, MethodInfo method, bool throwSignatureException = false)
        {
            if (method == null)
                throw new CommandContractSignatureException($"Constructor error - info cannot be null.");

            if (command == null)
                throw new CommandContractSignatureException($"Constructor error - command cannot be null.");

            Command = command;

            Method = method;

            IsValid = Validate(throwSignatureException);
        }

        /// <summary>
        /// This is the command connected to the signature
        /// </summary>
        public ICommand Command { get; protected set;}
        /// <summary>
        /// This is the specific method.
        /// </summary>
        public MethodInfo Method { get; protected set; }
        /// <summary>
        /// This property specifies whether the signature is valid.
        /// </summary>
        public bool IsValid { get; protected set; }

        /// <summary>
        /// This method validates the method.
        /// </summary>
        /// <param name="throwException">Set this to true throw an exception if the signature does not match,</param>
        /// <returns>Returns true if the signature is validated.</returns>
        protected abstract bool Validate(bool throwException = false);
    }
}
