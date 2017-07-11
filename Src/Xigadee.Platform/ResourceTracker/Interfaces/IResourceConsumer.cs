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
    /// This interface is used by a resource consumer.
    /// </summary>
    public interface IResourceConsumer: IResourceBase
    {
        /// <summary>
        /// This method is used to start the tracking of a resource.
        /// </summary>
        /// <param name="group"></param>
        /// <param name="requestId"></param>
        /// <returns>Returns the tracking identifier.</returns>
        Guid Start(string group, Guid requestId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="profileId"></param>
        /// <param name="start"></param>
        /// <param name="result"></param>
        void End(Guid profileId, int start, ResourceRequestResult result);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="profileId"></param>
        /// <param name="retryStart"></param>
        /// <param name="reason"></param>
        void Retry(Guid profileId, int retryStart, ResourceRetryReason reason);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="profileId"></param>
        /// <param name="retryStart"></param>
        void Exception(Guid profileId, int retryStart);
    }
}
