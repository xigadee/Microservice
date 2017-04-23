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
    internal class PolicyWrapper: WrapperBase, IMicroservicePolicy
    {
        #region Declarations
        object syncLock = new object();

        MicroservicePolicy mPolicyMicroservice = null;
        TaskManagerPolicy mPolicyTaskManager = null;
        CommandContainerPolicy mPolicyCommand = null;
        CommunicationPolicy mPolicyCommunication = null;
        SchedulerPolicy mPolicyScheduler = null;
        SecurityContainerPolicy mPolicySecurity = null;
        ResourceContainerPolicy mPolicyResourceTracker = null;
        DataCollectionPolicy mPolicyDataCollection = null;
        SerializationPolicy mPolicySerialization = null;

        /// <summary>
        /// This is the collection of policy settings for the Microservice.
        /// </summary>
        List<PolicyBase> mPolicySettings;
        #endregion

        internal PolicyWrapper(IEnumerable<PolicyBase> policySettings, Func<ServiceStatus> getStatus) : base(getStatus)
        {
            mPolicySettings = policySettings?.ToList() ?? new List<PolicyBase>();

        }

        #region PolicyResolve<P>(P existing, Action<Microservice, P> onResolve = null)
        /// <summary>
        /// This is the helper class used to pull out the policy container from the incoming collection and to set it within the correct settings.
        /// </summary>
        /// <typeparam name="P">The policy type.</typeparam>
        /// <param name="existing">The existing value. If this is not null the method will bypass the setup.</param>
        /// <param name="onResolve">An action that can be called to adjust the policy settings when it is first resolved.</param>
        /// <returns>Returns the policy.</returns>
        protected P PolicyResolve<P>(P existing, Action<P> onResolve = null)
            where P : PolicyBase, new()
        {
            if (existing == null)
            {
                lock (syncLock)
                {
                    if (existing == null)
                    {
                        existing = mPolicySettings?.Where((p) => p is P).Cast<P>().FirstOrDefault() ?? new P();
                        onResolve?.Invoke(existing);
                    }
                }
            }

            return existing;
        }
        #endregion

        #region Microservice
        /// <summary>
        /// This is the policy used to set the Microservice default settings.
        /// </summary>
        /// <returns>The microservice policy.</returns>
        public virtual MicroservicePolicy Microservice
        {
            get
            {
                return PolicyResolve(mPolicyMicroservice, (p) => mPolicyMicroservice = p);
            }
        }
        #endregion
        #region TaskManager
        /// <summary>
        /// This method retrieves the policy for the task manager.
        /// </summary>
        /// <returns>The policy.</returns>
        public virtual TaskManagerPolicy TaskManager
        {
            get
            {
                return PolicyResolve(mPolicyTaskManager, (p) => mPolicyTaskManager = p);
            }
        }
        #endregion
        #region ResourceTracker
        /// <summary>
        /// This is the policy for the resource tracker.
        /// </summary>
        /// <returns></returns>
        public virtual ResourceContainerPolicy ResourceTracker
        {
            get
            {
                return PolicyResolve(mPolicyResourceTracker, (p) => mPolicyResourceTracker = p);
            }
        }
        #endregion
        #region CommandContainer
        /// <summary>
        /// This is the policy used to set the command container.
        /// </summary>
        /// <returns></returns>
        public virtual CommandContainerPolicy CommandContainer
        {
            get
            {
                return PolicyResolve(mPolicyCommand, (p) => mPolicyCommand = p);
            }
        }
        #endregion
        #region Communication
        /// <summary>
        /// This is the policy used to set the communication component settings.
        /// </summary>
        /// <returns></returns>
        public virtual CommunicationPolicy Communication
        {
            get
            {
                return PolicyResolve(mPolicyCommunication, (p) => mPolicyCommunication = p);
            }
        }
        #endregion
        #region Scheduler
        /// <summary>
        /// This is the policy for the scheduler.
        /// </summary>
        public virtual SchedulerPolicy Scheduler
        {
            get
            {
                return PolicyResolve(mPolicyScheduler, (p) => mPolicyScheduler = p);
            }
        }
        #endregion
        #region Security
        /// <summary>
        /// This is the policy used to set the securty container settings.
        /// </summary>
        /// <returns></returns>
        public virtual SecurityContainerPolicy Security
        {
            get
            {
                return PolicyResolve(mPolicySecurity, (p) => mPolicySecurity = p);
            }
        }
        #endregion
        #region DataCollection
        /// <summary>
        /// This is the policy used to set the data collection container settings.
        /// </summary>
        /// <returns>The policy</returns>
        public virtual DataCollectionPolicy DataCollection
        {
            get
            {
                return PolicyResolve(mPolicyDataCollection, (p) => mPolicyDataCollection = p);
            }
        }
        #endregion
        #region Serialization
        /// <summary>
        /// This is the policy used to set the data collection container settings.
        /// </summary>
        /// <returns>The policy</returns>
        public virtual SerializationPolicy Serialization
        {
            get
            {
                return PolicyResolve(mPolicySerialization, (p) => mPolicySerialization = p);
            }
        }
        #endregion
    }
}
