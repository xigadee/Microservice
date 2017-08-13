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
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
#endregion
namespace Xigadee
{
    public abstract partial class CommandBase<S, P, H>
    {
        #region Declarations
        /// <summary>
        /// This is the job timer
        /// </summary>
        private List<Schedule> mSchedules;
        #endregion
        #region JobsTearUp
        /// <summary>
        /// This method extracts any job schedules from the command and registers each command.
        /// </summary>
        protected virtual void JobsTearUp()
        {
            if (mPolicy.ScheduleReflectionSupported)
                JobSchedulesReflectionInitialise();

            JobSchedulesManualRegister();
        }
        #endregion
        #region JobSchedulesReflectionInitialise()
        /// <summary>
        /// This method can be overriden to enable additional schedules to be registered for the job.
        /// </summary>
        protected virtual void JobSchedulesReflectionInitialise()
        {
            this.ScheduleMethodAttributeSignatures<JobScheduleAttribute>()
                .SelectMany((s) => s.Item2.ToSchedules())
                .ForEach((r) => Scheduler.Register(r));
        }
        #endregion
        #region JobSchedulesManualRegister()
        /// <summary>
        /// This method can be overriden to enable additional schedules to be registered for the job.
        /// </summary>
        protected virtual void JobSchedulesManualRegister()
        {
        }
        #endregion

        #region JobsTearDown()
        /// <summary>
        /// This method stops and registered job schedules.
        /// </summary>
        protected virtual void JobsTearDown()
        {
            mSchedules.ForEach((s) => Scheduler.Unregister(s));
            mSchedules.Clear();
        } 
        #endregion



        protected virtual CommandJobSchedule JobScheduleRegister()
        {
            return null;
        }


    }
}
