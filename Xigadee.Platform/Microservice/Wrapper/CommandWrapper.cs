#region Copyright
// Copyright Hitachi Consulting
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    internal class CommandWrapper: WrapperBase, IMicroserviceCommand
    {
        /// <summary>
        /// This container holds the components that do work on the system.
        /// </summary>
        private CommandContainer mCommands;

        public CommandWrapper(CommandContainer commands, Func<ServiceStatus> getStatus) : base(getStatus)
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

        public IEnumerator<ICommand> GetEnumerator()
        {
            return mCommands.Commands.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return mCommands.Commands.GetEnumerator();
        }


    }
}
