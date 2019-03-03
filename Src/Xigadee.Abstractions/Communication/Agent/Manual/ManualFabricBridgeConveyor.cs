using System;
using System.Collections.Generic;
using System.Text;

namespace Xigadee
{
    public enum ManualFabricBridgeConveyorMode
    {
        QueueRoundRobin,
        QueueRandom,
        Subscription
    }

    /// <summary>
    /// This is the class used to distribute messages between senders and listeners.
    /// </summary>
    public class ManualFabricBridgeConveyor
    {
        private JsonContractSerializer mSerializer = new JsonContractSerializer();

        public ManualFabricBridgeConveyor(ManualFabricBridgeConveyorMode mode, string channelId, int priority)
        {
            Id = ToKey(channelId, priority);
            ChannelId = channelId;
            Priority = priority;
        }

        /// <summary>
        /// This is the mode that determines how incoming messages are distributed to outgoing parties.
        /// </summary>
        public ManualFabricBridgeConveyorMode Mode { get; }
        /// <summary>
        /// This is the unique key.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// This is the channel id.
        /// </summary>
        public string ChannelId { get; }

        /// <summary>
        /// This is the channel priority.
        /// </summary>
        public int Priority { get; }

        public void Enqueue(TransmissionPayload payload)
        {

        }

        public void Register(Action<TransmissionPayload> endpoint)
        {

        }

        /// <summary>
        /// Converts the combination of channel id and priority in to a unique key.
        /// </summary>
        /// <param name="channelId">This is the channel id.</param>
        /// <param name="priority">This is the channel priority.</param>
        /// <returns>The unique key.</returns>
        public static string ToKey(string channelId, int priority)
        {
            return $"{channelId}/{priority}";
        }

    }

    public static class ManualFabricBridgeConveyorHelper
    {
        public static string ToConveyorKey(this TransmissionPayload p)
        {
            return ManualFabricBridgeConveyor.ToKey(p.Message.ChannelId, p.Message.ChannelPriority);
        }
    }
}
