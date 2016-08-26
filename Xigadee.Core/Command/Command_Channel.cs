#region using
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
#endregion
namespace Xigadee
{
    public abstract partial class CommandBase<S, P, H>
    {
        #region ChannelId
        /// <summary>
        /// This is the default listening channel id for incoming requests.
        /// </summary>
        public virtual string ChannelId { get; set; }
        /// <summary>
        /// Specifies whether the channel can be autoset during configuration
        /// </summary>
        public virtual bool ChannelIdAutoSet
        {
            get { return mPolicy.ChannelIdAutoSet; }
        }
        #endregion

        #region ResponseChannelId
        /// <summary>
        /// This is the channel used for the response to outgoing messages.
        /// </summary>
        public virtual string ResponseChannelId
        {
            get; set;
        }
        /// <summary>
        /// Specifies whether the response channel can be set during configuration.
        /// </summary>
        public virtual bool ResponseChannelIdAutoSet
        {
            get { return mPolicy.ResponseChannelIdAutoSet; }
        }
        #endregion

        #region MasterJobNegotiationChannelId
        /// <summary>
        /// This is the channel used to negotiate control for a master job.
        /// </summary>
        public virtual string MasterJobNegotiationChannelId
        {
            get; set;
        }
        /// <summary>
        /// Specifies whether the master job negotiation channel can be set during configuration.
        /// </summary>
        public virtual bool MasterJobNegotiationChannelIdAutoSet
        {
            get { return mPolicy.MasterJobNegotiationChannelIdAutoSet; }
        }
        #endregion
    }
}
