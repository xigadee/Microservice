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
#endregion
namespace Xigadee
{
    /// <summary>
    /// This is the default custom command class that allows for full customization in policy and statistics.
    /// </summary>
    /// <typeparam name="S">The statistics class type.</typeparam>
    /// <typeparam name="P">The customer command policy.</typeparam>
    /// <typeparam name="H">The command handler type.</typeparam>
    public abstract partial class CommandBase<S, P, H>: ServiceBase<S>, ICommand
        where S : CommandStatistics, new()
        where P : CommandPolicy, new()
        where H : class, ICommandHandler, new()
    {
        #region Declarations
        /// <summary>
        /// This is the command policy.
        /// </summary>
        protected readonly P mPolicy;
        /// <summary>
        /// This is the concurrent dictionary that contains the supported commands.
        /// </summary>
        protected Dictionary<CommandHolder, H> mSupported;
        /// <summary>
        /// This is the fast lookup collection for a command.
        /// </summary>
        protected ConcurrentDictionary<ServiceMessageHeader, H> mCommandCache;
        /// <summary>
        /// This event is used by the component container to discover when a command is registered or deregistered.
        /// Implement IMessageHandlerDynamic to enable this feature.
        /// </summary>
        public event EventHandler<CommandChange> OnCommandChange;

        /// <summary>
        /// This is the job timer
        /// </summary>
        private List<Schedule> mSchedules;
        /// <summary>
        /// This is the shared service collection.
        /// </summary>
        protected ISharedService mSharedServices;
        #endregion
        #region Constructor
        /// <summary>
        /// This is the default constructor that calls the CommandsRegister function.
        /// </summary>
        protected CommandBase(P policy = null)
        {
            mPolicy = PolicyCreateOrValidate(policy);

            mSupported = new Dictionary<CommandHolder, H>();
            mSchedules = new List<Schedule>();

            StartupPriority = mPolicy.StartupPriority ?? 0;

            mCommandCache = new ConcurrentDictionary<ServiceMessageHeader, H>();
        }
        #endregion

        #region PolicyCreateOrValidate(P incomingPolicy)
        /// <summary>
        /// This method ensures that a policy object exists for the command. You should override this method to set any
        /// default configuration properties.
        /// </summary>
        /// <returns>Returns the incoming policy or creates a default policy if this is not set..</returns>
        protected virtual P PolicyCreateOrValidate(P incomingPolicy)
        {
            return incomingPolicy ?? new P();
        }
        #endregion

        #region StartInternal/StopInternal
        /// <summary>
        /// This override method starts the command.
        /// </summary>
        protected override void StartInternal()
        {
            try
            {
                if (mPolicy == null)
                    throw new CommandStartupException("Command policy cannot be null");

                CommandsRegister();

                if (mPolicy.CommandReflectionSupported)
                    CommandsRegisterReflection();

                if (mPolicy.OutgoingRequestsEnabled)
                    OutgoingRequestsInitialise();

                if (mPolicy.MasterJobEnabled)
                    MasterJobTearUp();

                if (mPolicy.JobPollEnabled)
                    JobsStart();

                if (mPolicy.CommandNotify == CommandNotificationBehaviour.OnStartUp)
                    CommandsNotify();
            }
            catch (Exception ex)
            {
                Collector?.LogException($"Command '{GetType().Name}' start exception", ex);
                throw ex;
            }
        }
        /// <summary>
        /// This override method stops the command.
        /// </summary>
        protected override void StopInternal()
        {
            try
            {
                //If enabled, stop any master job processes.
                if (mPolicy.MasterJobEnabled)
                    MasterJobTearDown();

                if (mPolicy.JobPollEnabled)
                    JobsStop();

                if (mPolicy.OutgoingRequestsEnabled)
                    OutgoingRequestsStop();

                CommandsNotify(true);
            }
            catch (Exception ex)
            {
                Collector?.LogException($"Command '{GetType().Name}' stop exception", ex);
                throw;
            }
        }
        #endregion

        protected virtual void JobsStart()
        {
            JobSchedulesInitialise();

            if (mPolicy.ScheduleReflectionSupported)
                JobSchedulesReflectionInitialise();
        }

        protected virtual void JobsStop()
        {
            mSchedules.ForEach((s) => Scheduler.Unregister(s));
            mSchedules.Clear();

        }

        #region PayloadSerializer
        /// <summary>
        /// This is the requestPayload serializer used across the system.
        /// </summary>
        public IPayloadSerializationContainer PayloadSerializer
        {
            get;
            set;
        }
        #endregion

        #region JobSchedulesInitialise()
        /// <summary>
        /// This method can be overriden to enable additional schedules to be registered for the job.
        /// </summary>
        protected virtual void JobSchedulesInitialise()
        {

        }
        #endregion

        #region OriginatorId
        /// <summary>
        /// This is the service originator Id.
        /// </summary>
        public MicroserviceId OriginatorId
        {
            get;
            set;
        }
        #endregion

        #region Scheduler
        /// <summary>
        /// This is the scheduler. It is needed to process request timeouts.
        /// </summary>
        public virtual IScheduler Scheduler
        {
            get; set;
        }
        #endregion
        #region SchedulerRegister(Schedule schedule)
        /// <summary>
        /// This method registers a schedule and adds it to the collection so that it can be 
        /// deregistered later when the command stops.
        /// </summary>
        /// <param name="schedule">The schedule to register.</param>
        protected virtual void SchedulerRegister(Schedule schedule)
        {
            mSchedules.Add(Scheduler.Register(schedule));
        } 
        #endregion

        #region StartupPriority
        /// <summary>
        /// This is the message handler priority used when starting up.
        /// </summary>
        public int StartupPriority
        {
            get; set;
        }
        #endregion

        #region Collector
        /// <summary>
        /// This is the data collector reference used for managing data change and logging.
        /// </summary>
        public virtual IDataCollection Collector
        {
            get; set;
        } 
        #endregion
        #region SharedServices
        /// <summary>
        /// This is the shared service collection for commands that wish to share direct access to internal data.
        /// </summary>
        public virtual ISharedService SharedServices
        {
            get
            {
                return mSharedServices;
            }

            set
            {
                SharedServicesChange(value);
            }
        }

        /// <summary>
        /// This method is called to set or remove the shared service reference.
        /// You can override your logic to safely set the shared service collection here.
        /// </summary>
        /// <param name="sharedServices">The shared service reference or null if this is not set.</param>
        protected virtual void SharedServicesChange(ISharedService sharedServices)
        {
            mSharedServices = sharedServices;
        }
        #endregion
    }

