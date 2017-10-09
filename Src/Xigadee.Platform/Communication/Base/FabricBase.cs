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

#region using
#endregion
namespace Xigadee
{
    /// <summary>
    /// This class is used to form the fabric used to communicate between Microservices.
    /// </summary>
    public abstract class FabricBridgeBase<B>
        where B: ICommunicationBridge
    {
        /// <summary>
        /// Gets the <see cref="ICommunicationBridge"/> for the specified mode.
        /// </summary>
        /// <value>
        /// The <see cref="ICommunicationBridge"/>.
        /// </value>
        /// <param name="mode">The communication mode.</param>
        /// <returns>A bridge for the specific communication mode.</returns>
        public abstract B this[FabricMode mode] { get; }

        /// <summary>
        /// Gets the queue agent.
        /// </summary>
        public virtual B Queue => this[FabricMode.Queue];
        /// <summary>
        /// Gets the broadcast agent.
        /// </summary>
        public virtual B Broadcast => this[FabricMode.Broadcast];
    }
}
