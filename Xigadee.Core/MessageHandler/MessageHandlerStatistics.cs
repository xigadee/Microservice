#region using
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
#endregion
namespace Xigadee
{

    public class MessageHandlerStatistics: MessagingStatistics
    {
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public MessageHandlerStatistics() : base()
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
