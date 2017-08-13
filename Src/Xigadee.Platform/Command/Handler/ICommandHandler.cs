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
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This interface is used for command handlers to implement, to allow the mapping of incoming requests to their specific commands.
    /// </summary>
    public interface ICommandHandler
    {
        /// <summary>
        /// Initialises the specified handler.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="action">The action.</param>
        /// <param name="exceptionAction">The exception action.</param>
        /// <param name="referenceId">The reference identifier.</param>
        /// <param name="isMasterJob">Specifies whether the command is owned by a master job.</param>
        void Initialise(MessageFilterWrapper key
            , Func<TransmissionPayload, List<TransmissionPayload>, Task> action
            , Func<Exception, TransmissionPayload, List<TransmissionPayload>, Task> exceptionAction = null
            , string referenceId = null
            , bool isMasterJob = false
            );
        /// <summary>
        /// Gets the key used to match the request to the appropriate command.
        /// </summary>
        MessageFilterWrapper Key { get; }
        /// <summary>
        /// Gets the action used for executing the command.
        /// </summary>
        Func<TransmissionPayload, List<TransmissionPayload>, Task> Action { get; }
        /// <summary>
        /// Gets the exception action used if an uncaught exception in thrown during execution of the command.
        /// </summary>
        Func<Exception, TransmissionPayload, List<TransmissionPayload>, Task> ExceptionAction { get; }
        /// <summary>
        /// Executes the specified request.
        /// </summary>
        /// <param name="rq">The incoming request.</param>
        /// <param name="rs">The outgoing response collection.</param>
        Task Execute(TransmissionPayload rq, List<TransmissionPayload> rs);
        /// <summary>
        /// Gets the handler statistics.
        /// </summary>
        ICommandHandlerStatistics HandlerStatistics { get; }
        /// <summary>
        /// Gets the reference identifier.
        /// </summary>
        string ReferenceId { get; }
        /// <summary>
        /// Specifies whether this command is a master job command.
        /// </summary>
        bool IsMasterJob { get; }
    }
}