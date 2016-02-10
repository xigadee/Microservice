using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public class CommunicationStatistics: StatusBase
    {
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

        public int ActiveReservedSlots;

        public int ActiveAllowedOverage;

        //public int ActiveReservedSlots;
        public ClientPriorityStatistics ActiveListeners { get; set; }

        public List<MessagingServiceStatistics> Senders { get; set; }

        public List<MessagingServiceStatistics> Listeners { get; set; }

        public List<MessagingServiceStatistics> DeadLetterListeners { get; set; }

    }
}
