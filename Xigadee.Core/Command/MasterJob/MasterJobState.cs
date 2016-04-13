#region using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This class contains the master job messaging action types used to negotiate control.
    /// </summary>
    public static class MasterJobStates
    {
        public const string WhoIsMaster = "whoismaster";
        public const string RequestingControl1 = "requestingcontrol1";
        public const string RequestingControl2 = "requestingcontrol2";
        public const string TakingControl = "takingcontrol";

        public const string IAmMaster = "iammaster";
        public const string IAmStandby = "iamstandby";

        public const string ResyncMaster = "resyncmaster";
    }

    /// <summary>
    /// This is the current status of the job.
    /// </summary>
    public enum MasterJobState: int
    {
        Disabled = -1,

        Inactive = 0,
        VerifyingComms = 6,
        Starting = 1,
        Requesting1 = 2,
        Requesting2 = 3,
        TakingControl = 4,
        Active = 5
    }
}
