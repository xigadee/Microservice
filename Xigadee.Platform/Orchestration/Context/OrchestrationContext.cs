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
    /// The orchestration context is used to hold the process state as it passes through the orchestration flow.
    /// </summary>
    public class OrchestrationContext
    {
        /// <summary>
        /// This is the unique Id for the context.
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();
        /// <summary>
        /// This is the version Id for the context.
        /// </summary>
        public Guid VersionId { get; set; } = Guid.NewGuid();
        /// <summary>
        /// This is the flow Id that the context is based on.
        /// </summary>
        public Guid OrchestrationId { get; set; } 

        /// <summary>
        /// This is the date when the context was created.
        /// </summary>
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        /// <summary>
        /// This is the last update for the context.
        /// </summary>
        public DateTime? DateUpdated { get; set; }
    }
}
