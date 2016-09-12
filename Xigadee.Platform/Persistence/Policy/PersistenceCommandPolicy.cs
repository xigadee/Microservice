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
    /// This is the default command policy for the persistence class.
    /// </summary>
    public class PersistenceCommandPolicy:CommandPolicy
    {

        /// <summary>
        /// Persistence managers should not be master jobs by default.
        /// </summary>
        public override bool MasterJobEnabled { get { return false; } }
        /// <summary>
        /// This is the default time allowed when making a call to the underlying persistence layer.
        /// </summary>
        public TimeSpan? DefaultTimeout { get; set; }
        /// <summary>
        /// The resource consumer 
        /// </summary>
        public IResourceConsumer ResourceConsumer { get; set; }
        /// <summary>
        /// Specifies the persistence retry policy
        /// </summary>
        public PersistenceRetryPolicy PersistenceRetryPolicy { get; set; }
        /// <summary>
        /// This is the resoure profile for the persistence manager.
        /// </summary>
        public ResourceProfile ResourceProfile { get; set; }


    }
}
