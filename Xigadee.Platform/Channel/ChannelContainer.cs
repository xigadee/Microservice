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
        private Dictionary<string, Channel> mContainerIncoming;
        private Dictionary<string, Channel> mContainerOutgoing;
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
            mContainerIncoming = new Dictionary<string, Channel>();
            mContainerOutgoing = new Dictionary<string, Channel>();
        }

        protected override void StopInternal()
        {
            mContainerIncoming.Clear();
            mContainerOutgoing.Clear();
        }

        /// <summary>
        /// This is a list of the incoming and outgoing channels.
        /// </summary>
        public IEnumerable<Channel> Channels
        {
            get
            {
                return mContainerIncoming.Values.Union(mContainerOutgoing.Values);
            }
        }

        /// <summary>
        /// This method adds a channel to the collection
        /// </summary>
        /// <param name="item">The channel to add.</param>
        public virtual void Add(Channel item)
        {
            switch (item.Direction)
            {
                case ChannelDirection.Incoming:
                    if (mContainerIncoming.ContainsKey(item.Id))
                        throw new DuplicateChannelException(item.Id, item.Direction);

                    mContainerIncoming.Add(item.Id, item);
                    break;
                case ChannelDirection.Outgoing:
                    if (mContainerOutgoing.ContainsKey(item.Id))
                        throw new DuplicateChannelException(item.Id, item.Direction);

                    mContainerOutgoing.Add(item.Id, item);
                    break;
            }
        }

        /// <summary>
        /// This method removes a channel from the collection.
        /// </summary>
        /// <param name="item">The channel item.</param>
        /// <returns>True if the channel is removed.</returns>
        public virtual bool Remove(Channel item)
        {
            switch (item.Direction)
            {
                case ChannelDirection.Incoming:
                    if (mContainerIncoming.ContainsKey(item.Id))
                        return mContainerIncoming.Remove(item.Id);
                    break;
                case ChannelDirection.Outgoing:
                    if (mContainerOutgoing.ContainsKey(item.Id))
                        return mContainerOutgoing.Remove(item.Id);
                    break;
            }

            return false;
        }
    }
}
