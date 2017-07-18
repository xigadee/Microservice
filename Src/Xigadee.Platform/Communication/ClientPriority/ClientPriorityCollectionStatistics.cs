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
using System.Collections.Generic;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This is the statistics collection that holds the active clients.
    /// </summary>
    public class ClientPriorityCollectionStatistics: StatusBase
    {
        /// <summary>
        /// The override name that ensures this is at the top of the JSON.
        /// </summary>
        public override string Name
        {
            get
            {
                return base.Name;
            }

            set
            {
                base.Name = value;
            }
        }
        /// <summary>
        /// The current polling algorithm.
        /// </summary>
        public string Algorithm { get; set; }
        /// <summary>
        /// The client priority list.
        /// </summary>
        public List<ClientPriorityHolderStatistics> Clients { get; set; }
    }
}
