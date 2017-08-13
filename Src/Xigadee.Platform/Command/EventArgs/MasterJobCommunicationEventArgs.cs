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

using System.Diagnostics;

namespace Xigadee
{
    /// <summary>
    /// This class contains the change information for the command.
    /// </summary>
    [DebuggerDisplay("{Debug()}")]
    public class MasterJobCommunicationEventArgs: MasterJobEventArgsBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MasterJobCommunicationEventArgs"/> class.
        /// </summary>
        /// <param name="direction">The direction.</param>
        /// <param name="state">The state.</param>
        /// <param name="action">The action.</param>
        /// <param name="iteration">The current iteration.</param>
        /// <param name="originatorId">The originator identifier.</param>
        public MasterJobCommunicationEventArgs(MasterJobCommunicationDirection direction
            , MasterJobState state, string action, long iteration, string originatorId = null)
            :base(iteration)
        {
            State = state;
            Action = action;
            Direction = direction;
            OriginatorId = originatorId;
        }
        /// <summary>
        /// Gets the message originator identifier.
        /// </summary>
        public string OriginatorId { get; }
        /// <summary>
        /// Gets the direction.
        /// </summary>
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
