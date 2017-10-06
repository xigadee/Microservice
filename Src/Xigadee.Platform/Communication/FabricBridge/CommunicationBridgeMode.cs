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
    /// This enumeration specifies the mode used for the bridge.
    /// </summary>
    public enum FabricMode
    {
        /// <summary>
        /// In round robin mode a response message is only sent to a single recipient.
        /// </summary>
        Queue,
        /// <summary>
        /// In broadcast mode a response message is sent to all recipients.
        /// </summary>
        Broadcast,
        /// <summary>
        /// The mode is not used by the agent.
        /// </summary>
        NotSet
    }
}
