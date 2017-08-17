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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
#endregion
namespace Xigadee
{
    public abstract partial class CommandBase<S, P, H>
    {
        #region Events
        /// <summary>
        /// This event is used to signal a change of registered message types for the command.
        /// </summary>
        public event EventHandler<ScheduleChangeEventArgs> OnScheduleChange; 
        #endregion
        #region Declarations
        /// <summary>
        /// This is the job timer
        /// </summary>
        private List<CommandJobSchedule> mSchedules;
        #endregion

        #region *--> JobsTearUp
        /// <summary>
        /// This method extracts any job schedules from the command and registers each command.
        /// </summary>
        protected virtual void JobsTearUp()
        {
            mSchedules = new List<CommandJobSchedule>();

            if (mPolicy.ScheduleReflectionSupported)
                JobSchedulesReflectionInitialise<JobScheduleAttribute>();

            JobSchedulesManualRegister();
        }
        #endregion
        #region JobSchedulesReflectionInitialise()
        /// <summary>
        /// This method can be overridden to enable additional schedules to be registered for the job.
        /// </summary>
        protected virtual void JobSchedulesReflectionInitialise<A>() where A: JobScheduleAttributeBase
        {
            foreach (var holder in this.CommandMethodSignatures<A, CommandScheduleSignature>(true))
            {
                JobScheduleRegister(holder.Signature.Action
                    , holder.Attribute.ToTimerConfig()
                    , name: holder.Attribute.Name
                    , isLongRunning: holder.Attribute.IsLongRunningProcess
                    , isMasterJob: holder.Attribute.IsMasterJob
                    );
            }
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

        #region *--> JobsTearDown()
        /// <summary>
        /// This method stops and registered job schedules.
        /// </summary>
        protected virtual void JobsTearDown()
        {
            mSchedules.ForEach((s) => JobScheduleUnregister(s));
            mSchedules.Clear();
        }
        #endregion

        #region JobScheduleRegister(...)   
        /// <summary>
        /// Creates and registers a schedule.
        /// </summary>
        /// <param name="execute">The execution function.</param>
        /// <param name="interval">The poll interval.</param>
        /// <param name="initialWait">The initial wait.</param>
        /// <param name="initialWaitUTCTime">The optional initial UTC wait time that the polling will begin if the initialWait is set to null.</param>
        /// <param name="context">The optional schedule context</param>
        /// <param name="name">The name of the schedule.</param>
        /// <param name="isLongRunning">A boolean flag that specifies whether the process is long running.</param>
        /// <param name="tearUp">The set up action.</param>
        /// <param name="tearDown">The clear down action.</param>
        /// <param name="isMasterJob">Indicates whether this schedule is associated to a master job.</param>
        /// <returns>Returns the new schedule.</returns>
        protected virtual CommandJobSchedule JobScheduleRegister(Func<Schedule, CancellationToken, Task> execute
            , TimeSpan? interval = null
            , TimeSpan? initialWait = null
            , DateTime? initialWaitUTCTime = null
            , object context = null
            , string name = null
            , bool isLongRunning = false
            , Action<Schedule> tearUp = null
            , Action<Schedule> tearDown = null
            , bool isMasterJob = false)
        {
            var schedule = new CommandJobSchedule();

            schedule.Initialise(execute, new ScheduleTimerConfig(interval, initialWait, initialWaitUTCTime, false), context, name, isLongRunning, tearUp, tearDown, isMasterJob);

            return JobScheduleRegister(schedule);
        }
        /// <summary>
        /// Creates and registers a schedule.
        /// </summary>
        /// <param name="execute">The execution function.</param>
        /// <param name="timerConfig">The timer poll configuration. If this is set to null, the schedule will fire immediately after it is registered.</param>
        /// <param name="context">The optional schedule context</param>
        /// <param name="name">The name of the schedule.</param>
        /// <param name="isLongRunning">A boolean flag that specifies whether the process is long running.</param>
        /// <param name="tearUp">The set up action.</param>
        /// <param name="tearDown">The clear down action.</param>
        /// <param name="isMasterJob">Indicates whether this schedule is associated to a master job.</param>
        /// <returns>Returns the new schedule.</returns>
        protected virtual CommandJobSchedule JobScheduleRegister(Func<Schedule, CancellationToken, Task> execute
            , ScheduleTimerConfig timerConfig = null
            , object context = null
            , string name = null
            , bool isLongRunning = false
            , Action<Schedule> tearUp = null
            , Action<Schedule> tearDown = null
            , bool isMasterJob = false)
        {
            var schedule = new CommandJobSchedule();

            schedule.Initialise(execute, timerConfig, context, name, isLongRunning, tearUp, tearDown, isMasterJob);

            return JobScheduleRegister(schedule);
        }
        /// <summary>
        /// Jobs the schedule register.
        /// </summary>
        /// <param name="schedule">The schedule.</param>
        /// <returns>Returns the schedule.</returns>
        protected virtual CommandJobSchedule JobScheduleRegister(CommandJobSchedule schedule)
        {
            try
            {
                schedule.TearUp?.Invoke(schedule);
            }
            catch (Exception ex)
            {
                StatisticsInternal.Ex = ex;
                Collector?.LogException($"Job '{schedule.Name} could not be teared up.'", ex);
            }

            //Set the identifiers for debug.
            schedule.CommandId = ComponentId;
            schedule.CommandName = FriendlyName;

            Scheduler.Register(schedule);
            mSchedules.Add(schedule);
            FireAndDecorateEventArgs(OnScheduleChange, () => new ScheduleChangeEventArgs(false, schedule));

            return schedule;
        }
        #endregion
        #region JobScheduleUnregister(CommandJobSchedule schedule)
        /// <summary>
        /// Jobs the schedule unregister.
        /// </summary>
        /// <param name="schedule">The schedule.</param>
        /// <returns></returns>
        protected virtual bool JobScheduleUnregister(CommandJobSchedule schedule)
        {
            try
            {
                schedule.TearDown?.Invoke(schedule);
            }
            catch (Exception ex)
            {
                StatisticsInternal.Ex = ex;
                Collector?.LogException($"Job '{schedule.Name} could not be teared down.'", ex);
            }

            var success = Scheduler.Unregister(schedule);

            if (success)
                FireAndDecorateEventArgs(OnScheduleChange, () => new ScheduleChangeEventArgs(true, schedule));

            return success;
        } 
        #endregion
    }
}
