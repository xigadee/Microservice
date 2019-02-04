using System.Collections.Generic;

namespace Xigadee
{
    /// <summary>
    /// This holds the current master job statistics.
    /// </summary>
    public class MasterJobStatistics
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="MasterJobStatistics"/> is enabled.
        /// </summary>
        public bool Enabled { get; set; }
        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// Gets or sets the server.
        /// </summary>
        public MasterJobPartner Master { get; set; }
        /// <summary>
        /// Gets or sets the standbys.
        /// </summary>
        public List<MasterJobPartner> Standbys { get; set; }
    }
}
