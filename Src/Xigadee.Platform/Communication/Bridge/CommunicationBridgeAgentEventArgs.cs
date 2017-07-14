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
    /// This class is used to hold temporary payload and exception information.
    /// </summary>
    public class CommunicationBridgeAgentEventArgs: EventArgs
    {
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="Payload">The payload.</param>
        /// <param name="Ex">The optional exception.</param>
        public CommunicationBridgeAgentEventArgs(TransmissionPayload Payload, Exception Ex = null)
        {
            this.Payload = Payload;
            this.Ex = Ex;
        }
        /// <summary>
        /// The payload passing through the agent.
        /// </summary>
        public TransmissionPayload Payload { get; }
        /// <summary>
        /// The optional exception. 
        /// </summary>
        public Exception Ex { get; }
    }
}
