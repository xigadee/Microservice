using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// The channel class is used to simplify the connection between communication and command components.
    /// </summary>
    public class Channel
    {
        /// <summary>
        /// THe default constructor.
        /// </summary>
        /// <param name="Id">The channel Id.</param>
        /// <param name="Direction">The direction of the channel - Incoming or outgoing</param>
        /// <param name="Description">The optional description</param>
        public Channel(string Id, ChannelDirection Direction, string Description = null)
        {
            if (string.IsNullOrEmpty(Id))
                throw new ArgumentNullException("Id cannot be null or empty.");

            this.Id = Id;
            this.Direction = Direction;
            this.Description = Description;
        }

        /// <summary>
        /// The channel Id.
        /// </summary>
        public string Id { get; }
        /// <summary>
        /// An optional description of the channel
        /// </summary>
        public string Description { get; set;}
        /// <summary>
        /// The direction of the channel - Incoming or outgoing
        /// </summary>
        public ChannelDirection Direction { get; }
    }
}
