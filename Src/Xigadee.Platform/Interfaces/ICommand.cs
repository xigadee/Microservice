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
    /// This interface includes the events supported by a command.
    /// </summary>
    public interface ICommandEvents
    {
        /// <summary>
        /// This event is fired when an request is received and resolved.
        /// </summary>
        event EventHandler<TransmissionPayload> OnRequest;
        /// <summary>
        /// This event is fired when a request is received but not resolved.
        /// </summary>
        event EventHandler<TransmissionPayload> OnRequestUnresolved;
        /// <summary>
        /// This event is fired when an outgoing request is initiated.
        /// </summary>
        event EventHandler<OutgoingRequest> OnOutgoingRequest;
        /// <summary>
        /// This event is fired when an outgoing request times out.
        /// </summary>
        event EventHandler<OutgoingRequest> OnOutgoingRequestTimeout;
        /// <summary>
        /// This event is fired when an outgoing request completes
        /// </summary>
        event EventHandler<OutgoingRequest> OnOutgoingRequestComplete;
    }
    /// <summary>
    /// This is the root interface implemented by the command class.
    /// </summary>
    public interface ICommand: ICommandMasterJob, ICommandEvents, IService, IRequireScheduler, IRequirePayloadManagement
        , IRequireServiceOriginator, IRequireSharedServices, IRequireDataCollector
    {
        /// <summary>
        /// This is the default listening channel id for incoming requests.
        /// </summary>
        string ChannelId { get; set; }
        /// <summary>
        /// Specifies whether the channel can be autoset during configuration
        /// </summary>
        bool ChannelIdAutoSet { get; }
        /// <summary>
        /// This is the channel used for the response to outgoing messages.
        /// </summary>
        string ResponseChannelId { get; set; }
        /// <summary>
        /// Specifies whether the response channel can be set during configuration.
        /// </summary>
        bool ResponseChannelIdAutoSet { get; }

        /// <summary>
        /// This is the reference to the task manager that can be called to submit new jobs for processing.
        /// </summary>
        Action<ICommand, string, TransmissionPayload> TaskManager { get; set; }
        /// <summary>
        /// This method is called for processes that support direct notification from the Task Manager that a process has been
        /// cancelled.
        /// </summary>
        /// <param name="originatorKey">The is the originator tracking key.</param>
        void TimeoutTaskManager(string originatorKey);

        /// <summary>
        /// This method should return true if the handler support this specific message.
        /// </summary>
        /// <param name="messageHeader">The message header.</param>
        /// <returns>Returns true if the message channelId is supported.</returns>
        bool SupportsMessage(ServiceMessageHeader messageHeader);
        /// <summary>
        /// This method processes the supported message.
        /// </summary>
        /// <param name="request">The request payload.</param>
        /// <param name="responseMessages">The response collection.</param>
        Task ProcessRequest(TransmissionPayload request, List<TransmissionPayload> responseMessages);
        /// <summary>
        /// Returns a list of message header types.
        /// </summary>
        /// <returns>Returns ServiceMessageHeader definition.</returns>
        List<MessageFilterWrapper> SupportedMessageTypes();
        /// <summary>
        /// This is the handler priority used when starting and stopping services.
        /// The higher the value, the sooner the command will be started relative to the other commands with lower priorities.
        /// </summary>
        int StartupPriority { get; set; }
        /// <summary>
        /// This event is used to signal a change of registered message types for the command.
        /// </summary>
        event EventHandler<CommandChange> OnCommandChange;
        /// <summary>
        /// This event is fired when the command's masterjob state is changed.
        /// </summary>
        event EventHandler<MasterJobStateChangeEventArgs> OnMasterJobStateChange;
    }

    /// <summary>
    /// This interface holds the masterjob configuration properties.
    /// </summary>
    public interface ICommandMasterJob
    {
        /// <summary>
        /// Gets or sets the master job negotiation channel priority.
        /// </summary>
        int MasterJobNegotiationChannelPriority { get; set; }
        /// <summary>
        /// This is the channel type used to handle the messaging handshake.
        /// </summary>
        string MasterJobNegotiationChannelType { get; set; }
        /// <summary>
        /// This is the channel used to negotiate control for a master job.
        /// </summary>
        string MasterJobNegotiationChannelIdIncoming { get; set; }
        /// <summary>
        /// This is the channel used to negotiate control for a master job.
        /// </summary>
        string MasterJobNegotiationChannelIdOutgoing { get; set; }
        /// <summary>
        /// Specifies whether the master job negotiation channel can be set during configuration.
        /// </summary>
        bool MasterJobNegotiationChannelIdAutoSet { get; }
    }
}
