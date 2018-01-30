using System;
using System.Collections;
using System.Collections.Generic;

namespace Xigadee
{
    internal class CommandWrapper: WrapperBase, IMicroserviceCommand
    {
        /// <summary>
        /// This container holds the components that do work on the system.
        /// </summary>
        private CommandContainer mCommands;

        internal CommandWrapper(CommandContainer commands, Func<ServiceStatus> getStatus) : base(getStatus)
        {
            mCommands = commands;
        }

        #region SharedServices
        /// <summary>
        /// This collection holds the shared services for the Microservice.
        /// </summary>
        public ISharedService SharedServices { get { return mCommands.SharedServices; } }
        #endregion
        //Command
        #region Register(IMessageHandler command)
        /// <summary>
        /// This method allows you to manually register a job.
        /// </summary>
        public virtual ICommand Register(ICommand command)
        {
            ValidateServiceNotStarted();

            return mCommands.Add(command);
        }
        #endregion

        /// <summary>
        /// Returns an enumerator that iterates through the command collection.
        /// </summary>
        /// <returns>
        /// An enumerator that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<ICommand> GetEnumerator()
        {
            return mCommands.Commands.GetEnumerator();
        }
        /// <summary>
        /// Returns an enumerator that iterates through the command collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return mCommands.Commands.GetEnumerator();
        }


    }
}
