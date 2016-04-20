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

        public double CapacityPercentage
        {
            get;set;
        }

        public bool IsDeadletter
        {
            get; set;
        }

        public string LastPoll
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

        public decimal PollSuccessRate
        {
            get; set;
        }

        public decimal? PollTimeReduceRatio
        {
            get; set;
        }


        public long? PriorityCalculated
        {
            get; set;
        }

        public long? PriorityQueueLength
        {
            get; set;
        }

        public string PriorityRecalculated
        {
            get; set;

        }

        public decimal PriorityWeighting
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
