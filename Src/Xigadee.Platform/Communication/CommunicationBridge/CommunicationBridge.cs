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
using System.Threading;
using System.Threading.Tasks;
using Xigadee;

namespace Xigadee
{
    /// <summary>
    /// The communication bridge is used to abstract the work of a communication bus.
    /// This can be used for integration testing, or for actual production systems.
    /// You can switch in different technologies when needed, with the default settings based on a manual bridge.
    /// </summary>
    public class CommunicationBridge
    {
        #region Constructor
        /// <summary>
        /// This is the default constructor that specifies the broadcast mode.
        /// </summary>
        /// <param name="mode">This property specifies how the bridge communicates to the senders from the listeners.</param>
        /// <param name="agent">This is the communication agent. When not set this defaults to the manual agent.</param>
        public CommunicationBridge(CommunicationBridgeMode mode, CommunicationBridgeAgent agent = null)
        {
            Agent = agent ?? new ManualCommunicationBridgeAgent(mode);
        }
        #endregion

        #region Agent
        /// <summary>
        /// This is the underlying Agent.
        /// </summary>
        public CommunicationBridgeAgent Agent { get; }
        #endregion

        /// <summary>
        /// This is the communication mode that the bridge is working under.
        /// </summary>
        public CommunicationBridgeMode Mode { get { return Agent.Mode; } }

        /// <summary>
        /// This method returns a new listener.
        /// </summary>
        /// <returns>The listener.</returns>
        public IListener GetListener()
        {
            return Agent.GetListener();
        }

        /// <summary>
        /// This method returns a new sender.
        /// </summary>
        /// <returns>The sender.</returns>
        public ISender GetSender()
        {
            return Agent.GetSender();
        }
    }
}
