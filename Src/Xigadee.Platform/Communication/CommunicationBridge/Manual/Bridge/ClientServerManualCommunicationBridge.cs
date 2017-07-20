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

namespace Xigadee
{
    /// <summary>
    /// This class is a shortcut to create a simple client/server communication bridge.
    /// </summary>
    public class ClientServerManualCommunicationBridge
    {
        CommunicationBridge mToServer;
        CommunicationBridge mToClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientServerManualCommunicationBridge"/> class.
        /// </summary>
        public ClientServerManualCommunicationBridge()
        {
            mToServer = new CommunicationBridge(CommunicationBridgeMode.RoundRobin);
            mToClient = new CommunicationBridge(CommunicationBridgeMode.Broadcast);
        }

        ///// <summary>
        ///// This method returns a new listener.
        ///// </summary>
        ///// <returns>The listener.</returns>
        //public IListener ServerGetListener()
        //{
        //    return Agent.GetListener();
        //}

        ///// <summary>
        ///// This method returns a new sender.
        ///// </summary>
        ///// <returns>The sender.</returns>
        //public ISender GetSender()
        //{
        //    return Agent.GetSender();
        //}

    }
}
