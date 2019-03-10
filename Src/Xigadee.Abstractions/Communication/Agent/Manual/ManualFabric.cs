using System;
using System.Collections.Generic;

namespace Xigadee
{
    /// <summary>
    /// This is the communication bridge that simulates passing messages between Microservices and can be used for unit test based scenarios.
    /// </summary>
    public class ManualFabric : CommunicationFabricBase<ICommunicationFabricBridge>
    {
        #region Declarations
        private List<ICommunicationFabricBridge> mBridges = new List<ICommunicationFabricBridge>();
        #endregion

        /// <summary>
        /// Gets a value indicating whether the payload history will be stored.
        /// </summary>
        public bool PayloadHistoryEnabled { get; } = true;
        /// <summary>
        /// Gets the retry attempts. Null if not specified.
        /// </summary>
        public int? RetryAttempts { get; } = 3;

        /// <summary>
        /// Gets a new communication bridge for the specified mode.
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <returns>Returns the new bridge.</returns>
        /// <exception cref="NotSupportedException">The communication bridge mode is not supported</exception>
        protected override ICommunicationFabricBridge this[CommunicationFabricMode mode]
        {
            get
            {
                if (mode == CommunicationFabricMode.NotSet)
                    throw new NotSupportedException("The communication bridge mode is not supported");

                var bridge = new ManualFabricBridge(this, mode, PayloadHistoryEnabled, RetryAttempts);

                mBridges.Add(bridge);

                return bridge;
            }
        }
    }
}
