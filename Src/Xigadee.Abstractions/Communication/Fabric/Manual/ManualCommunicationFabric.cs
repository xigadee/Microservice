using System;
using System.Collections.Concurrent;
namespace Xigadee
{
    /// <summary>
    /// This is the communication bridge that simulates passing messages between Microservices and can be used for unit test based scenarios.
    /// </summary>
    public class ManualCommunicationFabric: CommunicationFabricBase<ICommunicationFabricBridge>
    {
        #region Declarations
        private ConcurrentDictionary<string, ManualFabricChannel> mChannels;

        private ConcurrentDictionary<ManualCommunicationFabricMode, ICommunicationFabricBridge> mAgents;
        #endregion
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ManualCommunicationFabric"/> class.
        /// </summary>
        public ManualCommunicationFabric(bool payloadHistoryEnabled = true, int? retryAttempts = null)
        {
            mChannels = new ConcurrentDictionary<string, ManualFabricChannel>();
            mAgents = new ConcurrentDictionary<ManualCommunicationFabricMode, ICommunicationFabricBridge>();

            PayloadHistoryEnabled = payloadHistoryEnabled;
            RetryAttempts = retryAttempts;
        } 
        #endregion

        /// <summary>
        /// Gets a value indicating whether the payload history will be stored.
        /// </summary>
        public bool PayloadHistoryEnabled { get; }
        /// <summary>
        /// Gets the retry attempts. Null if not specified.
        /// </summary>
        public int? RetryAttempts { get; }

        /// <summary>
        /// Gets the <see cref="ICommunicationFabricBridge"/> with the specified mode.
        /// </summary>
        /// <value>
        /// The <see cref="ICommunicationFabricBridge"/>.
        /// </value>
        /// <param name="mode">The mode.</param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException">The communication bridge mode is not supported</exception>
        public override ICommunicationFabricBridge this[ManualCommunicationFabricMode mode]
        {
            get
            {
                if (mode == ManualCommunicationFabricMode.NotSet)
                    throw new NotSupportedException("The communication bridge mode is not supported");

                return mAgents.GetOrAdd(mode, (m) => new ManualCommunicationBridgeAgent(this, m, PayloadHistoryEnabled, RetryAttempts));
            }
        }


        /// <summary>
        /// Creates the queue reader client.
        /// </summary>
        /// <param name="channelId">The channel identifier.</param>
        /// <returns>The manual fabric connection.</returns>
        public ManualFabricConnection CreateQueueClient(string channelId)
        {
            return GetChannel(channelId).CreateConnection(ManualFabricConnectionMode.Queue);
        }
        /// <summary>
        /// Creates the subscription reader client.
        /// </summary>
        /// <param name="channelId">The channel identifier.</param>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <returns>The manual fabric connection.</returns>
        public ManualFabricConnection CreateSubscriptionClient(string channelId, string subscriptionId)
        {
            return GetChannel(channelId).CreateConnection(ManualFabricConnectionMode.Subscription, subscriptionId);
        }
        /// <summary>
        /// Creates the transmit client.
        /// </summary>
        /// <param name="channelId">The channel identifier.</param>
        /// <returns>The manual fabric connection.</returns>
        public ManualFabricConnection CreateTransmitClient(string channelId)
        {
            return GetChannel(channelId).CreateConnection(ManualFabricConnectionMode.Transmit);
        }

        /// <summary>
        /// Gets the channel.
        /// </summary>
        /// <param name="channelId">The channel identifier.</param>
        /// <returns>The manual fabric connection.</returns>
        protected ManualFabricChannel GetChannel(string channelId)
        {
            ManualFabricChannel channel;
            if (!mChannels.TryGetValue(channelId, out channel))
            {
                channel = new ManualFabricChannel(channelId);
                //This might happen in a multi-threaded world.
                if (!mChannels.TryAdd(channelId, channel))
                {
                    channel = mChannels[channelId];
                }
            }
            return channel;
        }

    }
}
