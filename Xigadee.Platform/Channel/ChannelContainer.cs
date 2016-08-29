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
    public class ChannelContainer: ServiceContainerBase<ChannelContainerStatistics, ChannelContainerPolicy>, IRequireSharedServices, IChannelService
    {
        #region Declarations
        private Dictionary<string, Channel> mContainerIncoming;
        private Dictionary<string, Channel> mContainerOutgoing;

        private ISharedService mSharedServices;
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

        #region Channels
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
        #endregion

        #region SharedServices
        /// <summary>
        /// This is the shared service collection for commands that wish to share direct access to internal data.
        /// </summary>
        public virtual ISharedService SharedServices
        {
            get
            {
                return mSharedServices;
            }

            set
            {
                SharedServicesChange(value);
            }
        }
        /// <summary>
        /// This method is called to set or remove the shared service reference.
        /// You can override your logic to safely set the shared service collection here.
        /// </summary>
        /// <param name="sharedServices">The shared service reference or null if this is not set.</param>
        protected virtual void SharedServicesChange(ISharedService sharedServices)
        {
            mSharedServices = sharedServices;
            mSharedServices.RegisterService<IChannelService>(this, "Channel");
        }
        #endregion

        #region Add(Channel item)
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
        #endregion
        #region Remove(Channel item)
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
        #endregion

        public bool Exists(string channelId, ChannelDirection direction)
        {
            switch (direction)
            {
                case ChannelDirection.Incoming:
                    if (mContainerIncoming.ContainsKey(channelId))
                        return true;
                    break;
                case ChannelDirection.Outgoing:
                    if (mContainerOutgoing.ContainsKey(channelId))
                        return true;
                    break;
            }

            return false;
        }

        public bool TryGet(string channelId, ChannelDirection direction, out Channel channel)
        {
            switch (direction)
            {
                case ChannelDirection.Incoming:
                    return mContainerIncoming.TryGetValue(channelId, out channel);
                case ChannelDirection.Outgoing:
                    return mContainerOutgoing.TryGetValue(channelId, out channel);
            }

            channel = null;
            return false;
        }
    }
}
