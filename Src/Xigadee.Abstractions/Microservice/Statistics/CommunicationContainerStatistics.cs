using System;
using System.Collections.Generic;
using System.Text;

namespace Xigadee
{
    /// <summary>
    /// This is the default statistics class.
    /// </summary>
    public class CommunicationContainerStatistics : StatusBase
    {
        #region Name
        /// <summary>
        /// Name override so that it gets serialized at the top of the JSON data.
        /// </summary>
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
        #endregion

        /// <summary>
        /// This list of active clients and their poll statistics.
        /// </summary>
        public ClientPriorityCollectionStatistics Active { get; set; }
        /// <summary>
        /// The senders collection.
        /// </summary>
        public List<MessagingServiceStatistics> Senders { get; set; }
        /// <summary>
        /// The listeners collection.
        /// </summary>
        public List<MessagingServiceStatistics> Listeners { get; set; }
    }

}
