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
        /// This event is called if there is an exception during transmission.
        /// </summary>
        public event EventHandler<CommunicationBridgeAgentEventArgs> OnException;
        /// <summary>
        /// This event is fired before a cloned payload is sent to a remote listener
        /// </summary>
        public event EventHandler<CommunicationBridgeAgentEventArgs> OnTransmit;
        /// <summary>
        /// This event is fired when a message is received from a sender and before it is resolved.
        /// </summary>
        public event EventHandler<CommunicationBridgeAgentEventArgs> OnReceive;

        /// <summary>
        /// This is the default constructor. 
        /// </summary>
        /// <param name="mode">The operational mode.</param>
        protected CommunicationBridgeAgent(CommunicationBridgeMode mode = CommunicationBridgeMode.NotUsed)
        {
            Mode = mode;
        }
        /// <summary>
        /// This method is used to fire the event when an exception occurs.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="payload">The payload.</param>
        /// <param name="ex">The exception raised.</param>
        protected void OnExceptionInvoke(object sender, TransmissionPayload payload, Exception ex)
        {
            var e = new CommunicationBridgeAgentEventArgs(payload, ex);
            OnException?.Invoke(sender, e);
        }
        /// <summary>
        /// This event is thrown when a payload is transmitted.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="payload">The payload.</param>
        protected void OnTransmitInvoke(object sender, TransmissionPayload payload)
        {
            var e = new CommunicationBridgeAgentEventArgs(payload);
            OnTransmit?.Invoke(sender, e);
        }
        /// <summary>
        /// This event is raised when a payload is received by a listener.
        /// </summary>
        /// <param name="sender">The listener.</param>
        /// <param name="payload">The payload.</param>
        protected void OnReceiveInvoke(object sender, TransmissionPayload payload)
        {
            var e = new CommunicationBridgeAgentEventArgs(payload);
            OnReceive?.Invoke(sender, e);
        }

        /// <summary>
        /// This is the communication bridge mode.
        /// </summary>
        public CommunicationBridgeMode Mode { get; }

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
