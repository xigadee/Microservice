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

using System.Collections.Generic;

namespace Xigadee
{
    /// <summary>
    /// This holds the current master job statistics.
    /// </summary>
    public class MasterJobStatistics
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="MasterJobStatistics"/> is enabled.
        /// </summary>
        public bool Enabled { get; set; }
        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// Gets or sets the server.
        /// </summary>
        public MasterJobPartner Master { get; set; }
        /// <summary>
        /// Gets or sets the standbys.
        /// </summary>
        public List<MasterJobPartner> Standbys { get; set; }
    }
}
