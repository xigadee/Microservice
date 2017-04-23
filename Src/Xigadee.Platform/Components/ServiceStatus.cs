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

namespace Xigadee
{
    /// <summary>
    /// The is the job status enumeration.
    /// </summary>
    public enum ServiceStatus
    {
        /// <summary>
        /// The job is created.
        /// </summary>
        Created,
        /// <summary>
        /// The job has experienced an error during index-up
        /// </summary>
        Faulted,

        /// <summary>
        /// The job is starting.
        /// </summary>
        Starting,
        /// <summary>
        /// The job is starting.
        /// </summary>

        Running,
        /// <summary>
        /// The job is running.
        /// </summary>
        Stopped,
        /// <summary>
        /// The job is stopped.
        /// </summary>
        Stopping,
        /// <summary>
        /// The job is stopping.
        /// </summary>
        Pausing,
        /// <summary>
        /// The job is pausing. This is not currently supported.
        /// </summary>
        Paused,
        /// <summary>
        /// The job is resuming. This is not currently supported.
        /// </summary>
        Resuming
    }
}
