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

        public List<string> SupportedHandlers { get; set; }
    }
}
