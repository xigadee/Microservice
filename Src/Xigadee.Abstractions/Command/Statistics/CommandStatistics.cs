using System.Collections.Generic;

namespace Xigadee
{
    /// <summary>
    /// This class contains the default statistics for the command.
    /// </summary>
    /// <seealso cref="Xigadee.MessagingStatistics" />
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

        #region Name
        /// <summary>
        /// This is the messaging statistics name.
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
        /// Gets or sets the startup priority.
        /// </summary>
        public int StartupPriority { get; set; }
        /// <summary>
        /// Gets or sets the outgoing requests.
        /// </summary>
        public List<string> OutgoingRequests { get; set; }

        /// <summary>
        /// Gets or sets the supported handlers.
        /// </summary>
        public List<ICommandHandlerStatistics> SupportedHandlers { get; set; }
        /// <summary>
        /// Gets or sets the master job statistics.
        /// </summary>
        public MasterJobStatistics MasterJob { get; set; }

    }
}
