namespace Xigadee
{
    /// <summary>
    /// This class contains the master job messaging action types used to negotiate control.
    /// </summary>
    public static class MasterJobStates
    {
        /// <summary>
        /// The who-is-master message type
        /// </summary>
        public const string WhoIsMaster = "whoismaster";
        /// <summary>
        /// The requesting-control1 message type
        /// </summary>
        public const string RequestingControl1 = "requestingcontrol1";
        /// <summary>
        /// The requesting-control2 message type
        /// </summary>
        public const string RequestingControl2 = "requestingcontrol2";
        /// <summary>
        /// The taking-control message type
        /// </summary>
        public const string TakingControl = "takingcontrol";
        /// <summary>
        /// The i-am-master message type
        /// </summary >
        public const string IAmMaster = "iammaster";
        /// <summary>
        /// The i-am-standby message type
        /// </summary>
        public const string IAmStandby = "iamstandby";
        /// <summary>
        /// The resync-master message type
        /// </summary>
        public const string ResyncMaster = "resyncmaster";
    }

    /// <summary>
    /// This is the current status of the job.
    /// </summary>
    public enum MasterJobState: int
    {
        /// <summary>
        /// The master job is disabled.
        /// </summary>
        Disabled = -1,
        /// <summary>
        /// The inactive state
        /// </summary>
        Inactive = 0,
        /// <summary>
        /// The verifying comms state
        /// </summary>
        VerifyingComms = 1,
        /// <summary>
        /// The starting state
        /// </summary>
        Starting = 2,
        /// <summary>
        /// The requesting1 state
        /// </summary>
        Requesting1 = 3,
        /// <summary>
        /// The requesting2 state
        /// </summary>
        Requesting2 = 4,
        /// <summary>
        /// The taking control state
        /// </summary>
        TakingControl = 5,
        /// <summary>
        /// The active state
        /// </summary>
        Active = 10
    }
}
