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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#endregion
namespace Xigadee
{
    /// <summary>
    /// This class is used to define a schedule for the SchedulerContainer.
    /// </summary>
    [DebuggerDisplay("Type={ScheduleType} Name={Name} Active={Active} {Message}")]
    public class Schedule: MessagingStatistics, ISchedule
    {
        #region Declarations
        private DateTime? mNextPoll;
        private DateTime? mLastPoll;
        private bool mShouldPoll;
        private Func<Schedule, CancellationToken, Task> mExecute;
        private long mPollCount = 0;
        private long mPollActiveSkip = 0;
        #endregion
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="execute">The async schedule function.</param>
        /// <param name="name">The schedule name.</param>
        public Schedule(Func<Schedule, CancellationToken, Task> execute, string name = null)
            : base()
        {
            mExecute = execute;
            mShouldPoll = true;
            Name = name;
        }
        #endregion

        #region ScheduleType
        /// <summary>
        /// Gets the type of the schedule. By default, this is based on the class name, but it can be overridden.
        /// </summary>
        public virtual string ScheduleType { get { return GetType().Name; } } 
        #endregion
        #region Name
        /// <summary>
        /// This is the override for the name to bring it to the top .
        /// </summary>
        public override string Name
        {
            get
            {
                return base.Name;
            }
            set
            {
                base.Name = value;
            }
        }
        #endregion

        #region Id
        /// <summary>
        /// This is the time Id.
        /// </summary>
        public Guid Id { get; } = Guid.NewGuid();
        #endregion

        #region Active
        /// <summary>
        /// Indicates whether the schedule is currently executing.
        /// </summary>
        public bool Active { get; set; }
        #endregion

        #region ExecutionCount
        /// <summary>
        /// This is the number of times the schedule has been executed.
        /// </summary>
        public long ExecutionCount
        {
            get
            {
                return mPollCount;
            }
        }
        #endregion

        #region WillRunIn
        /// <summary>
        /// This is a debug property to identify when the schedule will next run.
        /// </summary>
        public string WillRunIn
        {
            get
            {
                if (Active || !NextPollTime.HasValue)
                    return null;
                var span = NextPollTime.Value - DateTime.UtcNow;

                return StatsCounter.LargeTime(span);
            }
        }
        #endregion

        #region PollSkip()
        /// <summary>
        /// This method is called when the job is active but the ShouldPoll condition has been met.
        /// </summary>
        public void PollSkip()
        {
            Interlocked.Increment(ref mPollActiveSkip);
        }
        #endregion
        #region PollActiveSkip
        /// <summary>
        /// This is the count of the number of time the schedule was meant to be polled but 
        /// was still active.
        /// </summary>
        public long PollActiveSkip { get { return mPollActiveSkip; } }
        #endregion

        #region NextPollTime
        /// <summary>
        /// This is the next poll time in UTC.
        /// </summary>
        public DateTime? NextPollTime
        {
            get
            {
                return mNextPoll;
            }
        }
        #endregion
        #region LastPollTime
        /// <summary>
        /// This is the next poll time in UTC.
        /// </summary>
        public DateTime? LastPollTime
        {
            get
            {
                return mLastPoll;
            }
        }
        #endregion

        #region InitialWait
        /// <summary>
        /// This is the initial wait in a TimeSpan before the timer event fires.
        /// </summary>
        public TimeSpan? InitialWait { get; set; }
        #endregion
        #region InitialTime
        /// <summary>
        /// This is the specific time that the event should initially fire.
        /// </summary>
        public DateTime? InitialTime { get; set; }
        #endregion
        #region Frequency
        /// <summary>
        /// This is the repeat frequency that the event should fire.
        /// </summary>
        public TimeSpan? Frequency { get; set; }
        #endregion

        #region ExecuteException
        /// <summary>
        /// Gets the last execution exception.
        /// </summary>
        public ScheduleException ExecuteException { get; set; }
        #endregion

        #region Start()
        /// <summary>
        /// Resets the schedule for the start of execution.
        /// </summary>
        public virtual void Start()
        {
            ExecuteException = null;
            Active = true;
        }
        #endregion

        #region Context
        /// <summary>
        /// Gets or sets the context which can be used to store data between scheduled calls.
        /// </summary>
        public object Context { get; set; }
        #endregion

