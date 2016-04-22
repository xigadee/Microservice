using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This is the statistics class used to record the status of the command.
    /// </summary>
    public class PersistenceInitiatorStatistics: CommandStatistics
    {
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public PersistenceInitiatorStatistics() : base()
        {
        }
        #endregion


        public string TypeKey { get; set; }
        public string TypeEntity { get; set; }
    }
}
