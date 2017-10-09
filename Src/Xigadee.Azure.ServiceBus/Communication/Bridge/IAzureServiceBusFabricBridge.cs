using System;
using System.Collections.Generic;
using System.Text;

namespace Xigadee
{
    /// <summary>
    /// This interface contains extensions to allow additional Service Bus paramters to be set for the listeners and senders.
    /// </summary>
    /// <seealso cref="Xigadee.ICommunicationBridge" />
    public interface IAzureServiceBusFabricBridge: ICommunicationBridge
    {
        /// <summary>
        /// Gets a listener agent for the bridge.
        /// </summary>
        /// <returns>Returns the listener agent.</returns>
        IListener GetListener(string entityName);
        /// <summary>
        /// Gets a sender for the bridge.
        /// </summary>
        /// <returns>Returns the sender agent.</returns>
        ISender GetSender(string entityName);
    }
}
