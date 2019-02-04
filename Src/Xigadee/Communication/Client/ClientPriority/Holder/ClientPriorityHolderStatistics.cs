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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This class holds for the statistics for the client in the priority stack.
    /// </summary>
    /// <seealso cref="Xigadee.StatusBase" />
    public class ClientPriorityHolderStatistics: StatusBase
    {
        /// <summary>
        /// Gets or sets the Client identifier.
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        public Guid ClientId { get; set; }
        /// <summary>
        /// This is the service name
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
        /// Gets or sets the current ordinal poll position.
        /// </summary>
        public int Ordinal { get; set; }
        /// <summary>
        /// Gets or sets the algorithm name.
        /// </summary>
        public string Algorithm { get; set; }
        /// <summary>
        /// Gets or sets the mapping channel.
        /// </summary>
        public string MappingChannel { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is currently reserved.
        /// </summary>
        public bool IsReserved { get; set; }
        /// <summary>
        /// Gets or sets the current reserved slots or null if not reserved..
        /// </summary>
        public int? Reserved { get; set; }
        /// <summary>
        /// Gets or sets the client priority holder metrics.
        /// </summary>
        public ClientPriorityHolderMetricsStatistics Metrics { get; set; }
        /// <summary>
        /// Gets or sets the last exception.
        /// </summary>
        public Exception LastException { get; set; }
        /// <summary>
        /// Gets or sets the last exception time.
        /// </summary>
        public DateTime? LastExceptionTime { get; set; }
    }
}
