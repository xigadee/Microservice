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
    //Initialize
    public partial class Microservice
    {
        #region CoreEngineInitialize()
        /// <summary>
        /// This method initializes the process loop components.
        /// </summary>
        protected virtual void CoreEngineInitialize()
        {
            mScheduler = InitialiseSchedulerContainer();

            mTaskManager = InitialiseTaskManager();
        }
        #endregion

        #region InitialiseTaskManager()
        /// <summary>
        /// This method creates the task manager and sets the default bulkhead reservations.
        /// </summary>
        /// <returns></returns>
        protected virtual TaskManager InitialiseTaskManager()
        {
            var policy = Policies.TaskManager;

            var taskTracker = new TaskManager(Execute, policy);

            return taskTracker;
        }
        #endregion
        #region InitialiseResourceMonitor()
        /// <summary>
        /// This method creates the default resource tracker for the Microservice.
        /// Resource trackers are used to limit incoming messages that use a particular resource
        /// that is overloaded.
        /// </summary>
        /// <returns>Returns the resource tracker.</returns>
        protected virtual ResourceContainer InitialiseResourceMonitor()
        {
            var container = new ResourceContainer(Policies.ResourceMonitor);

            return container;
        }
        #endregion
        #region InitialiseCommandContainer()
        /// <summary>
        /// This method creates the component container.
        /// This container holds the jobs, message initiators and handlers and is used to 
        /// assign incoming requests to the appropriate command.
        /// </summary>
        /// <returns>Returns the container.</returns>
        protected virtual CommandContainer InitialiseCommandContainer()
        {
            var container = new CommandContainer(Policies.CommandContainer);

            return container;
        }
        #endregion
        #region InitialiseCommunicationContainer()
        /// <summary>
        /// This method creates the communication container. This container contains all the 
        /// listeners and senders registered on the service, and assigns priority when polling for 
        /// new incoming requests.
        /// </summary>
        /// <returns>The communication container.</returns>
        protected virtual CommunicationContainer InitialiseCommunicationContainer()
        {
            var container = new CommunicationContainer(Policies.Communication);

            return container;
        }
        #endregion
        #region InitialiseSchedulerContainer()
        /// <summary>
        /// This method returns the default scheduler container.
        /// </summary>
        /// <returns>The default scheduler.</returns>
        protected virtual SchedulerContainer InitialiseSchedulerContainer()
        {
            var container = new SchedulerContainer(Policies.Scheduler);

            return container;
        }
        #endregion

        #region InitialiseServiceHandlerContainer()
        /// <summary>
        /// THis method returns the default scheduler container.
        /// </summary>
        protected virtual ServiceHandlerContainer InitialiseServiceHandlerContainer()
        {
            var container = new ServiceHandlerContainer(Policies.ServiceHandlers);

            return container;
        }
        #endregion

        #region InitialiseDataCollectionContainer()
        /// <summary>
        /// This method creates the data collection container, which is responsible for logging, event source management, and telemetry.
        /// </summary>
        /// <returns>The data collection container.</returns>
        protected virtual DataCollectionContainer InitialiseDataCollectionContainer()
        {
            var container = new DataCollectionContainer(Policies.DataCollection);

            return container;
        }
        #endregion
    }
}
