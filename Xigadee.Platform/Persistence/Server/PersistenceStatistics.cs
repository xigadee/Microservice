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
using System.Threading;
using System.Threading.Tasks;

namespace Xigadee
{
    public class PersistenceStatistics: CommandStatistics
    {
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

        #region ErrorIncrement()
        /// <summary>
        /// This method is used to increment the active and total record count.
        /// </summary>
        public virtual void RetryIncrement()
        {
            Interlocked.Increment(ref mRetries);
        }
        #endregion

        public long Retries {get { return mRetries; } }

        private long mRetries;

        public string[] RequestsInPlay { get; set; }
    }
}
