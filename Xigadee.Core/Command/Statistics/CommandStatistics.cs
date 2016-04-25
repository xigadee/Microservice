using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public class CommandStatistics: MessagingStatistics
    {
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public CommandStatistics() : base()
        {
            MasterJob = new MasterJobStatistics();
        }
        #endregion

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

        public int StartupPriority { get; set; }

        public List<string> OutgoingRequests { get; set; }


        public List<ICommandHandlerStatistics> SupportedHandlers { get; set; }

        public MasterJobStatistics MasterJob { get; set; }

    }
}
