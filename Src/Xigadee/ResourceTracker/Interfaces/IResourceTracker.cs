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

namespace Xigadee
{
    /// <summary>
    /// This interface is registered in the Shared Service container, and allows services that require resource tracking to interact.
    /// </summary>
    public interface IResourceTracker
    {
        /// <summary>
        /// This method is used to register a resource consumer.
        /// </summary>
        /// <param name="name">The reference name.</param>
        /// <param name="profile">The profile for the resource.</param>
        /// <returns>Returns a resource consumer object.</returns>
        IResourceConsumer RegisterConsumer(string name, ResourceProfile profile);
        /// <summary>
        /// This method is used to register a resource rate limiter.
        /// </summary>
        /// <param name="name">The reference name.</param>
        /// <param name="profiles">The profiles that we wish to track and combine together.</param>
        /// <returns>Returns the rate limiter.</returns>
        IResourceRequestRateLimiter RegisterRequestRateLimiter(string name, IEnumerable<ResourceProfile> profiles);
        /// <summary>
        /// This method is used to register a resource rate limiter.
        /// </summary>
        /// <param name="name">The reference name.</param>
        /// <param name="profiles">The profiles that we wish to track and combine together.</param>
        /// <returns>Returns the rate limiter.</returns>
        IResourceRequestRateLimiter RegisterRequestRateLimiter(string name, params ResourceProfile[] profiles);
        /// <summary>
        /// This method retrieves the current status for the Resource profile.
        /// </summary>
        /// <param name="name">The name of the resource.</param>
        /// <returns>The current status of the resource.</returns>
        ResourceStatus ResourceStatusGet(string name);
    }
}
