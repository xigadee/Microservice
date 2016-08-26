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
        #region Declarations
        private Dictionary<string,Channel> mContainer;
        #endregion
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
            mContainer = new Dictionary<string, Channel>();
        }

        protected override void StopInternal()
        {
            mContainer.Clear();
        }

        public virtual void Add(Channel item)
        {
            if (mContainer.ContainsKey(item.Id))
            {
                throw new DuplicateChannelException(item.Id);
            }

            mContainer.Add(item.Id, item);
        }

        public virtual bool Remove(Channel item)
        {
            if (mContainer.ContainsKey(item.Id))
            {
                return mContainer.Remove(item.Id);
            }

            return false;
        }
    }
}
