#region using
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This policy determines how channels are managed in the Microservice.
    /// </summary>
    public class ChannelContainerPolicy: PolicyBase
    {
        /// <summary>
        /// This property specifies that channel can be created automatically if they do not exist.
        /// </summary>
        public bool AutoCreateChannels { get; set; } = true;
    }
}