    #region CommandBase
    /// <summary>
    /// This is the standard command base constructor.
    /// </summary>
    public abstract class CommandBase: CommandBase<CommandStatistics>
    {
        /// <summary>
        /// This is the default constructor for the command class.
        /// </summary>
        /// <param name="policy">The optional command policy. If this is null, then the policy will be created.</param>
        public CommandBase(CommandPolicy policy = null) : base(policy)
        {
        }
    }
    #endregion
    #region CommandBase<S>
    /// <summary>
    /// This is the extended command constructor that allows for custom statistics.
    /// </summary>
    /// <typeparam name="S">The statistics class type.</typeparam>
    public abstract class CommandBase<S>: CommandBase<S, CommandPolicy>
        where S : CommandStatistics, new()
    {
        /// <summary>
        /// This is the default constructor for the command class.
        /// </summary>
        /// <param name="policy">The optional command policy. If this is null, then the policy will be created.</param>
        public CommandBase(CommandPolicy policy = null) : base(policy)
        {
        }
    }
    #endregion
    #region CommandBase<S,P>
    /// <summary>
    /// This is the extended command constructor that allows for custom statistics.
    /// </summary>
    /// <typeparam name="S">The statistics class type.</typeparam>
    /// <typeparam name="P">The customer command policy.</typeparam>
    public abstract class CommandBase<S, P>: CommandBase<S, P, CommandHandler<CommandHandlerStatistics>>
        where S : CommandStatistics, new()
        where P : CommandPolicy, new()
    {
        /// <summary>
        /// This is the default constructor for the command class.
        /// </summary>
        /// <param name="policy">The optional command policy. If this is null, then the policy will be created.</param>
        public CommandBase(P policy = null) : base(policy)
        {
        }
    }
    #endregion
}


