using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public class JobStatistics: MessageInitiatorStatistics
    {
        public JobStatistics()
        {
            MasterJob = new MasterJobStatistics();
        }
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

        public MasterJobStatistics MasterJob { get; set; }
    }

    public class MasterJobStatistics
    {
        public bool Active { get; set; }
        public string Status { get; set; }
        public string Server { get; set; }

        public List<StandbyPartner> Standbys { get; set; }
    }
}
