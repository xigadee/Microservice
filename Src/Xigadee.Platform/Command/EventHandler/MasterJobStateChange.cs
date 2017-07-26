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
using System.Diagnostics;

namespace Xigadee
{
    public class MasterJobEventArgsBase: EventArgs
    {
        public MasterJobEventArgsBase(string serviceName, string commandName, int? iteration = null)
        {
            ServiceName = serviceName;
            CommandName = commandName;
            Iteration = iteration;
        }

        public int? Iteration { get; }
        /// <summary>
        /// This is the service name.
        /// </summary>
        public string ServiceName { get; }

        public string CommandName { get; }

        public DateTime TimeStamp { get; } = DateTime.UtcNow;
    }
    /// <summary>
    /// This class contains the change information for the command.
    /// </summary>
    [DebuggerDisplay("{Debug()}")]
    public class MasterJobStateChangeEventArgs: MasterJobEventArgsBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MasterJobStateChangeEventArgs"/> class.
        /// </summary>
        /// <param name="oldState">The old state.</param>
        /// <param name="newState">The new state.</param>
        public MasterJobStateChangeEventArgs(string serviceName, string commandName, MasterJobState oldState, MasterJobState newState, int? iteration = null):base(serviceName, commandName, iteration)
        {
            StateOld = oldState;
            StateNew = newState;
        }
        /// <summary>
        /// The previous state.
        /// </summary>
        public MasterJobState StateOld { get; }
        /// <summary>
        /// The new master job state.
        /// </summary>
        public MasterJobState StateNew { get; }

        /// <summary>
        /// Shows a debug string for the event.
        /// </summary>
        public string Debug()
        {
            return $"State Change - {ServiceName}/{CommandName}: {StateOld} > {StateNew} @ {TimeStamp} - {Iteration}\r\n";
        }
    }

    public enum MasterJobCommunicationDirection
    {
        Incoming,
        Outgoing
    }
    /// <summary>
    /// This class contains the change information for the command.
    /// </summary>
    [DebuggerDisplay("{Debug()}")]
    public class MasterJobCommunicationEventArgs: MasterJobEventArgsBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MasterJobStateChangeEventArgs"/> class.
        /// </summary>
        /// <param name="state">The old state.</param>
        /// <param name="newState">The new state.</param>
        public MasterJobCommunicationEventArgs(string serviceName, string commandName
            , MasterJobCommunicationDirection direction
            , MasterJobState state, string action, int iteration, string originatorId = null)
            :base(serviceName, commandName, iteration)
        {
            State = state;
            Action = action;
            Direction = direction;
            OriginatorId = originatorId;
        }

        public string OriginatorId { get; }

        public MasterJobCommunicationDirection Direction { get; }
        /// <summary>
        /// The previous state.
        /// </summary>
        public MasterJobState State { get; }
        /// <summary>
        /// The new master job state.
        /// </summary>
        public string Action { get; }

        /// <summary>
        /// Shows a debug string for the event.
        /// </summary>
        public string Debug()
        {
            return $"Message {Direction} - {ServiceName}/{CommandName}: {State} --> {Action} @ {TimeStamp} - {Iteration} [{OriginatorId??ServiceName}]\r\n";
        }
    }
}
