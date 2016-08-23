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
    /// This container holds the channels active within the Microservice, and tracks messages from incoming to outgoing.
    /// </summary>
    public class ChannelContainer: ServiceContainerBase<ChannelContainerStatistics, ChannelContainerPolicy>
    {
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public ChannelContainer(ChannelContainerPolicy policy = null):base(policy)
        {

        }
        #endregion

        protected override void StartInternal()
        {
        }

        protected override void StopInternal()
        {
        }
    }
}
