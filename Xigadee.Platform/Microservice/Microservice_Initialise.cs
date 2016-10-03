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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
#endregion
namespace Xigadee
{
    //Initialise
    public partial class Microservice
    {
        #region Declarations
        object syncLock = new object();

        MicroservicePolicy mPolicyMicroservice = null;
        TaskManagerPolicy mPolicyTaskManager = null;
        CommandContainerPolicy mPolicyCommand = null;
        CommunicationPolicy mPolicyCommunication = null;
        SchedulerPolicy mPolicyScheduler = null;
        SecurityPolicy mPolicySecurity = null;
        ResourceTrackerPolicy mPolicyResourceTracker = null;
        DataCollectionPolicy mPolicyDataCollection = null; 
        #endregion

        #region PolicyResolve<P>(P existing, Action<Microservice, P> onResolve = null)
        /// <summary>
        /// This is the helper class used to pull out the policy container from the incoming collection and to set it within the correct settings.
        /// </summary>
        /// <typeparam name="P">The policy type.</typeparam>
        /// <param name="existing">The existing value. If this is not null the method will bypass the setup.</param>
        /// <param name="onResolve">An action that can be called to adjust the policy settings when it is first resolved.</param>
        /// <returns>Returns the policy.</returns>
        protected P PolicyResolve<P>(P existing, Action<Microservice, P> onResolve = null)
            where P : PolicyBase, new()
        {
            if (existing == null)
            {
                lock (syncLock)
                {
                    if (existing == null)
                    {
                        existing = mPolicySettings?.Where((p) => p is P).Cast<P>().FirstOrDefault() ?? new P();
                        onResolve?.Invoke(this, existing);
                    }
                }
            }

            return existing;
        } 
        #endregion

        #region PolicyMicroservice
        /// <summary>
        /// This is the policy used to set the Microservice default settings.
        /// </summary>
        /// <returns>The microservice policy.</returns>
        public virtual MicroservicePolicy PolicyMicroservice
        {
            get
            {
                return PolicyResolve(mPolicyMicroservice, (m, p) => mPolicyMicroservice = p);
            }
        } 
        #endregion

        #region InitialiseTaskManager()
        /// <summary>
        /// This method creates the task manager and sets the default bulkhead reservations.
        /// </summary>
        /// <returns></returns>
        protected virtual TaskManager InitialiseTaskManager()
        {
            var taskTracker = new TaskManager(4, Execute, PolicyTaskManager);

            taskTracker.BulkheadReserve(3, 1, 2);
            taskTracker.BulkheadReserve(2, 2, 2);
            taskTracker.BulkheadReserve(1, 8, 8);
            taskTracker.BulkheadReserve(0, 0);

            return taskTracker;
        }
        #endregion
        #region PolicyTaskManager
        /// <summary>
        /// This method retrieves the policy for the task manager.
        /// </summary>
        /// <returns>The policy.</returns>
        public virtual TaskManagerPolicy PolicyTaskManager
        {
            get
            {
                return PolicyResolve(mPolicyTaskManager, (m, p) => mPolicyTaskManager = p);
            }
        } 
        #endregion

        #region InitialiseResourceTracker()
        /// <summary>
        /// This method creates the default resource tracker for the Microservice.
        /// Resource trackers are used to limit incoming messages that use a particular resource
        /// that is overloaded.
        /// </summary>
        /// <returns>Returns the resource tracker.</returns>
        protected virtual ResourceTracker InitialiseResourceTracker()
        {
            var container = new ResourceTracker(PolicyResourceTracker);

            return container;
        }
        #endregion
        #region PolicyResourceTracker
        /// <summary>
        /// This is the policy for the resource tracker.
        /// </summary>
        /// <returns></returns>
        public virtual ResourceTrackerPolicy PolicyResourceTracker
        {
            get
            {
                return PolicyResolve(mPolicyResourceTracker, (m, p) => mPolicyResourceTracker = p);
            }
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
            var container = new CommandContainer(PolicyCommandContainer);

            return container;
        }
        #endregion
        #region PolicyCommandContainer
        /// <summary>
        /// This is the policy used to set the command container.
        /// </summary>
        /// <returns></returns>
        public virtual CommandContainerPolicy PolicyCommandContainer
        {
            get
            {
                return PolicyResolve(mPolicyCommand, (m, p) => mPolicyCommand = p);
            }
        } 
        #endregion

        #region InitialiseCommunicationContainer()
        /// <summary>
        /// This method creates the communication container. This container comtains all the 
        /// listeners and senders registered on the service, and assigns priority when polling for 
        /// new incoming requests.
        /// </summary>
        /// <returns>The communication container.</returns>
        protected virtual CommunicationContainer InitialiseCommunicationContainer()
        {
            var container = new CommunicationContainer(PolicyCommunication);

            return container;
        }
        #endregion
        #region PolicyCommunication
        /// <summary>
        /// This is the policy used to set the communication component settings.
        /// </summary>
        /// <returns></returns>
        public virtual CommunicationPolicy PolicyCommunication
        {
            get
            {
                return PolicyResolve(mPolicyCommunication, (m, p) => mPolicyCommunication = p);
            }
        } 
        #endregion

        #region InitialiseSchedulerContainer()
        /// <summary>
        /// This method returns the default scheduler container.
        /// </summary>
        /// <returns>The default scheduler.</returns>
        protected virtual SchedulerContainer InitialiseSchedulerContainer()
        {
            var container = new SchedulerContainer(PolicyScheduler);

            return container;
        }
        #endregion
        #region PolicyScheduler
        /// <summary>
        /// This is the policy for the scheduler.
        /// </summary>
        public virtual SchedulerPolicy PolicyScheduler
        {
            get
            {
                return PolicyResolve(mPolicyScheduler, (m, p) => mPolicyScheduler = p);
            }
        } 
        #endregion

        #region InitialiseSecurityContainer()
        /// <summary>
        /// This method creates the component container.
        /// This container holds the jobs, message initiators and handlers and is used to 
        /// assign incoming requests to the appropriate command.
        /// </summary>
        /// <returns>Returns the container.</returns>
        protected virtual SecurityContainer InitialiseSecurityContainer()
        {
            var container = new SecurityContainer(PolicySecurity);

            return container;
        }
        #endregion
        #region PolicySecurity
        /// <summary>
        /// This is the policy used to set the securty container settings.
        /// </summary>
        /// <returns></returns>
        public virtual SecurityPolicy PolicySecurity
        {
            get
            {
                return PolicyResolve(mPolicySecurity, (m, p) => mPolicySecurity = p);
            }
        } 
        #endregion

        #region InitialiseSerializationContainer(List<IPayloadSerializer> payloadSerializers)
        /// <summary>
        /// THis method returns the default scheduler container.
        /// </summary>
        protected virtual SerializationContainer InitialiseSerializationContainer(IEnumerable<IPayloadSerializer> payloadSerializers)
        {
            var container = new SerializationContainer(payloadSerializers);

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
            var container = new DataCollectionContainer(PolicyDataCollection);

            return container;
        }
        #endregion
        #region PolicyDataCollection
        /// <summary>
        /// This is the policy used to set the data collection container settings.
        /// </summary>
        /// <returns>The policy</returns>
        public virtual DataCollectionPolicy PolicyDataCollection
        {
            get
            {
                return PolicyResolve(mPolicyDataCollection, (m, p) => mPolicyDataCollection = p);
            }
        } 
        #endregion
    }
}
