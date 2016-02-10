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
    public class MessageInitiatorStatistics:MessageHandlerStatistics
    {
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="batchSize">The batch size to recycle the statistics.</param>
        public MessageInitiatorStatistics():base()
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
    }
}
