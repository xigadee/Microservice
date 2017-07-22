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

namespace Xigadee
{
    /// <summary>
    /// This class contains the change information for the command.
    /// </summary>
    public class MasterJobStateChange
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MasterJobStateChange"/> class.
        /// </summary>
        /// <param name="oldState">The old state.</param>
        /// <param name="newState">The new state.</param>
        public MasterJobStateChange(MasterJobState oldState, MasterJobState newState)
        {
            StateOld = oldState;
            StateNew = newState;
        }
        /// <summary>
        /// The previous state.
        /// </summary>
        public MasterJobState StateOld { get; protected set; }
        /// <summary>
        /// The new master job state.
        /// </summary>
        public MasterJobState StateNew { get; protected set; }
    }
}
