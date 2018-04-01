using System;
using System.Collections.Generic;
using System.Text;

namespace Xigadee
{
    /// <summary>
    /// This interface is implemented by components that require a direct access to the 
    /// system channels.
    /// </summary>
    public interface IRequireChannelService
    {
        /// <summary>
        /// The service reference.
        /// </summary>
        IChannelService ChannelService { get; set; }
    }
}
