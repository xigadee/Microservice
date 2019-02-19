#region using
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;
using Xigadee;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This context holds the current state of the master job process for the command.
    /// </summary>
    public class MasterJobContext
    {
        #region Events
        /// <summary>
        /// This event can is fired when the state of the master job is changed.
        /// </summary>
        public event EventHandler<MasterJobStateChangeEventArgs> OnMasterJobStateChange;
        #endregion
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MasterJobContext"/> class.
        /// </summary>
        public MasterJobContext(string name, MasterJobNegotiationStrategyBase strategy)
        {
            Name = name;
            NegotiationStrategy = strategy ?? new MasterJobNegotiationStrategy();
        }
        #endregion

        #region Name
        /// <summary>
        /// Gets the name or the master job.
        /// </summary>
        public string Name { get; }
        #endregion

        #region Partners
        /// <summary>
        /// This collection holds the list of master job standby partners.
        /// </summary>
        public ConcurrentDictionary<string, MasterJobPartner> Partners { get; } = new ConcurrentDictionary<string, MasterJobPartner>();
        #endregion
        #region PartnerSet(string originatorServiceId, bool isStandby)
        /// <summary>
        /// The method add the MasterJob Partner.
        /// </summary>
        /// <param name="originatorServiceId">The originator service identifier.</param>
        /// <param name="isStandby">if set to <c>true</c> [is standby].</param>
        public void PartnerSet(string originatorServiceId, bool isStandby)
        {
            var record = new MasterJobPartner(originatorServiceId, isStandby);

            if (isStandby)
            {
                Partners.AddOrUpdate(record.ServiceId, s => record, (s, o) => record);
            }
            else
            {
                MasterJobPartner dontCare;
                Partners.TryRemove(record.ServiceId, out dontCare);
                PartnerMaster = record;
            }
        }
        #endregion
        #region PartnerMaster
        /// <summary>
        /// Gets the current master.
        /// </summary>
        public MasterJobPartner PartnerMaster { get; private set; }
        #endregion
        #region PartnerMasterClear()
        /// <summary>
        /// Clears the masters record.
        /// </summary>
        public void PartnerMasterClear()
        {
            PartnerMaster = null;
        } 
        #endregion

        //Master Poll
        #region MasterPollAttemptsIncrement()
        /// <summary>
        /// Increments the poll attempts to the currently active server.
        /// </summary>
        public void MasterPollAttemptsIncrement()
        {
            mCurrentMasterPollAttempts++;
        } 
        #endregion
        #region MasterPollAttemptsExceeded()
        /// <summary>
        /// Checks whether the poll attempts have been exceeded.
        /// </summary>
        /// <returns>Returns true if the poll limit has been exceeded.</returns>
        public bool MasterPollAttemptsExceeded()
        {
            return NegotiationStrategy.PollAttemptsExceeded(State, mCurrentMasterPollAttempts);
        }
        #endregion
        #region MasterPollAttempts
        private int mCurrentMasterPollAttempts =0;
        /// <summary>
        /// Gets the master poll attempts.
        /// </summary>
        public int MasterPollAttempts { get { return mCurrentMasterPollAttempts; } }
        #endregion

        //State
        #region StateChangeCounter
        /// <summary>
        /// The state change counter.
        /// </summary>
        public long StateChangeCounter { get { return mStateChangeCounter; } } 
        #endregion
        #region State
        private MasterJobState mState;
        private object mLockState = new object();
        private long mStateChangeCounter = 0;
        /// <summary>
        /// This boolean property identifies whether this job is the master job for the particular 
        /// NegotiationMessageType.
        /// </summary>
        public virtual MasterJobState State
        {
            get
            {
                return mState;
            }
            set
            {
                MasterJobState? oldState;

                lock (mLockState)
                {
                    if (value != mState)
                    {
                        oldState = mState;
                        mState = value;
                        mCurrentMasterPollAttempts = 0;
                    }
                    else
                        oldState = null;
                }

                try
                {
                    if (oldState.HasValue)
                    {
                        OnMasterJobStateChange?.Invoke(this, new MasterJobStateChangeEventArgs(oldState.Value, value, Interlocked.Increment(ref mStateChangeCounter)));
                    }
                }
                catch { }
            }
        }
        #endregion

        //Negotiation
        #region NegotiationStrategy
        /// <summary>
        /// Gets the negotiation strategy.
        /// </summary>
        public MasterJobNegotiationStrategyBase NegotiationStrategy { get; }
        #endregion

        #region NegotiationPollSchedule
        /// <summary>
        /// Gets the negotiation poll schedule.
        /// </summary>
        public MasterJobNegotiationPollSchedule NegotiationPollSchedule { get; protected set; }
        #endregion
        #region NegotiationPollScheduleInitialise(Func<Schedule, CancellationToken, Task> execute)
        /// <summary>
        /// Initialises the poll schedule.
        /// </summary>
        /// <param name="execute">The execute function.</param>
        /// <returns>Returns the new schedule.</returns>
        public virtual MasterJobNegotiationPollSchedule NegotiationPollScheduleInitialise(Func<Schedule, CancellationToken, Task> execute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute", $"execute is null - {nameof(MasterJobContext)}/{nameof(NegotiationPollScheduleInitialise)}");

            //Register the schedule used for poll requests.
            return NegotiationPollSchedule = new MasterJobNegotiationPollSchedule(execute, NegotiationStrategy.InitialPoll, Name);
        }
        #endregion
        #region NegotiationPollSetNextTime()
        /// <summary>
        /// This method sets the next poll time based on the current state and the number of poll attempts.
        /// </summary>
        public void NegotiationPollSetNextTime()
        {
            NegotiationStrategy.SetNextPollTime(NegotiationPollSchedule, State, mCurrentMasterPollAttempts);
        } 
        #endregion
        #region NegotiationPollLastOut
        /// <summary>
        /// The timestamp for the last negotiation message out.
        /// </summary>
        public DateTime? NegotiationPollLastOut { get; set; }
        #endregion
        #region NegotiationPollLastIn
        /// <summary>
        /// The timestamp for the last negotiation message received.
        /// </summary>
        public DateTime? NegotiationPollLastIn { get; set; }
        #endregion

        #region Start()
        /// <summary>
        /// Starts the context.
        /// </summary>
        public virtual void Start()
        {
            Partners.Clear();
            State = MasterJobState.VerifyingComms;
        }
        #endregion
        #region Stop()
        /// <summary>
        /// Stops the context.
        /// </summary>
        public virtual void Stop()
        {
            State = MasterJobState.Disabled;
        } 
        #endregion
    }
}
