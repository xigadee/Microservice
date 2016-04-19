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
    public class ClientPriorityHolderStatistics: StatusBase
    {
        public Guid Id { get; set; }

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

        public string Algorithm { get; set; }

        public int Ordinal { get; set; }
        public int SkipCount { get; set; }
        public int Priority { get; set; }
        public decimal PriorityWeighting { get; set; }
        public long? PriorityCalculated { get; set; }
        public bool IsReserved { get; set; }
        public int? LastReserved { get; set; }
        public double CapacityPercentage { get; set; }
        public string Status { get; set; }
        public string MappingChannel { get; set; }
        public TimeSpan? PollLast { get; set; }
        public MessagingServiceStatistics Client { get; set; }

        public Exception LastException { get; set; }
        public DateTime? LastExceptionTime { get; set; }

        public decimal? PollSuccessRate { get; set; }
    }
}