        #region Complete...
        /// <summary>
        /// Completes the specified poll.
        /// </summary>
        /// <param name="success">Specifies whether the poll was a success.</param>
        /// <param name="recalculate">Specifies whether to recalcualte the next poll.</param>
        /// <param name="isException">Specifices whether the poll failed..</param>
        /// <param name="lastEx">The last exception.</param>
        /// <param name="exceptionTime">The exception time.</param>
        public virtual void Complete(bool success
            , bool recalculate = true
            , bool isException = false
            , Exception lastEx = null
            , DateTime? exceptionTime = null)
        {
            if (!success)
                ErrorIncrement();

            if (isException)
                ExecuteException = new ScheduleException() { Ex = lastEx, LastTime = exceptionTime };

            if (recalculate)
                Recalculate();

            Active = false;
        } 
        #endregion

        #region Recalculate(bool force = false)
        /// <summary>
        /// This method recalculates the next time the event should fire.
        /// </summary>
        /// <param name="force">This parameter forces the schedule to recalculate even if the next poll time is in the future.</param>
        public virtual void Recalculate(bool force = false)
        {
            //Check whether we have a time set already, if so quit.
            if (!force && NextPollTime.HasValue && NextPollTime.Value > DateTime.UtcNow)
                return;

            if (InitialTime.HasValue)
            {
                mNextPoll = InitialTime.Value;
                InitialWait = null;
                InitialTime = null;
                return;
            }

            if (InitialWait.HasValue)
            {
                mNextPoll = DateTime.UtcNow.Add(InitialWait.Value);
                InitialWait = null;
                InitialTime = null;
                return;
            }

            if (Frequency.HasValue)
            {
                mNextPoll = DateTime.UtcNow.Add(Frequency.Value);
                return;
            }

            mNextPoll = null;
        }
        #endregion
        #region ShouldPoll
        /// <summary>
        /// This boolean property calculates whether the scheduler should fire.
        /// You can set this to false to disable the timer event.
        /// </summary>
        public bool ShouldPoll
        {
            get
            {
                var compareTime = DateTime.UtcNow;

                bool result = !Active
                    && mShouldPoll
                    && mNextPoll.HasValue
                    && (mNextPoll.Value <= compareTime);

                return result;
            }
            set
            {
                mShouldPoll = value;
                if (value)
                    Recalculate();
            }
        }
        #endregion

        #region Execute()
        /// <summary>
        /// This method executes the schedule.
        /// </summary>
        public async Task Execute(CancellationToken token)
        {
            int start = ActiveIncrement();

            try
            {
                mLastPoll = DateTime.UtcNow;
                Interlocked.Increment(ref mPollCount);
                await mExecute(this, token);
            }
            finally
            {
                ActiveDecrement(start);
            }
        }
        #endregion

        #region State
        /// <summary>
        /// This generic object can be used to hold state.
        /// </summary>
        public object State { get; set; }
        #endregion

        #region IsInternal
        /// <summary>
        /// Identifies whether the schedule is internal and should skip any process queueing.
        /// </summary>
        public bool IsInternal { get; set; }
        #endregion
        #region IsLongRunning
        /// <summary>
        /// A helper method that lets the task scheduler that the process might be long running.
        /// </summary>
        public bool IsLongRunning { get; set; }
        #endregion

        #region Message
        /// <summary>
        /// This is the debug message.
        /// </summary>
        public override string Message
        {
            get
            {
                return $"{ScheduleType}:{Name ?? Id.ToString("N").ToUpperInvariant()} [ShouldPoll={mShouldPoll}] @ {NextPollTime} Hits = {mPollCount}";
            }
            set
            {
                base.Message = value;
            }
        }
        #endregion

        /// <summary>
        /// This internal exception is used to store exception information.
        /// </summary>
        public class ScheduleException
        {
            /// <summary>
            /// The thrown exception.
            /// </summary>
            public virtual Exception Ex { get; set; }

            #region IsLastException
            /// <summary>
            /// Identifies whether the last execute caused an unhandled exception.
            /// </summary>
            public bool IsLastException { get; set; }
            #endregion
            #region LastTime
            /// <summary>
            /// This is the last exception time.
            /// </summary>
            public DateTime? LastTime { get; set; } = DateTime.UtcNow;
            #endregion
        }
    }

}
