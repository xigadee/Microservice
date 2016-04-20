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

        public Guid ClientId { get; set; }

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
        public int Ordinal { get; set; }

        public string Algorithm { get; set; }


        public string MappingChannel { get; set; }


        public bool IsReserved { get; set; }

        public int? Reserved { get; set; }


        public ClientPriorityHolderMetricsStatistics Metrics { get; set; }

        public Exception LastException { get; set; }

        public DateTime? LastExceptionTime { get; set; }
    }
}
