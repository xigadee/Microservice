using System;
using System.Collections.Concurrent;
namespace Xigadee
{
    /// <summary>
    /// This is the communication bridge that simulates passing messages between Microservices and can be used for unit test based scenarios.
    /// </summary>
    public class ManualFabric : CommunicationFabricBase<IManualCommunicationFabricBridge>
    {
        #region Declarations
        private ConcurrentDictionary<ManualCommunicationFabricMode, IManualCommunicationFabricBridge> mBridges;
        #endregion
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ManualFabric"/> class.
        /// </summary>
        public ManualFabric(bool payloadHistoryEnabled = true, int? retryAttempts = null)
        {
            mBridges = new ConcurrentDictionary<ManualCommunicationFabricMode, IManualCommunicationFabricBridge>();

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
        /// <param name="mode">The mode.</param>
        /// <returns>Returns the bridge.</returns>
        /// <exception cref="NotSupportedException">The communication bridge mode is not supported</exception>
        public override IManualCommunicationFabricBridge this[ManualCommunicationFabricMode mode]
        {
            get
            {
                if (mode == ManualCommunicationFabricMode.NotSet)
                    throw new NotSupportedException("The communication bridge mode is not supported");

                return mBridges.GetOrAdd(mode, (m) => 
                    new ManualFabricBridge(this, m, PayloadHistoryEnabled, RetryAttempts));
            }
        }
    }
}
