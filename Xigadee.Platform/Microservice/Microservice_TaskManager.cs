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
    //TaskManager
    public partial class Microservice
    {
        #region Declarations
        /// <summary>
        /// This class contains the running tasks and provides a breakdown of the current availability for new tasks.
        /// </summary>
        private TaskManager mTaskManager;
        /// <summary>
        /// This is the scheduler container.
        /// </summary>
        private SchedulerContainer mScheduler;
        #endregion

        #region TaskManagerInitialise()
        /// <summary>
        /// This method initialises the process loop components.
        /// </summary>
        protected virtual void TaskManagerInitialise()
        {
            mScheduler = InitialiseSchedulerContainer();

            mTaskManager = InitialiseTaskManager();
        }
        #endregion
        #region TaskManagerStart()
        /// <summary>
        /// This method starts the processing process loop.
        /// </summary>
        protected virtual void TaskManagerStart()
        {
            TaskManagerRegisterProcesses();

            ServiceStart(mTaskManager);

            ServiceStart(mScheduler);
        }
        #endregion
        #region TaskManagerStop()
        /// <summary>
        /// This method stops the process loop.
        /// </summary>
        protected virtual void TaskManagerStop()
        {
            ServiceStop(mScheduler);

            ServiceStop(mTaskManager);
        }
        #endregion

        #region TaskManagerProcessRegister()
        /// <summary>
        /// 
        /// </summary>
        protected virtual void TaskManagerRegisterProcesses()
        {
            mTaskManager.ProcessRegister("SchedulesProcess"
                , 5, mScheduler);

            mTaskManager.ProcessRegister("ListenersProcess"
                , 4, mCommunication);

            mTaskManager.ProcessRegister("Overload Check"
                , 3, mDataCollection);

        }
        #endregion
    }
}
