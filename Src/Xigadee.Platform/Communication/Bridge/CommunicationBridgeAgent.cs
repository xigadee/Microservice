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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This is the base abstract class used to implement different communication technologies.
    /// </summary>
    public abstract class CommunicationBridgeAgent
    {
        /// <summary>
        /// This is the communiciation bridge mode.
        /// </summary>
        protected CommunicationBridgeMode mMode = CommunicationBridgeMode.RoundRobin;

        /// <summary>
        /// This method sets the operating mode. Override this if you wish to restict the modes that are supported.
        /// </summary>
        /// <param name="mode">The communication bridge mode.</param>
        public virtual void SetMode(CommunicationBridgeMode mode)
        {
            mMode = mode;
        }

        /// <summary>
        /// This method returns a new listener.
        /// </summary>
        /// <returns>The listener.</returns>
        public abstract IListener GetListener();

        /// <summary>
        /// This method returns a new sender.
        /// </summary>
        /// <returns>The sender.</returns>
        public abstract ISender GetSender();

    }
}
