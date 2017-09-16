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
    /// This interface lists the policy options for the Microservice.
    /// </summary>
    public interface IMicroservicePolicy
    {
        /// <summary>
        /// Gets the Microservice default policy.
        /// </summary>
        MicroservicePolicy Microservice { get; }
        /// <summary>
        /// Gets the task manager policy.
        /// </summary>
        /// <value>
        TaskManagerPolicy TaskManager { get; }
        /// <summary>
        /// Gets the resource monitor policy.
        /// </summary>
        ResourceContainerPolicy ResourceMonitor { get; }
        /// <summary>
        /// Gets the command container policy.
        /// </summary>
        CommandContainerPolicy CommandContainer { get; }
        /// <summary>
        /// Gets the communication policy.
        /// </summary>
        CommunicationPolicy Communication { get; }
        /// <summary>
        /// Gets the scheduler policy.
        /// </summary>
        SchedulerPolicy Scheduler { get; }
        /// <summary>
        /// Gets the security policy.
        /// </summary>
        SecurityContainerPolicy Security { get; }
        /// <summary>
        /// Gets the data collection policy.
        /// </summary>
        DataCollectionPolicy DataCollection { get; }
        /// <summary>
        /// Gets the serialization policy.
        /// </summary>
        SerializationPolicy Serialization { get; }
    }
}
