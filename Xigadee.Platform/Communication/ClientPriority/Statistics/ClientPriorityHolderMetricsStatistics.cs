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
    public class ClientPriorityHolderMetricsStatistics: StatusBase
    {

        public int Priority
        {
            get; set;
        }

        public decimal PriorityWeighting
        {
            get; set;

        }

        public long? QueueLength
        {
            get; set;
        }

        public double CapacityPercentage
        {
            get;set;
        }

        public bool IsDeadletter
        {
            get; set;
        }

        public string FabricPollWaitTime { get; set; }

        public string LastPoll
        {
            get; set;
        }

        public int? LastOffered
        {
            get; set;
        }

        public int? LastReserved
        {
            get; set;
        }

        public int? LastActual
        {
            get; set;
        }

        public DateTime? LastActualTime
        {
            get; set;
        }

        public string MaxAllowedPollWait
        {
            get; set;
        }

        public string MinExpectedPollWait
        {
            get; set;
        }

        public long PollAchievedBatch
        {
            get; set;
        }

        public long PollAttemptedBatch
        {
            get; set;
        }

        public string PollSuccessRate
        {
            get; set;
        }

        public string PollTimeReduceRatio
        {
            get; set;
        }


        public long? PriorityCalculated
        {
            get; set;
        }


        public string PriorityRecalculated
        {
            get; set;

        }



        public int SkipCount
        {
            get; set;

        }

        public string Status
        {
            get; set;
        }
    }
}
