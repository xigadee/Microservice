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
    /// This class contains the base properties for the orchestration components.
    /// </summary>
    public abstract class OrchestrationFlowComponentBase
    {
        /// <summary>
        /// This is teh uniqueidentifier for the component
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// This property specifies whether the component is active.
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// This is the timestamp that the component was created
        /// </summary>
        public DateTime? DateCreated { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// This is the timestamp that the component was updated.
        /// </summary>
        public DateTime? DateUpdated { get; set; }

        /// <summary>
        /// This is the friendly name for the component.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// This is the optional friendly description of the component.
        /// </summary>
        public string Description { get; set; }
    }
}
