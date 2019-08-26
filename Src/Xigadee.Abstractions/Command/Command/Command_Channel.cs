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
        #region ChannelId/ChannelIdAutoSet
        /// <summary>
        /// This is the default listening channel id for incoming requests.
        /// </summary>
        public virtual string ChannelId
        {
            get
            {
                return Policy.ChannelId;
            }
            set
            {
                Policy.ChannelId = value;
            }
        }
        /// <summary>
        /// Specifies whether the channel can be auto-set during configuration
        /// </summary>
        public virtual bool ChannelIdAutoSet
        {
            get { return Policy.ChannelIdAutoSet; }
        }
        #endregion
        #region ResponseChannelId/ResponseChannelIdAutoSet
        /// <summary>
        /// This is the channel used for the response to outgoing messages.
        /// </summary>
        public virtual string ResponseChannelId
        {
            get
            {
                return Policy.ResponseChannelId;
            }
            set
            {
                Policy.ResponseChannelId = value;
            }
        }
        /// <summary>
        /// Specifies whether the response channel can be set during configuration.
        /// </summary>
        public virtual bool ResponseChannelIdAutoSet
        {
            get { return Policy.ResponseChannelIdAutoSet; }
        }
        #endregion
    }
}
