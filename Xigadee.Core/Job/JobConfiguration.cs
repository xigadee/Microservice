#region using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This class provides a configuration class for the Job/MasterJob
    /// </summary>
    public class JobConfiguration
    {
        public TimeSpan? Interval { get; set; }

        public TimeSpan? InitialWait { get; set; }

        public DateTime? InitialTime { get; set; }

        public bool IsMasterJob { get; set; }

        public bool HasTimerPoll { get; set; }

        public bool IsLongRunningJob { get; set; }

        public int MasterJobNegotiationChannelPriority { get; set; }

        public string MasterJobNegotiationChannelId { get; set; }

        public string MasterJobNegotiationChannelType { get; set; }

        public string MasterJobName { get; set; }

        public static JobConfiguration ToJob(TimeSpan? interval, TimeSpan? initialWait, DateTime? initialTime, bool isLongRunningJob = false)
        {
            return new JobConfiguration { HasTimerPoll = true, InitialTime = initialTime, InitialWait = initialWait, Interval = interval, IsMasterJob= false, IsLongRunningJob = isLongRunningJob };
        }

        public static JobConfiguration ToMasterJob(string negotiationChannelId, string negotiationChannelType=null, int negotiationChannelPriority = 1, string name = null)
        {
            return new JobConfiguration() {
                  InitialWait = TimeSpan.FromSeconds(2)
                , HasTimerPoll = true
                , Interval = TimeSpan.FromSeconds(20)
                , IsMasterJob = true
                , MasterJobNegotiationChannelId = negotiationChannelId
                , MasterJobNegotiationChannelType = negotiationChannelType
                , MasterJobNegotiationChannelPriority = negotiationChannelPriority
                , MasterJobName = name
            };
        }
    }
}
